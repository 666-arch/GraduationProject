using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Models
{
    public class BlogCategory:BaseEntity
    {
        public string CategoryName { get; set; }  //分类名称
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; } //用户外键
        public User User { get; set; }
    }
}
