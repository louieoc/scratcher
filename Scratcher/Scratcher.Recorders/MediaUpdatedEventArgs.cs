using System;
using Scratcher.Recorders.Metadata;

namespace Scratcher.Recorders
{
	public class MediaUpdatedEventArgs : EventArgs
	{
		public string DeviceName { get; set; }
		public MediaCollection Audio { get; set; }
	}
}
