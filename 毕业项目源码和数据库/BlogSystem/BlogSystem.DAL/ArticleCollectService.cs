using BlogSystem.IDAL;
using BlogSystem.Models;

namespace BlogSystem.DAL
{
    public class ArticleCollectService:BaseService<ArticleCollect>, IArticleCollectService
    {
        public ArticleCollectService():base(new BlogContext())
        {
            
        }
    }
}