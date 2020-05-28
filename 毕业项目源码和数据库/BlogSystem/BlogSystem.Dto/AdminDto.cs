using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Dto
{
    public class AdminDto
    {
        public Guid Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
