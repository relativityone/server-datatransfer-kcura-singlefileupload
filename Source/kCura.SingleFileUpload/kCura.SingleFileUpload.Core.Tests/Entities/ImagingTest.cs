using NUnit.Framework;
using kCura.SingleFileUpload.Core.Entities;

namespace kCura.SingleFileUpload.Core.Tests.Entities
{
	[TestFixture]
	public class ImagingTest
	{
		private Imaging _imaging;

		[SetUp]
		public void SetUp()
		{
			_imaging = new Imaging();
		}

		[Test]
		public void TestImagingProperties()
		{
			// Act
			_imaging.Fdv = true;
			_imaging.Image = false;
			_imaging.NewImage = null;
			_imaging.DocID = 1;
			_imaging.ProfileID = null;
			_imaging.ErrorFile = 0;
			_imaging.Fri = true;

			// Assert
			Assert.IsTrue(_imaging.Fdv);
			Assert.IsFalse(_imaging.Image);
			Assert.IsNull(_imaging.NewImage);
			Assert.AreEqual(1, _imaging.DocID);
			Assert.IsNull(_imaging.ProfileID);
			Assert.AreEqual(0, _imaging.ErrorFile);
			Assert.IsTrue(_imaging.Fri);
		}
	}
}