using System;
using System.Collections.Generic;
using System.Web.UI;
using BlogSystem.Dto;

namespace BlogSystem.BAM.Models.UserJsonModel
{
    public class UserJson
    {
        public int code { get; set; }
        public string msg { get; set; }
        public int count { get; set; }
        public List<UserInformation> data { get; set; }
    }
}