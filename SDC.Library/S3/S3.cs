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
using System.Web;

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
            var s3Config = new AmazonS3Config() { ServiceURL = "http://" + _s3_bucket_region };
            using (var cli = new AmazonS3Client(
                _s3_access_key, 
                _s3_secret_access_key, 
                s3Config))
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
                    throw new Exception("s3: upload failed.");
                }
                else
                {
                    var psur = new GetPreSignedUrlRequest()
                    {
                        BucketName = _s3_bucket_name,
                        Key = key, 
                        Expires = DateTime.Now.AddDays(3000)
                    };


                    return new S3File()
                    {
                        Key = key,
                        Url = HttpUtility.HtmlEncode(
                            String.Format("https://{0}.{1}/{2}", _s3_bucket_name, _s3_bucket_region, key))
                    };
                }
            }
        }

        public static void DeleteUserAvatar(string key)
        {
            var s3Config = new AmazonS3Config() { ServiceURL = "http://" + _s3_bucket_region };
            using (var cli = new AmazonS3Client(
                _s3_access_key,
                _s3_secret_access_key,
                s3Config))
            {
                DeleteObjectRequest req = new DeleteObjectRequest()
                {
                    BucketName = _s3_bucket_name,
                    Key = key
                };

                //I should use async.
                // shoud I? :))

                var response = cli.DeleteObject(req);
                if(response.HttpStatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception("s3: delete file failed.");
                }
            }
        }
    }
}
