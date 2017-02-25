using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using MK.MobileDevice;
using MK.MobileDevice.TAI;
using MK.MobileDevice.XEDevice;
using Scratcher.Recorders.Metadata;

namespace Scratcher.Recorders.IPhone
{
	public class VoiceMemosRecorder : IAudioRecorder
	{
		private iTMDiPhone _iphone;
		private iTMDiPhoneLL _lliph;
		private iPhone _mdv;
		private TAIiPhone _tai;
		private iPhoneXE xe;

		private readonly string _localCachePath;
		private BackgroundWorker _backgroundWorker;
		private const string TargetExtension = "m4a";

		public VoiceMemosRecorder(string localCachePath)
		{
			_localCachePath = localCachePath;
			Audio = new MediaCollection();
		}

		public event EventHandler<MediaUpdatedEventArgs> MediaUpdated;

		public MediaCollection Audio { get; private set; }

		public void StartListening()
		{
			if (_backgroundWorker == null)
			{
				InitializeBackgroundWorker();
			}

			// start the background thread
			if (_backgroundWorker.IsBusy)
			{
				return;
			}

			_backgroundWorker.RunWorkerAsync();
		}

		public void StopListening()
		{
			_backgroundWorker.CancelAsync();
		}

		public void CopyAudio(string targetPath)
		{
			FileUtilities.MakeSureTheTargetPathExists(targetPath);

			LoopThroughPhoneRecordingsAndDoStuffToThem((recording, currentPath) =>
			{
				if (!recording.ToLower().EndsWith(TargetExtension)) return;

				string updatedRecordingName = recording;
				var meta = Audio.FirstOrDefault(a => a.Filename == recording);
				if (meta != null)
				{
					updatedRecordingName = UpdateRecordingFileName(recording, updatedRecordingName, meta);
				}

				var thisFile = Combine(currentPath, recording);
				Console.WriteLine("Copying {0} to local cache at {1}...", recording, _localCachePath);
				var to = Path.Combine(targetPath, updatedRecordingName);

				Retry(() =>
				{
					try
					{
						var copied = _mdv.CopyFileFromDevice(to, thisFile);
						if (!copied)
						{
							_mdv.ReConnect();
							return false;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						throw;
					}

					if (meta != null)
					{
						if (File.Exists(to))
						{
							SetTitle(to, meta.Title);
							FileUtilities.SetFileDateTimeAttributes(to, meta.RecordingDate, meta.RecordingDate);
						}
						else
						{
							// now what?
							return false;
						}
					}
					return File.Exists(to);
				});
			});

			// raise some kind of "AudioCopied" event?
		}

		private static string UpdateRecordingFileName(string recording, string updatedRecordingName, MediaFile meta)
		{
			var filenameWithoutExtension = recording.Substring(0, recording.Length - 4);
			updatedRecordingName = string.Format("{0} {1}.{2}",
				filenameWithoutExtension,
				FileUtilities.SanitizeWindowsFilename(meta.Title),
				TargetExtension);
			return updatedRecordingName;
		}

		private bool Retry(Func<bool> func)
		{
			const int maxRetry = 3;
			var attempt = 0;
			var succeeded = false;

			while (attempt < maxRetry && !succeeded)
			{
				succeeded = func();
				if (succeeded) return true;
				attempt++;
			}

			return false;
		}

		private void InitializeBackgroundWorker()
		{
			_backgroundWorker = new BackgroundWorker
			{
				WorkerReportsProgress = false,
				WorkerSupportsCancellation = true
			};

			_backgroundWorker.DoWork += Listen;
		}

		protected void OnDeviceConnected(EventArgs e)
		{
			GetAudioData();

			if (MediaUpdated != null)
			{
				var args = new MediaUpdatedEventArgs { Audio = Audio };
				MediaUpdated(this, args);
			}
		}

		// TagLib examples:
		// http://stackoverflow.com/questions/4361587/where-can-i-find-tag-lib-sharp-examples
		private void SetTitle(string filepath, string title)
		{
			try
			{
				using (var tagFile = TagLib.File.Create(filepath))
				{
					tagFile.Tag.Title = title;
					tagFile.Save();
				}
			}
			catch (TagLib.CorruptFileException)
			{
				// can't do much with this
				return;
			}
			catch (FileNotFoundException)
			{
				// also, what do I do with this?
				return;
			}
		}

		private void GetAudioData()
		{
			LoopThroughPhoneRecordingsAndDoStuffToThem((recording, currentPath) =>
			{
				if (recording != "Recordings.db") return;

				var thisFile = Combine(currentPath, recording);
				Console.WriteLine("Copying {0} to local cache at {1}...", recording, _localCachePath);
				var to = Path.Combine(_localCachePath, recording);
				_mdv.CopyFileFromDevice(to, thisFile);

				var sqLiteReader = new SqLiteReader(to);
				Audio = sqLiteReader.GetVoiceMemoMediaCollections();
			});
		}

		private void LoopThroughPhoneRecordingsAndDoStuffToThem(Action<string, string> func)
		{
			var currentPath = _mdv.CurrentDirectory;
			var dirs = _mdv.GetDirectories(currentPath);
			foreach (var dir in dirs)
			{
				if (dir != "Recordings") continue;

				currentPath = Combine(currentPath, dir);
				var recordings = _mdv.GetContents(dir);
				foreach (var recording in recordings)
				{
					func(recording, currentPath);
				}
			}
		}

		private void Listen(object sender, DoWorkEventArgs eventArgs)
		{
			var worker = sender as BackgroundWorker;
			if (worker == null || worker.CancellationPending)
			{
				return;
			}

			Console.WriteLine("Listening for iPhone...");

			//_iphone = new iTMDiPhone();
			//_lliph = new iTMDiPhoneLL();
			try
			{
				_mdv = new iPhone();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			_tai = new TAIiPhone();

			_mdv.Connect += Mdv_Connect;
			_mdv.HostAttached += Mdv_HostAttached;
			_iphone.Connect += IphoneConnect;
			_tai.Connect += Tai_Connect;
			_lliph.HostAttached += Lliph_HostAttached;
			_iphone.DfuConnect += IphoneDfuConnect;

			while (!worker.CancellationPending)
			{
				if (_mdv.attachedToHost ^ _mdv.IsConnected)
				{
					//Console.Clear();
					//Console.WriteLine("Device attached but not paired. Enter passcode on device to pair.");
				}
				Thread.Sleep(100);
			}
		}

		private void Lliph_HostAttached(object sender, ITMDConnectEventArgs args)
		{
			Console.WriteLine("iTMD.dll detected device attached to host.");
			Console.WriteLine(_lliph.DeviceName);
		}

		private void Mdv_HostAttached(object sender, USBMultiplexArgs args)
		{
			Console.WriteLine("libimobiledevice.dll detected device attached to host.");
			Console.WriteLine("Device Locked: {0}", args.IsLocked);
		}

		private void Tai_Connect(object sender, MK.MobileDevice.TAI.ConnectEventArgs args)
		{

		}

		private void IphoneDfuConnect(object sender, EventArgs e)
		{
			Console.WriteLine("iTunesMobileDevice.dll Connected to [DFU]" + _iphone.DeviceName);
			Console.WriteLine("Requesting Recovery...");
			_iphone.EnterDFU();
			Console.ReadKey();
		}

		private void Mdv_Connect(object sender, MK.MobileDevice.ConnectEventArgs args)
		{
			Console.WriteLine("libimobiledevice.dll Connected [MUX] to " + _mdv.DeviceName);
			Console.WriteLine("IsWifiConnect: {0}", _mdv.IsWifiConnect);
			OnDeviceConnected(args);
		}

		private static string Combine(string path1, string path2)
		{
			if (path1.EndsWith("/"))
			{
				path1 = path1.Substring(0, path1.Length - 1);
			}
			return string.Format("{0}/{1}", path1, path2);
		}

		private void IphoneConnect(object sender, ITMDConnectEventArgs args)
		{
			Console.WriteLine("iTunesMobileDevice.dll Connected to " + _iphone.DeviceName);
			string dn = _iphone.DeviceName;
			string pn = _iphone.ProductName;
			string pv = _iphone.ProductVersion;
			string infoS = String.Format("connected to: {0} ({1}, iOS {2})", dn, pn, pv);
			Console.WriteLine("FMIP AMDeviceCopyValue: {0}", _iphone.RequestProperty("com.apple.fmip", "IsAssociated"));
		}
	}
}
