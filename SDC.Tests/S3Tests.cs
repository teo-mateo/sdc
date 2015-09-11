using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SDC.Library.S3;
using System.IO;

namespace SDC.Tests
{
    [TestClass]
    public class S3Tests
    {
        //[TestMethod]
        //public void UploadUserAvatar_File_Test()
        //{
        //    string path = @"C:\Users\andreea.botoc\Dropbox\Camera Uploads\pic1.jpg";
        //    var userid = Guid.NewGuid().ToString();
        //    S3File f = S3.UploadUserAvatar(userid, path);
        //    Assert.IsTrue(f.Url.StartsWith("https://sdc-dev.s3.amazonaws.com/profile_pics/" + userid + "/pic1.jpg"));
        //}

        [TestMethod]
        public void UploadUserAvatar_Stream_Test()
        {
            string path = @"C:\Users\andreea.botoc\Dropbox\Camera Uploads\pic1.jpg";
            var userid = Guid.NewGuid().ToString();
            using (var fs = new FileStream(path, FileMode.Open))
            {
                userid = Guid.NewGuid().ToString();
                S3File f = S3.UploadUserAvatar(userid, "pic1.jpg", fs);
                Assert.IsTrue(f.Url.StartsWith("https://sdc-dev.s3.amazonaws.com/profile_pics/" + userid + "/pic1.jpg"));
            }
        }
    }
}
