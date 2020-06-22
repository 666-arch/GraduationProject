using BlogSystem.IDAL;
using BlogSystem.Models;

namespace BlogSystem.DAL
{
    public class ReplyCommentsService:BaseService<ReplyComments>,IReplyCommentsService
    {
        public ReplyCommentsService():base(new BlogContext())
        {
            
        }
    }
}