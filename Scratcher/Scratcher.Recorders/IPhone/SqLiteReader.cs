using System;
using System.Data.SQLite;
using System.IO;
using Scratcher.Recorders.Metadata;

namespace Scratcher.Recorders.IPhone
{
	public class SqLiteReader
	{
		private const string RecordingsQuery = "select cast(zdate as float) as zdate, zduration, zcustomlabel, zpath, zrecordingid from zrecording";
		private readonly string _databaseFilePath;
		private const int IntValueThatMeansUndefined = -1;
		private readonly DateTimeOffset _dateTimeOffsetValueThatMeansUndefined = DateTimeOffset.MinValue;

		public SqLiteReader(string databaseFilePath)
		{
			_databaseFilePath = databaseFilePath;
		}

		public MediaCollection GetVoiceMemoMediaCollections()
		{
			using (var dbConnection =GetDbConnection())
			{
				dbConnection.Open();
				var sql = RecordingsQuery;
				var command = new SQLiteCommand(sql, dbConnection);
				var collection = new MediaCollection();

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var mediaFile = MapReaderToIphoneRecordingMetadata(reader) as MediaFile;
						collection.Add(mediaFile);
					}
				}

				return collection;
			}
		}

		public MediaFile GetVoiceMemoMediaFile(string filename)
		{
			using (var dbConnection = GetDbConnection())
			{
				dbConnection.Open();
				var sql = string.Format("{0} where zpath like @filenamepattern", RecordingsQuery);
				var command = new SQLiteCommand(sql, dbConnection);
				command.Parameters.AddWithValue("@filenamepattern", string.Format("%/{0}", filename));

				using (var reader = command.ExecuteReader())
				{
					reader.Read();
					return MapReaderToIphoneRecordingMetadata(reader);
				}
			}
		}

		private SQLiteConnection GetDbConnection()
		{
			return new SQLiteConnection(string.Format("Data Source={0};Version=3;datetimeformat=CurrentCulture", _databaseFilePath));
		}

		private IphoneRecordingDbMetadata MapReaderToIphoneRecordingMetadata(SQLiteDataReader reader)
		{
			var filepath = reader["zpath"].ToString();
			var filename = GetFilenameFromPath(filepath);
			return new IphoneRecordingDbMetadata
			{
				Title = reader["zcustomlabel"].ToString(),
				Duration = GetTimespanFromSecondsString(reader["zduration"]),
				RecordingID = GetRecordingID(reader["zrecordingid"]),
				Filepath = filepath,
				ZDate = GetTimestampFromUnixTimeString(reader["zdate"]),
				Filename = filename,
				RecordingDate = GetDateTimeOffsetFromFilename(filename)
			};
		}

		private string GetFilenameFromPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				return string.Empty;

			var fileInfo = new FileInfo(path);
			return fileInfo.Name;
		}

		private int GetRecordingID(object o)
		{
			int recordingID;
			return int.TryParse(o.ToString(), out recordingID)
				? recordingID
				: IntValueThatMeansUndefined;
		}

		private DateTimeOffset GetTimestampFromUnixTimeString(object o)
		{
			double timestamp;
			return !double.TryParse(o.ToString(), out timestamp)
				? DateTimeOffset.MinValue
				: UnixTimeStampToDateTime(timestamp);
		}

		private DateTimeOffset GetDateTimeOffsetFromFilename(string filename)
		{
			try
			{
				// we're parsing this so don't get fancy: 20161228 214457.m4a
				var filenameParts = filename.Split(' ');
				var datepart = filenameParts[0];
				var year = datepart.Substring(0, 4);
				var month = datepart.Substring(4, 2);
				var day = datepart.Substring(6, 2);

				var timehalf = filenameParts[1];
				var timehalves = timehalf.Split('.');
				var timepart = timehalves[0];
				var hour = timepart.Substring(0, 2);
				var minute = timepart.Substring(2, 2);
				var second = timepart.Substring(4, 2);

				var date = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day),
					int.Parse(hour), int.Parse(minute), int.Parse(second), DateTimeKind.Local);

				return date;
			}
			catch
			{
				// swallowing the exception because we don't really care that much
				return _dateTimeOffsetValueThatMeansUndefined;
			}
		}

		private static DateTimeOffset UnixTimeStampToDateTime(double unixTimeStamp)
		{
			// Unix timestamp is seconds past epoch
			var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
			return dtDateTime;
		}

		private TimeSpan GetTimespanFromSecondsString(object o)
		{
			double duration;
			return !double.TryParse(o.ToString(), out duration) 
				? TimeSpan.MinValue 
				: TimeSpan.FromSeconds(duration);
		}

		public class IphoneRecordingDbMetadata : MediaFile
		{
			public int RecordingID { get; set; }

			/// <summary>
			/// this value stored in the db appears to be an epoch time evaluating to dates in the 1980s, so not quite
			/// the actual recording date
			/// </summary>
			public DateTimeOffset ZDate { get; set; }
		}
	}
}