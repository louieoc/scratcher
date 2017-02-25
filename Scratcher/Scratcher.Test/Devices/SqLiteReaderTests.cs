using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scratcher.Recorders.IPhone;
using Scratcher.Recorders.Metadata;

namespace Scratcher.Recorders.Test.Devices
{
	[TestClass]
	public class SqLiteReaderTests
	{
		private const string DatabaseFilepath = @"D:\programs\scratcher\Scratcher\Scratcher.Core.Test\bin\cache\Recordings.db";

		[TestMethod]
		[TestCategory("DependentOnSpecificSQLiteDatabase")]
		public void GetVoiceMemoData_GivenFilename_ReturnsMediaFile()
		{
			var reader = new SqLiteReader(DatabaseFilepath);
			var metadata = reader.GetVoiceMemoMediaFile("20161228 214457.m4a");
			Assert.IsNotNull(metadata);
			Assert.AreEqual("Lux buildup at the end", metadata.Title);
			WriteMediaFileToConsole(metadata);
		}

		[TestMethod]
		[TestCategory("DependentOnSpecificSQLiteDatabase")]
		public void GetVoiceMemoData_ReturnsMediaCollection()
		{
			var reader = new SqLiteReader(DatabaseFilepath);
			var metadatas = reader.GetVoiceMemoMediaCollections().ToList();
			Assert.IsNotNull(metadatas);
			Assert.IsTrue(metadatas.Any());
			foreach (var metadata in metadatas)
			{
				WriteMediaFileToConsole(metadata);
			}
		}

		private static void WriteMediaFileToConsole(MediaFile mediaFile)
		{
			Console.WriteLine("{0}", mediaFile.Title);
			Console.WriteLine("{0}", mediaFile.Duration);
			Console.WriteLine("{0}", mediaFile.RecordingDate);
			Console.WriteLine("{0}", mediaFile.Filepath);
			Console.WriteLine("{0}", mediaFile.Filename);

			var voiceMemo = mediaFile as SqLiteReader.IphoneRecordingDbMetadata;
			if (voiceMemo != null)
			{
				Console.WriteLine("ID: {0}", voiceMemo.RecordingID);
				Console.WriteLine("Zdate: {0}", voiceMemo.ZDate);
			}

			Console.WriteLine();
		}


		//[TestMethod]
		//[TestCategory("DependentOnSpecificSQLiteDatabase")]
		//public void GetIphoneRecordingMetadataForFilename_GivenFilename_ReturnsMetadata()
		//{
		//	var reader = new SqLiteReader(DatabaseFilepath);
		//	var metadata = reader.GetIphoneRecordingMetadataForFilename("20161228 214457.m4a");
		//	Assert.IsNotNull(metadata);
		//	Assert.AreEqual("Lux buildup at the end", metadata.CustomLabel);
		//	WriteMediaFileToConsole(metadata);
		//}

		//[TestMethod]
		//[TestCategory("DependentOnSpecificSQLiteDatabase")]
		//public void GetAllIphoneRecordingMetadata_ReturnsMetadata()
		//{
		//	var reader = new SqLiteReader(DatabaseFilepath);
		//	var metadatas = reader.GetAllIphoneRecordingMetadata().ToList();
		//	Assert.IsNotNull(metadatas);
		//	Assert.IsTrue(metadatas.Any());
		//	foreach (var metadata in metadatas)
		//	{
		//		WriteMediaFileToConsole(metadata);
		//	}
		//}

		//private static void WriteIphoneRecordingMetadataToConsole(MediaFile mediaFile)
		//{
		//	Console.WriteLine("{0}", mediaFile.Title);
		//	Console.WriteLine("{0}", mediaFile.Duration);
		//	Console.WriteLine("{0}", mediaFile.RecordingDate);
		//	Console.WriteLine("ID: {0}", mediaFile.RecordingID);
		//	Console.WriteLine("{0}", mediaFile.Filepath);
		//	Console.WriteLine("{0}", mediaFile.Filename);
		//	Console.WriteLine("Zdate: {0}", mediaFile.ZDate);
		//	Console.WriteLine();
		//}
	}
}
