using Amazon.S3;
using Amazon.S3.Model;
using SDC.data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDC.Library.S3
{
    public class S3File
    {
        public string Key { get; set; }
        public string Url { get; set; }
    }

    public class S3
    {
        private static string root_profile_pics = "profile_pics";

        private static string _s3_access_key;
        private static string _s3_secret_access_key;
        private static string _s3_bucket_name;
        private static string _s3_bucket_region;
        static S3()
        {
            _s3_bucket_name = System.Configuration.ConfigurationManager.AppSettings["S3:BucketName"];
            _s3_bucket_region = ConfigurationManager.AppSettings["S3:BucketRegion"];

            using (var db = new SDCContext())
            {
                _s3_access_key = db.Settings.Find("s3_access_key").Value;
                _s3_secret_access_key = db.Settings.Find("s3_secret_access_key").Value;
            }
        }

        public static S3File UploadUserAvatar(string userid, string fileName, Stream inputStream)
        {
            using (var cli = new AmazonS3Client(
                _s3_access_key, 
                _s3_secret_access_key, new AmazonS3Config()
            {
                ServiceURL = "http://" + _s3_bucket_region
            }))
            {
                //root_folder/userid/pic.jpg
                string key = String.Format("{0}/{1}/{2}", root_profile_pics, userid, fileName);

                PutObjectRequest req = new PutObjectRequest()
                {
                    BucketName = _s3_bucket_name,
                    ContentType = "image/jpg",
                    InputStream = inputStream,
                    Key = key,
                    CannedACL = S3CannedACL.PublicRead
                };

                var response = cli.PutObject(req);
                if(response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("upload failed.");
                }
                else
                {
                    var psur = new GetPreSignedUrlRequest()
                    {
                        BucketName = _s3_bucket_name,
                        Key = key
                    };
                    var url = cli.GetPreSignedURL(psur);
                    return new S3File()
                    {
                        Key = key,
                        Url = url
                    };
                }
            }
        }

        public static void DeleteUserAvatar(string key)
        {
            throw new NotImplementedException();
        }
    }
}
