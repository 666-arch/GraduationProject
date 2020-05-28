using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Models
{
    /// <summary>
    /// 粉丝表
    /// </summary>
    public class Fans:BaseEntity
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }  //当前的用户,根据当前用户Id可以找我关注的Id
        public User User { get; set; }

        [ForeignKey(nameof(FocusUser))]
        public Guid FocusUserId { get; set; }  //关注的用户Id
        public User FocusUser { get; set; }
    }
}
