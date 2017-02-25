//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Scratcher.Core.Devices;

//namespace Scratcher.Core.Test.Devices
//{
//	[TestClass]
//	public class IphoneRecordingMetadataTests
//	{
//		[TestMethod]
//		public void Filename_GivenAPathIsDefined_ReturnsFilename()
//		{
//			var metadata = new SqLiteReader.IphoneRecordingDbMetadata();
//			metadata.Path = "first/second/third/file.txt";
//			Assert.AreEqual("file.txt", metadata.Filename);
//		}

//		[TestMethod]
//		public void Timestamp_GivenAPathIsDefinedInTheExpectedFormat_ReturnsTimestamp()
//		{
//			var metadata = new SqLiteReader.IphoneRecordingDbMetadata();
//			metadata.Path = "/var/mobile/Media/Recordings/20161228 214457.m4a";
//			var expected = new DateTime(2016, 12, 28, 21, 44, 57, 0, DateTimeKind.Local);
//			Assert.AreEqual(expected, metadata.Timestamp);
//		}

//		[TestMethod]
//		public void Timestamp_GivenAPathIsDefinedInTheExpectedFormatIncludingLeadingZeros_ReturnsTimestamp()
//		{
//			var metadata = new SqLiteReader.IphoneRecordingDbMetadata();
//			metadata.Path = "/var/mobile/Media/Recordings/20140116 022048.m4a";
//			var expected = new DateTime(2014, 1, 16, 02, 20, 48, 0, DateTimeKind.Local);
//			Assert.AreEqual(expected, metadata.Timestamp);
//		}

//		[TestMethod]
//		public void Timestamp_GivenAPathIsDefinedInAnUnexpectedFormat_ReturnsTimestamp()
//		{
//			var metadata = new IphoneRecordingDbMetadata();
//			metadata.Path = "/var/mobile/Media/Recordings/helloisitmeyourelookingfor.jpg";
//			var expected = DateTimeOffset.MinValue;
//			Assert.AreEqual(expected, metadata.Timestamp);
//		}
//	}
//}
