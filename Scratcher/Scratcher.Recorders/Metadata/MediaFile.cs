using System;
using System.Collections.Concurrent;

namespace Scratcher.Recorders.Metadata
{
	public class MediaFile
	{
		public string Title { get; set; }
		public string Filename { get; set; }
		public string Filepath { get; set; }
		public TimeSpan Duration { get; set; }
		public DateTimeOffset RecordingDate { get; set; }
	}

	public class Song
	{
		public MediaCollection Audio { get; set; }
	}

	public class MediaCollection : ConcurrentBag<MediaFile>
	{
		public string Name { get; set; }
	}
}