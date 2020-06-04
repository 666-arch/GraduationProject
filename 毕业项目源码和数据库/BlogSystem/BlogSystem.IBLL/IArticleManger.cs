using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.Dto;

namespace BlogSystem.IBLL
{
    public interface IArticleManger
    {
        Task CreateArticle(string title,string content,Guid[] categoryIds,Guid UserId,bool state);  //新增文章（已分类）

        Task CreateArticle(string title, string content, Guid UserId, bool state);  //新增文章（未分类的文章）

        Task CreateCategory(string name, Guid UserId);  //添加文章类别

        Task<List<BlogCategoryDto>> GetAllCategories(Guid userId);

        Task<List<ArticleDto>> GetAllArticlesByUserId(Guid userId);  //根据用户Id找文章

        Task<List<ArticleDto>> GetAllArticlesByNickName(string nickName,bool state); //根据昵称找文章

        Task<List<ArticleToBlogcateDto>> GetAllArticlesByCategoryId(Guid blogcateId);  //根据类别找Id

        Task<List<ArticleDto>> GetAllArticlesByState(bool state);   //根据发布状态查询（默认是false）

        Task<ArticleDto> GetOneArticleById(Guid articleId);   //根据文章Id查询

        Task<List<ArticleDto>> GetAllArticle(string search); //查询所有文章

        Task<int> RemoveCategory(Guid categoryId);  //删除某一类别

        Task EditCategory(Guid categoryId, string newCategoryName); //修改类别

        Task<BlogCategoryDto> GetOneCategoryByUser(Guid categoryId);  //查询某一类别

        Task<int> RemoveArticle(Guid articleId); //根据id删除文章

        Task EditArticle(Guid articleId,string title, string content, Guid[] categoryIds);  //修改文章

        Task CreateComment(Guid userid,Guid articleId,string content); //用户添加评论文章

        Task EditComment(Guid commentid,string content);  //用户修改自己的评论

        Task<CommentDto> GetAllCommentByUser(Guid commentid); //用户查询自己的评论

        Task RemoveCommentByUserId(Guid commentid);  //用户删除评论

        Task<List<CommentDto>> GetAllCommentByArticleId(Guid articleId);  //根据文章id查询评论

        Task<List<CommentDto>> GetAllComment(); //查询所有评论

        Task RecommendArticle(Guid articleId);  //给指定的文章点赞

        Task OppositionArticle(Guid articleId); //反对文章

        Task<List<CommentDto>> GetAllArticleByUserComment(Guid userid);   //查询用户评论了哪几篇文章

        Task<List<BlogCategoryDto>> GetAllBlogcategory();

        Task<List<CommentDto>> GetAllCommentByuseranarticle(Guid userid);

        Task<List<ArticleToBlogcateDto>> GetAllArticleTocate();     //文章类别中间表

        Task<List<ArticleToBlogcateDto>> GetAllArticleTocateByUserId(Guid userid);
    }
}
