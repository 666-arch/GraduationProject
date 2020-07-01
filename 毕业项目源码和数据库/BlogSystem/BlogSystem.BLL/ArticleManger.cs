using BlogSystem.DAL;
using BlogSystem.Dto;
using BlogSystem.IBLL;
using BlogSystem.IDAL;
using BlogSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.BLL
{
    public class ArticleManger : IArticleManger
    {
        public async Task CreateArticle(string title, string content, Guid[] categoryIds, Guid UserId, bool state)  //添加用户选择了分类的博客
        {
            using (var articleSvc = new ArticleService())
            {
                var article = new Article()
                {
                    Title = title,
                    Content = content,
                    State = state,
                    UserId = UserId
                };
                await articleSvc.CreateAsync(article);
                Guid articleId = article.Id;//拿到新增的文章id
                using (var articleToCategory = new ArticleToCategoryService())
                {
                    foreach (var item in categoryIds)
                    {
                        await articleToCategory.CreateAsync(new ArticleToCategory()
                        {
                            BlogCategoryId = item,
                            ArticleId = articleId

                        }, false);  //由于这里可能不止有一个类别id所有等全部添加完一起保存
                    }
                    await articleToCategory.Saved(); //循环完再保存
                }
            }
        }

        public async Task CreateArticle(string title, string content, Guid UserId, bool state)  //添加未分类的博客
        {
            using (var articleSvc = new ArticleService())
            {
                var article = new Article()
                {
                    Title = title,
                    Content = content,
                    State = state,
                    UserId = UserId
                };
                await articleSvc.CreateAsync(article);
            }
        }
        public async Task CreateCategory(string name, Guid UserId)  //用户新增类别标签
        {
            using (var categorySvc = new BlogCategoryService())
            {
                await categorySvc.CreateAsync(new BlogCategory()
                {
                    CategoryName = name,
                    UserId = UserId
                });
            }
        }
        public async Task EditArticle(Guid articleId, string title, string content, Guid[] categoryIds)  //修改文章
        {
            using (IArticleService articleSvc = new ArticleService())
            {
                var article = await articleSvc.GetOneByIdAsync(articleId);
                article.Title = title;
                article.Content = content;
                article.State = true;  //保存修改发布状态改为true
                article.CreateTime = DateTime.Now;     //将发布时间设置为当前时间

                await articleSvc.EditAsync(article);
                using (IArticleToCategoryService articleToCategorySvc = new ArticleToCategoryService())
                {
                    //根据文章Id先删除原来所有的类别
                    foreach (var item in articleToCategorySvc.GetAllAsync().Where(m => m.ArticleId == articleId))
                    {
                        await articleToCategorySvc.RemoveAsync(item, false);
                    }
                    await articleToCategorySvc.Saved();  //循环完后保存
                    if (categoryIds!=null)
                    {
                        foreach (var item in categoryIds)
                        {
                            await articleToCategorySvc.CreateAsync(new ArticleToCategory()
                            {
                                ArticleId = articleId,
                                BlogCategoryId = item
                            }, false);
                        }
                    }
                    await articleToCategorySvc.Saved();  //循环完后保存
                }
            }
        }
        public async Task EditCategory(Guid categoryId, string newCategoryName)  //修改类别
        {
            using (IBlogCategoryService categorySvc = new BlogCategoryService())
            {
                var cate = await categorySvc.GetAllAsync().FirstAsync(m => m.Id == categoryId);
                cate.CategoryName = newCategoryName;
                await categorySvc.EditAsync(cate);
            }
        }
        public async Task<List<BlogCategoryDto>> GetAllCategories(Guid userId)  //根据用户查询文章类型
        {
            using (IBlogCategoryService categorySvc = new BlogCategoryService())
            {
                return await categorySvc.GetAllAsync().Where(m => m.UserId == userId).Where(m => m.IsRemoved == false).Select(m => new BlogCategoryDto()
                {
                    Id = m.Id,
                   
                    CategoryName = m.CategoryName,
                    CreateTime = m.CreateTime
                }).ToListAsync();
            }
        }
        public async Task<List<ArticleDto>> GetAllArticlesByState(bool state)  //根据发布状态查询文章
        {
            using (IArticleService articleSvc = new ArticleService())
            {
                return await articleSvc.GetAllAsync().Where(m => m.State == state).Select(m => new ArticleDto()
                {
                    Title = m.Title,
                    Content = m.Content,
                    CreateTime = m.CreateTime,
                    State = m.State,
                    GoodCount = m.GoodCounnt,
                    BadCount = m.BadCount
                }).ToListAsync();
            }
        }
        public async Task<int> RemoveArticle(Guid articleId)   //根据文章Id删除
        {
            using (IArticleService articleSvc = new ArticleService())
            {
                var data=await articleSvc.GetAllAsync().FirstOrDefaultAsync(m => m.Id == articleId);
                var acid = data.Id;
                using (IArticleToCategoryService articleToCategoryService = new ArticleToCategoryService())
                {
                    //var article=await articleToCategoryService.GetAllAsync().Include(m => m.Article).Where(m => m.ArticleId == acid).ToListAsync();
                    //if (article.Count==0)   //说明该文章未被分类可以进行删除操作
                    //{
                    //    await articleSvc.RemoveAsync(data);
                    //    return 1;
                    //}
                    //else
                    //{
                    //    return 0;
                    //}

                    await articleSvc.RemoveAsync(data);
                    return 1;
                }
            }
        }
        public async Task<int> RemoveCategory(Guid categoryId)  //用户删除类别
        {
            using (IBlogCategoryService blogCategoryService = new BlogCategoryService())
            {
                var data =await blogCategoryService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == categoryId);
                using (IArticleToCategoryService articleToCategorySvc = new ArticleToCategoryService())
                {
                    //var cate = await articleToCategorySvc.GetAllAsync()
                    //    .Include(m => m.BlogCategory)
                    //    .Where(m => m.BlogCategoryId == data.Id).ToListAsync();
                    //if (cate.Count==0)  //如果文章类别没有相关的类别就执行删除,否则提示用户先删除文章对应的类别
                    //{
                    //    await blogCategoryService.RemoveAsync(data);
                    //    return 1;
                    //}
                    //else
                    //{
                    //    return 0;
                    //}
                    await blogCategoryService.RemoveAsync(data);
                    return 1;
                }
            }
        }

        public async Task<ArticleDto> GetOneArticleById(Guid articleId)  //根据文章Id查询
        {
            using (IArticleService articleSvc = new ArticleService())
            {
                var data = await articleSvc.GetAllAsync()
                   .Include(m => m.User)
                   .Where(m => m.Id == articleId)
                   .Select(m => new ArticleDto()
                   {
                       Id = m.Id,
                       Title = m.Title,
                       Content = m.Content,
                       CreateTime = m.CreateTime,
                       NickName = m.User.NickName,
                       GoodCount = m.GoodCounnt,
                       BadCount = m.BadCount,
                       UserId = m.UserId,
                       ImagePath=m.User.ImagePath,
                       State=m.State
                   }).FirstAsync();

                using (IArticleToCategoryService articleToCategorySvc = new ArticleToCategoryService())
                {
                    var cate = await articleToCategorySvc.GetAllAsync()
                        .Include(m => m.BlogCategory)
                        .Where(m => m.ArticleId == data.Id)
                        .Where(m=>m.BlogCategory.IsRemoved==false)
                        .ToListAsync();

                    data.CategoryIds = cate.Select(m => m.BlogCategoryId).ToArray();    //转化为数组Id
                    data.CategoryNames = cate.Select(m => m.BlogCategory.CategoryName).ToArray();
                    return data;
                }
            }
        }
        public async Task<List<ArticleToBlogcateDto>> GetAllArticlesByCategoryId(Guid blogcateId)
        {
            using (IArticleToCategoryService articleToCategory = new ArticleToCategoryService())
            {
                var data = await articleToCategory.GetAllAsync()
                    .Include(m => m.Article)
                    .Include(m => m.Article.User)
                    .Where(m => m.BlogCategoryId == blogcateId)
                    .Select(m => new ArticleToBlogcateDto()
                    {
                        ArticleId=m.ArticleId,
                        Title = m.Article.Title,
                        NickName = m.Article.User.NickName,
                        CateName = m.BlogCategory.CategoryName,
                        CreateTime = m.Article.CreateTime
                    }).ToListAsync();
                return data;
            }
            //using (IBlogCategoryService blogCategory = new BlogCategoryService())
            //{
            //    var data = await blogCategory.GetAllAsync().Where(m => m.UserId == userid).ToListAsync();
            //    foreach (var item in data)
            //    {
            //        var bid = item.Id;
            //        using (IArticleToCategoryService articleToCategory = new ArticleToCategoryService())
            //        {
            //            return await articleToCategory.GetAllAsync().Where(m => m.BlogCategoryId == bid).Include(m => m.BlogCategory)
            //                .Select(m => new BlogCategoryDto()
            //            {
            //                CategoryName=m.BlogCategory.CategoryName
            //            }).ToListAsync();
            //        }
            //    }
            //}
        }


        public async Task<List<ArticleDto>> GetAllTestData()
        {
            throw new NotImplementedException();
        }

        public async Task<List<ArticleDto>> GetAllArticlesByNickName(string nickName, string title, bool state)   //后台模糊查询（根据作者查询）
        {
            using (IArticleService articleService = new ArticleService())
            {
                var data=await articleService.GetAllAsync()
                    .Include(m => m.User)
                    .Where(m=>m.State==state)
                    .Where(m => string.IsNullOrEmpty(nickName)&string.IsNullOrEmpty(title) || m.User.NickName.Contains(nickName)&m.Title.Contains(title))
                    .OrderByDescending(m=>m.CreateTime)
                    .Select(m => new ArticleDto()
                    {
                        Id=m.Id,
                        Title = m.Title,
                        NickName = m.User.NickName,
                        State = m.State,
                        CreateTime = m.CreateTime,
                    }).ToListAsync();
                return data;
            }
        }
        public async Task<List<ArticleDto>> GetAllArticlesByUserId(Guid userId)
        {
            using (IArticleService articleSvc = new ArticleService())
            {
                var data= await articleSvc.GetAllAsync()
                    .Include(m => m.User)
                    .Where(m => m.UserId == userId)
                    
                    .Where(m=>m.IsRemoved==false)
                    .Select(m => new ArticleDto()
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Content = m.Content,
                        GoodCount = m.GoodCounnt,
                        BadCount = m.BadCount,
                        CreateTime = m.CreateTime,
                        State = m.State
                    }).ToListAsync();
                return data;
                //using (IArticleToCategoryService articleToCategorySvc = new ArticleToCategoryService())
                //{
                //    var cate = await articleToCategorySvc.GetAllAsync()
                //        .Include(m => m.BlogCategory)
                //        .Where(m => m.ArticleId == data.Id).ToListAsync();

                //    data.CategoryIds = cate.Select(m => m.BlogCategoryId).ToArray();
                //    data.CategoryNames = cate.Select(m => m.BlogCategory.CategoryName).ToArray();
                //    return data;
                //}
            }
        }

        public async Task<List<ArticleDto>> GetAllArticle(string search) //查询所有文章显示在主页(用户也可模糊查询)
        {
            using (IArticleService articleSvc = new ArticleService())
            {
                return await articleSvc.GetAllAsync().Include(m => m.User)
                    .Where(m => m.State == true)
                    .Where(m=>string.IsNullOrEmpty(search)||m.Title.Contains(search)||m.User.NickName.Contains(search))
                    .Select(m => new ArticleDto()
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Content = m.Content,
                        CreateTime = m.CreateTime,
                        GoodCount = m.GoodCounnt,
                        BadCount = m.BadCount,
                        ImagePath = m.User.ImagePath,
                        State = true,
                        NickName=m.User.NickName,
                        UserId = m.UserId
                        
                    }).ToListAsync();
            }
        }

        public async Task CreateComment(Guid userid, Guid articleId, string content)  //用户评论文章
        {
            using (ICommentService commentSvc = new CommentService())
            {
                await commentSvc.CreateAsync(new Comment()
                {
                    UserId = userid,
                    ArticleId = articleId,
                    Content = content
                });
            }
        }

        public async Task<List<CommentDto>> GetAllCommentByArticleId(Guid articleId)  //根据文章id查询对应的评论信息
        {
            using (ICommentService commentSvc = new CommentService())
            {
                return await commentSvc.GetAllAsync()
                       .Include(m => m.User)
                       .Where(m => m.ArticleId == articleId)
                       .Where(m => m.IsRemoved == false)
                       .Select(m => new CommentDto()
                       {
                           Id = m.Id,
                           Content = m.Content,
                           CreateTime = m.CreateTime,
                           NickName = m.User.NickName,
                           UserId = m.UserId,
                           ImagePath=m.User.ImagePath
                       }).ToListAsync();
            }
        }

        public async Task<List<CommentDto>> GetAllComment()     //查询所有评论信息
        {
            using (ICommentService commentService = new CommentService())
            {
                var data = await commentService.GetAllAsync()
                    .Where(m=>m.IsRemoved==false&m.Article.IsRemoved==false)
                    .Include(m => m.Article)
                    .Include(m=>m.User)
                    .Select(m => new CommentDto()
                    {
                        Title=m.Article.Title,
                        NickName=m.User.NickName,
                        ArticleId=m.ArticleId,
                        CreateTime=m.CreateTime,
                        UserId=m.UserId,
                        Content=m.Content
                     
                    }).ToListAsync();
                return data;
            }
        }

        public async Task<int> CreateCommentReport(Guid userid, Guid commentid, string content)  //举报评论
        {
            using (ICommentReportService creSvc = new CommentReportService())
            {
                var data=await creSvc.GetAllAsync()
                    .FirstOrDefaultAsync(m => m.UserId == userid & m.CommentId == commentid);
                if (data==null) //如果没记录可以举报
                {
                    await creSvc.CreateAsync(new CommentReport()
                    {
                        UserId = userid,
                        CommentId = commentid,
                        Content = content   //举报原因
                    });
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }

        public async Task RecommendArticle(Guid articleId) //点赞相当于修改
        {
            using (IArticleService articleService = new ArticleService())
            {
                var article = await articleService.GetOneByIdAsync(articleId);
                article.GoodCounnt++;
                await articleService.EditAsync(article); //修改
            }
        }

        public async Task OppositionArticle(Guid articleId)  //反对文章
        {
            using (IArticleService articleService = new ArticleService())
            {
                var article = await articleService.GetOneByIdAsync(articleId);
                article.BadCount++;
                await articleService.EditAsync(article); //修改
            }
        }

        //public async Task<List<ArticleDto>> GetAllArticleByUserComment(Guid userid) //查询用户评论的文章
        //{
        //    using (ICommentService commentService = new CommentService())
        //    {
        //        var data = await commentService.GetAllAsync().Where(m => m.UserId == userid).ToListAsync();

        //        using (IArticleService articleService = new ArticleService())
        //        {
        //            if (data != null)
        //            {
        //                return await articleService.GetAllAsync().Where(m => m.Id == articleid).Where(m => m.IsRemoved == false)
        //                .Select(m => new ArticleDto()
        //                {
        //                    Id = m.Id,
        //                    Title = m.Title,
        //                    GoodCount = m.GoodCounnt,
        //                    BadCount = m.BadCount,
        //                    CreateTime = m.CreateTime
        //                }).ToListAsync();
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //}

        public async Task<List<CommentDto>> GetAllArticleByUserComment(Guid userid) //查询用户评论的文章
        {
            using (ICommentService comment = new CommentService())
            {
                var data=await comment.GetAllAsync()
                    .Where(m => m.UserId == userid)
                    .OrderByDescending(m=>m.CreateTime) //降序排列
                    .Include(m => m.Article)
                    .Select(m => new CommentDto()
                {
                    ArticleId = m.ArticleId,
                    Title = m.Article.Title,
                    CreateTime = m.CreateTime
                }).ToListAsync();
                return data;
            }
        }


        public async Task RemoveCommentByUserId(Guid commentid)   //用户删除评论
        {
            using (ICommentService commentService = new CommentService())
            {
                var data = await commentService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == commentid);
                await commentService.RemoveAsync(data);
            }
        }

        public async Task EditComment(Guid commentid, string content)  //用户修改评论
        {
            using (ICommentService commentService = new CommentService())
            {
                var data=await commentService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == commentid);
                data.Content = content;
                await commentService.EditAsync(data);
            }
        }

        public async Task<CommentDto> GetAllCommentByUser(Guid commentid)  //用户查询自己的评论
        {
            using (ICommentService commentService = new CommentService())
            {
                return await commentService.GetAllAsync().Where(m => m.Id == commentid).Select(m => new CommentDto()
                {
                    Content = m.Content,
                    Id = m.Id,
                    ArticleId=m.ArticleId
                }).FirstOrDefaultAsync();
            }
        }

        public async Task<BlogCategoryDto> GetOneCategoryByUser(Guid categoryId)  //查询某一类别
        {
            using (IBlogCategoryService blogCategoryService = new BlogCategoryService())
            {
                return await blogCategoryService.GetAllAsync().Where(m => m.Id == categoryId).Select(m => new BlogCategoryDto()
                {
                    Id = m.Id,
                    CategoryName = m.CategoryName
                }).FirstOrDefaultAsync();
            }
        }

        public async Task<List<BlogCategoryDto>> GetAllBlogcategory()   //统计随笔
        {
            using (IBlogCategoryService blogCategoryService = new BlogCategoryService())
            {
                return await blogCategoryService.GetAllAsync().Where(m=>m.IsRemoved==false).Select(m => new BlogCategoryDto()
                {
                    Id = m.Id,
                    CategoryName = m.CategoryName
                }).ToListAsync();
            }
        }

        public async Task<List<CommentDto>> GetAllCommentByuseranarticle(Guid userid)   //最新评论
        {
            using (ICommentService comment = new CommentService())
            {
                var data = await comment.GetAllAsync()
                    .Where(m => m.UserId == userid)
                    .OrderByDescending(m => m.CreateTime) //降序排列
                    .Include(m => m.Article)
                    .Select(m => new CommentDto()
                    {
                        ArticleId = m.ArticleId,
                        Title = m.Article.Title,
                        Content=m.Content,
                        CreateTime = m.CreateTime
                    }).ToListAsync();
                return data;
            }
            //using (IArticleService articleService=new ArticleService())
            //{
            //    var data = await articleService.GetAllAsync().FirstOrDefaultAsync(m=>m.UserId== userid);
            //    var articleid = data.Id;
            //    using (ICommentService comment = new CommentService())
            //    {
            //         return await comment
            //            .GetAllAsync()
            //            .Where(m => m.ArticleId == articleid)
            //            .OrderByDescending(m=>m.CreateTime)
            //            .Include(m => m.Article)
            //            .Include(m=>m.User)
            //            .Select(m => new CommentDto()
            //        {
            //            Title=m.Article.Title,
            //            Content = m.Content,
            //            Id=m.ArticleId,
            //            NickName=m.User.NickName,
            //            CreateTime=m.CreateTime
            //        }).ToListAsync();
            //    }
            //}
        }

        public async Task<List<ArticleToBlogcateDto>> GetAllArticleTocate()
        {
            using (IArticleToCategoryService articleToCategory = new ArticleToCategoryService())
            {
                return await articleToCategory.GetAllAsync()
                    .Where(m => m.IsRemoved == false)
                    .Where(m => m.Article.State == true)    //已发布的文章
                    .Include(m => m.BlogCategory)
                    .Select(m => new ArticleToBlogcateDto()
                    {
                        CateName = m.BlogCategory.CategoryName,
                        BlogCategoryId = m.BlogCategoryId,
                        ArticleId = m.ArticleId
                    }).ToListAsync();
            }
        }

        public async Task<List<ArticleToBlogcateDto>> GetAllArticleTocateByUserId(Guid userid)  // 根据用户找到对应的类别下的文章并且统计数量
        {
            using (IArticleToCategoryService articleToCategory = new ArticleToCategoryService())
            {
            
                    return await articleToCategory.GetAllAsync()
                          .Include(m => m.BlogCategory)
                          .Include(m => m.Article)
                          .Where(m => m.IsRemoved == false)
                          .Where(m => m.Article.State == true)  //文章已发布
                          .Where(m => m.Article.UserId == userid)
                          .Select(m => new ArticleToBlogcateDto()
                          {
                              BlogCategoryId = m.BlogCategoryId,
                              CateName = m.BlogCategory.CategoryName,
                              ArticleId = m.ArticleId
                          }).ToListAsync();
         
            }
        }

        public async Task<List<UserInformation>> GetUserRandom()    //随机获取数据
        {
            using (IUserService user = new UserService())
            {
                var data= await user.GetAllAsync()
                    .OrderBy(m=>Guid.NewGuid())
                    .Take(5)
                    .Select(m => new UserInformation()
                    {
                        NickName = m.NickName,
                        Id=m.Id,
                        FansCount=m.FansCount,
                        ImagePath=m.ImagePath
                    }).ToListAsync();
                return data;
            }
        }

        public async Task CreateArticleCollect(Guid userId, Guid articleId)  //用户添加收藏
        {
            using (IArticleCollectService artSvc = new ArticleCollectService())
            {
                await artSvc.CreateAsync(new ArticleCollect()
                {
                    UserId = userId,
                    ArticleId = articleId
                });
            }
        }

        public async Task<ArticleCollectDto> GetArticleIsCollect(Guid userId, Guid articleId)
        {
            using (IArticleCollectService artSvc = new ArticleCollectService())
            {
                try
                {
                    var data = await artSvc.GetAllAsync()
                        .Where(m => m.UserId == userId)
                        .Where(m => m.ArticleId == articleId)
                        .Select(m => new ArticleCollectDto()
                        {
                            ArticleId = m.ArticleId
                        }).FirstAsync();
                    return data;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public async Task CancelCollect(Guid userId, Guid articleId)
        {
            using (IArticleCollectService articleCollect = new ArticleCollectService())
            {
                var data = await articleCollect.GetAllAsync()
                    .FirstOrDefaultAsync(m => m.UserId == userId && m.ArticleId == articleId);
                await articleCollect.RemoveAsync(data,save:true);
            }
        }

        public async Task<List<ArticleDto>> GetArticleLikeByArticleTitle(string title)
        {
            using (IArticleService articleService = new ArticleService())
            {
                var data = await articleService.GetAllAsync()
                    .Where(m => string.IsNullOrEmpty(title) || m.Title.Contains(title))
                    .OrderBy(m=>Guid.NewGuid())
                    .Take(5)
                    .Select(m => new ArticleDto()
                    {
                        Title = m.Title,
                        Id = m.Id
                    }).ToListAsync();
                return data;
            }
        }

        public async Task<List<ArticleCollectDto>> GetAllArticleByCollect() //文章收藏排序
        {
            using (IArticleCollectService articleCollect = new ArticleCollectService())
            {
                return await articleCollect.GetAllAsync()
                    .Include(m => m.Article)
                    .Select(m => new ArticleCollectDto()
                    {
                        Title = m.Article.Title,
                        ArticleId = m.ArticleId
                    }).ToListAsync();
            }
        }

        public async Task<CommentReportDto> GetAllReportsByUser(Guid userid, Guid commentid)        //判断是否存在举报记录
        {
            using (ICommentReportService comReSvc = new CommentReportService())
            {
                 var data = await comReSvc.GetAllAsync()
                     .Where(m => m.UserId == userid & m.CommentId == commentid)
                     .Select(m=>new CommentReportDto()
                     {
                         UserId = m.UserId,
                         CommentId = m.CommentId
                     })
                     .FirstAsync();
                 return data;
            }
        }

        public async Task CreateReplyToComment(Guid replierId, Guid targetToReplyId, string content, int replyType, Guid commentParentId, Guid replyToTargetCommentId)   //回复人、被回复人、回复内容、回复类型
        {
            using (IReplyCommentsService reply = new ReplyCommentsService())
            {
                await reply.CreateAsync(new ReplyComments()
                {
                    ReplierId = replierId,
                    TargetToReplyId = targetToReplyId,
                    ReplyContent = content,
                    ReplyType = replyType,
                    CommentParentId = commentParentId,
                    ReplyToTargetCommentId = replyToTargetCommentId
                });
            }
        }
        public async Task<List<ReplyCommentsDto>> GetAllReplyCommentsInfo(Guid commentParentId)
        {
            using (IReplyCommentsService reply = new ReplyCommentsService())
            {
                var data= await reply.GetAllAsync()
                    .Include(m => m.Comment.User)
                    .Where(m =>m.Comment.Article.UserId==commentParentId)
                    .OrderByDescending(m=>m.CreateTime)
                    .Select(m => new ReplyCommentsDto()
                    {
                        NickName = m.ReplierUser.NickName,  //回复人
                        ByReplyNickName = m.TargetToReplyUser.NickName,     //被回复人
                        ReplyContent = m.ReplyContent,   //回复内容
                        ImagePath = m.ReplierUser.ImagePath,    
                        ReplierId=m.ReplierId,  //回复用户Id
                        TargetToReplyId =m.TargetToReplyId, //回复目标用户Id
                        ReplyToTargetCommentId =m.ReplyToTargetCommentId,    //回复目标评论Id
                        Id=m.Id,
                        CommentParentId=m.CommentParentId,
                        CreateTime =m.CreateTime
                    }).ToListAsync();
                return data;
            }
        }

        public async Task RemoveReturnComment(Guid id)  
        {
            using (IReplyCommentsService replyComments = new ReplyCommentsService())
            {
                var data=await replyComments.GetAllAsync().Where(m => m.Id == id).FirstAsync();
                await replyComments.RemoveAsync(data);
            }
        }
    }
}
