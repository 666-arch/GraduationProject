using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.Dto
{
    public class ArticleToBlogcateDto
    {
        public Guid BlogCategoryId { get; set; }
        public Guid ArticleId { get; set; }
        public string CateName { get; set; }
        public int Count { get; set; }

        public string Title { get; set; }   //标题
        public string NickName { get; set; }    //作者
        public DateTime CreateTime { get; set; }    //创作时间
    }
}
