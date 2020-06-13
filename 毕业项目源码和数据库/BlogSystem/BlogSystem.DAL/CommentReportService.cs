using BlogSystem.IDAL;
using BlogSystem.Models;

namespace BlogSystem.DAL
{
    public class CommentReportService:BaseService<Models.CommentReport>,ICommentReportService
    {
        public CommentReportService():base(new BlogContext())
        {
            
        }
    }
}