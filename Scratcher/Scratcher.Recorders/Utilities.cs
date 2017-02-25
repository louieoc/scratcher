using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Scratcher.Recorders
{
	public class FileUtilities
	{
		public static string SanitizeWindowsFilename(string filename)
		{
			var sanitized = filename;
			// replace some characters with dashes
			sanitized = Regex.Replace(sanitized, @"[//\\|]", "-");
			// and some other characters with nothing
			sanitized = Regex.Replace(sanitized, @"[<>:\?\*""]", "");
			return sanitized;
		}

		public static void SetFileDateTimeAttributes(string filepath, DateTimeOffset createdDate, DateTimeOffset modifiedDate)
		{
			File.SetCreationTimeUtc(filepath, createdDate.ToUniversalTime().DateTime);
			File.SetLastWriteTimeUtc(filepath, modifiedDate.ToUniversalTime().DateTime);
		}

		public static void MakeSureTheTargetPathExists(string targetPath)
		{
			if (!Directory.Exists(targetPath))
			{
				Directory.CreateDirectory(targetPath);
			}
		}
	}
}
