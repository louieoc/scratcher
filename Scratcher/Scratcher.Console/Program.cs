using System;
using System.Threading;
using Scratcher.Recorders;
using Scratcher.Recorders.IPhone;
using Scratcher.Recorders.Metadata;

namespace Scratcher.Console
{
	class Program
	{
		private static IAudioRecorder _applieiPhone;

		static void Main(string[] args)
		{
			const int runFor = 300;
			_applieiPhone = new VoiceMemosRecorder(@"D:\programs\scratcher\Scratcher\testfiles");
			var waittime = TimeSpan.FromSeconds(runFor);
			var then = DateTime.Now.Add(waittime);
			_applieiPhone.MediaUpdated += OnMediaUpdated;
			_applieiPhone.StartListening();
			while (DateTime.Now < then)
			{
				Thread.Sleep(1000);
			}
			_applieiPhone.StopListening();
		}

		private static void OnMediaUpdated(object sender, MediaUpdatedEventArgs args)
		{
			foreach (var media in args.Audio)
			{
				WriteMediaFileToConsole(media);
			}
			_applieiPhone.CopyAudio(@"C:\voicememostest\");
		}

		private static void WriteMediaFileToConsole(MediaFile mediaFile)
		{
			System.Console.WriteLine("{0}", mediaFile.Title);
			System.Console.WriteLine("{0}", mediaFile.Duration);
			System.Console.WriteLine("{0}", mediaFile.RecordingDate);
			System.Console.WriteLine("{0}", mediaFile.Filepath);
			System.Console.WriteLine("{0}", mediaFile.Filename);

			var voiceMemo = mediaFile as SqLiteReader.IphoneRecordingDbMetadata;
			if (voiceMemo != null)
			{
				System.Console.WriteLine("ID: {0}", voiceMemo.RecordingID);
				System.Console.WriteLine("Zdate: {0}", voiceMemo.ZDate);
			}

			System.Console.WriteLine();
		}
	}
}
