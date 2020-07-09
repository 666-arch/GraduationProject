using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Dto
{
    public class UserInformation
    {
        public Guid Id { get; set; }
        public string Eamil { get; set; }
        public string Password { get; set; }
        public string NickName { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreateTime { get; set; }
        public string PersonalDescription { get; set; }
        public int FansCount { get; set; }
        public int FocusCount { get; set; }
        public bool IsFreeze { get; set; }
    }
}
