using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Dto
{
    public class FansDto
    {
        public Guid UserId { get; set; }
        public Guid FocusUserId { get; set; }
        public string NickName { get; set; }
        public string ImagePath { get; set; }
        public int FansCount { get; set; }
        public int FocusCount { get; set; }
    }
}
