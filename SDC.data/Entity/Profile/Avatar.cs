using SDC.data.Entity.s3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDC.data.Entity.Profile
{
    public class Avatar 
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }
        public int CustomForUserId { get; set; }

        public static Avatar[] GetDefaultAvatars(SDCContext db)
        {
            return db.Avatars.Where(p => p.CustomForUserId == 0).ToArray();
        }
    }
}