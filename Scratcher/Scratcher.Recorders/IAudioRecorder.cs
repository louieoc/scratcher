using System;
using Scratcher.Recorders.Metadata;

namespace Scratcher.Recorders
{
	public interface IAudioRecorder
	{
		event EventHandler<MediaUpdatedEventArgs> MediaUpdated;
		MediaCollection Audio { get; }
		void StartListening();
		void StopListening();
		void CopyAudio(string targetPath);
	}
}
