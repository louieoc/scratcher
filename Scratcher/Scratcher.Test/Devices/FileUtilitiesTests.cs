using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scratcher.Recorders;

namespace Scratcher.Test.Devices
{
	[TestClass]
	public class FileUtilitiesTests
	{
		[TestMethod]
		public void SanitizeWindowsFilename_GivenIllegalCharacters_ReturnsSanitizedString()
		{
			var title = "song in 3/4 time";
			var expected = "song in 3-4 time";
			Assert.AreEqual(expected, FileUtilities.SanitizeWindowsFilename(title));

			title = "song in 3\\4 time";
			expected = "song in 3-4 time";
			Assert.AreEqual(expected, FileUtilities.SanitizeWindowsFilename(title));

			title = "song in 3/4 time?";
			expected = "song in 3-4 time";
			Assert.AreEqual(expected, FileUtilities.SanitizeWindowsFilename(title));

			title = "song in 3><4 time";
			expected = "song in 34 time";
			Assert.AreEqual(expected, FileUtilities.SanitizeWindowsFilename(title));

			title = "song in 3|4 time";
			expected = "song in 3-4 time";
			Assert.AreEqual(expected, FileUtilities.SanitizeWindowsFilename(title));

			title = "song: in 3/4 time";
			expected = "song in 3-4 time";
			Assert.AreEqual(expected, FileUtilities.SanitizeWindowsFilename(title));
		}
	}
}
