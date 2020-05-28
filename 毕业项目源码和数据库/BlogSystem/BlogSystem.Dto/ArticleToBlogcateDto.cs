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
    }
}
