using BlogSystem.DAL;
using BlogSystem.Dto;
using BlogSystem.IBLL;
using BlogSystem.IDAL;
using BlogSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
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
                await articleSvc.EditAsync(article);
                using (IArticleToCategoryService articleToCategorySvc = new ArticleToCategoryService())
                {
                    //根据文章Id先删除原来所有的类别
                    foreach (var item in articleToCategorySvc.GetAllAsync().Where(m => m.ArticleId == articleId))
                    {
                        await articleToCategorySvc.RemoveAsync(item, false);
                    }
                    await articleToCategorySvc.Saved();  //循环完后保存
                    foreach (var item in categoryIds)
                    {
                        await articleToCategorySvc.CreateAsync(new ArticleToCategory()
                        {
                            ArticleId = articleId,
                            BlogCategoryId = item
                        }, false);
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
                    var article=await articleToCategoryService.GetAllAsync().Include(m => m.Article).Where(m => m.ArticleId == acid).ToListAsync();
                    if (article.Count==0)   //说明该文章未被分类可以进行删除操作
                    {
                        await articleSvc.RemoveAsync(data);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
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
                    var cate = await articleToCategorySvc.GetAllAsync()
                        .Include(m => m.BlogCategory)
                        .Where(m => m.BlogCategoryId == data.Id).ToListAsync();
                    if (cate.Count==0)  //如果文章类别没有相关的类别就执行删除,否则提示用户先删除文章对应的类别
                    {
                        await blogCategoryService.RemoveAsync(data);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
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
                        .Where(m => m.ArticleId == data.Id).ToListAsync();

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

        public async Task<List<ArticleDto>> GetAllArticlesByNickName(string nickName,bool state)   //后台模糊查询（根据作者查询）
        {
            using (IArticleService articleService = new ArticleService())
            {
                var data=await articleService.GetAllAsync()
                    .Include(m => m.User)
                    .Where(m=>m.State==state)
                    .Where(m => string.IsNullOrEmpty(nickName) || m.User.NickName.Contains(nickName))
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
                return await articleSvc.GetAllAsync()
                    .Include(m => m.User)
                    .Where(m => m.UserId == userId)
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
                        NickName=m.User.NickName
                        
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
                           UserId = m.UserId
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
                //using (IArticleService articles = new ArticleService())
                //{
                //    var data = await commentService.GetAllAsync()
                //        .GroupBy(c => new CommentDto()
                //        {
                //            ArticleId = c.ArticleId,
                //            Title = c.Article.Title
                //        },
                //        c => new ArticleDto()
                //        {
                //            Id = c.ArticleId
                //        }
                //        ).OrderByDescending(g => g.Count())
                //        .Select(
                //            g => new CommentDto()
                //            {
                //                Title = g.Key.Title,
                //                Column = g.Count()
                //            }
                //        ).ToListAsync();
                //    return data;
                //}
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
                    .Where(m=>m.IsRemoved==false)
                    .Where(m=>m.Article.State==true)    //文章已发布
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
    }
}
