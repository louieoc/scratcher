using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scratcher.Recorders.IPhone;
using Scratcher.Recorders.Metadata;

namespace Scratcher.Recorders.Test.Devices
{
	[TestClass]
	public class IphoneVoiceMemosTests
	{
		[TestMethod]
		public void IphoneVoiceMemosMediaCollectionBuilder_AddSqLiteMetadata_AddsMetadata()
		{
			throw new NotImplementedException();
		}

		[TestMethod]
		public void blah_blah()
		{
			var voiceMemos = new VoiceMemosRecorder(@"D:\programs\scratcher\Scratcher\testfiles");
			var waittime = TimeSpan.FromSeconds(10);
			var then = DateTime.Now.Add(waittime);
			voiceMemos.StartListening();
			while (DateTime.Now < then)
			{
				Thread.Sleep(100);
			}
			voiceMemos.StopListening();
			foreach (var media in voiceMemos.Audio)
			{
				WriteMediaFileToConsole(media);
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
	}
}
