using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogSystem.IBLL;
using BlogSystem.BLL;
using System.Threading.Tasks;
using BlogSystem.Dto;
using BlogSystem.MvcUI.Filters;
using System.Text;

namespace BlogSystem.MvcUI.Controllers
{
    [BlogAuth]
    public class ArticleController : Controller
    {
        // GET: Article
        public async Task<ActionResult> CreateArticle(Guid userid)
        {
            IArticleManger categoryManger = new ArticleManger();
            return View(await categoryManger.GetAllCategories(userid));
        }
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> CreateArticle(string title, string content,Guid userid,Guid[] categoryIds, bool state=true) //已发布
        {
            IArticleManger articleManger = new ArticleManger();
            if (categoryIds==null)  //用户可以选择对文章不进行分类
            {
                await articleManger.CreateArticle(title, content, userid, state); //调用为添加分类的博客新增
                //return RedirectToAction("UserHome", "User", new { userid = userid });
                return Json(new { status = 1, data = "发布成功" });
            }
            else
            {
                await articleManger.CreateArticle(title, content, categoryIds, userid, state);
                //return RedirectToAction("UserHome", "User", new { userid = userid });
                return Json(new { status = 1, data = "发布成功" });
            }
        }
        [HttpPost]
        [ValidateInput(false)]  //富文本防恶意字符串输入
        public async Task<ActionResult> IsCreateArticle(string title, string content, Guid userid, Guid[] categoryIds, bool state = false) //保存草稿
        {
            IArticleManger articleManger = new ArticleManger();
            if (categoryIds == null)
            {
                await articleManger.CreateArticle(title, content, userid, state); 
                return Json(new { status = 1, data = "成功保存到草稿箱" });
            }
            else
            {
                await articleManger.CreateArticle(title, content, categoryIds, userid, state);
                return Json(new { status = 1, data = "成功保存到草稿箱" }); 
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateBlogCategory(string categoryname, Guid userid)
        {
            IArticleManger categoryManger = new ArticleManger();
            await categoryManger.CreateCategory(categoryname, userid);
            return Json(new { status = 1, data = "添加成功" });
        }

        [ValidateInput(false)]
        public async Task<ActionResult> DetailArticle(Guid articleid)  //根据文章id查询文章
        {
            IArticleManger articleManger = new ArticleManger();
            var articledetail = await articleManger.GetOneArticleById(articleid);
            ViewBag.atitle = articledetail.Title;
            ViewBag.acontent = articledetail.Content;
            ViewBag.createtime = articledetail.CreateTime;
            ViewBag.auser = articledetail.NickName;
            ViewBag.astate = articledetail.State;
            //foreach (var item in articledetail.CategoryNames)   //类别名称
            //{
            //    ViewBag.acatename = item;
            //}
            ViewBag.acatename = articledetail.CategoryNames;
            foreach (var item in articledetail.CategoryIds)     //类别id
            {
                ViewBag.acateid = item;
            }
            ViewBag.agoodc = articledetail.GoodCount;   //文章点赞数
            ViewBag.abadc = articledetail.BadCount;     //文章反对数
            ViewBag.actid = articledetail.Id; //文章id
            ViewBag.uid = articledetail.UserId;
            ViewBag.uimg = articledetail.ImagePath;
            articleManger = new ArticleManger(); 
            ViewBag.comms= await articleManger.GetAllComment();
            string search = "";
            List<ArticleDto> articledata = await articleManger.GetAllArticle(search);
            ViewBag.articledata = articledata;

            List<CommentDto> coList = await articleManger.GetAllComment();  //文章热议
            ViewBag.coList = coList;

            var ajaxdata = await articleManger.GetUserRandom();
            ViewBag.dataList = ajaxdata;

            Guid userid = Guid.Parse(Session["Userid"].ToString());
            IUserMnager userMag = new UserManger();
            List<FansDto> focusListCount = await userMag.GetAllFocusByUserid(userid); //我关注的人
            ViewBag.fsList = focusListCount;

            ViewBag.artIsCollect= await articleManger.GetArticleIsCollect(userid, ViewBag.actid);   //该文是否被收藏

            string title = ViewBag.atitle;
            title=title.Substring(0, 2);

            ViewBag.ArticleTitleLike=await articleManger.GetArticleLikeByArticleTitle(title);

            ViewBag.ArtCollectList=await articleManger.GetAllArticleByCollect();

            return View(await articleManger.GetAllCommentByArticleId(articleid));//获取最新评论信息
        }

        public async Task<ActionResult> EditArticle(Guid articleid)  //修改文章(查询)
        {
            IArticleManger articleManger = new ArticleManger();
            var articledetail = await articleManger.GetOneArticleById(articleid);

            ViewBag.atitle = articledetail.Title;
            ViewBag.acontent = articledetail.Content;
            ViewBag.actid = articledetail.Id; //文章所有id
            ViewBag.uid = articledetail.UserId;
            ViewBag.cateids = articledetail.CategoryIds;//拿到文章对应的类别id
            //ViewBag.catename = articledetail.CategoryNames; //文章类别名称
            
            ViewBag.ategoryIds =await new ArticleManger().GetAllCategories(ViewBag.uid);

            Guid userid = Guid.Parse(Session["Userid"].ToString()); //获得Session
            ViewBag.article=await articleManger.GetAllArticlesByUserId(userid);
            return View(ViewBag.ategoryIds);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditArticle(Guid actid, string title, string content, Guid[] categoryIds) //修改文章(删除、添加)
        {
            IArticleManger articleManger = new ArticleManger();
            await articleManger.EditArticle(actid, title, content, categoryIds);
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<ActionResult> CreateComment(Guid articleid, string content)
        {
            Guid userid = Guid.Parse(Session["Userid"].ToString());
            IArticleManger articleManger = new ArticleManger();
            await articleManger.CreateComment(userid, articleid, content);
            return RedirectToAction("DetailArticle", new { articleid = articleid });
        }

        [HttpPost]
        public async Task<ActionResult> Recommend(Guid articleid)
        {
            IArticleManger articleManger = new ArticleManger();
            await articleManger.RecommendArticle(articleid);
            return RedirectToAction("DetailArticle", new { articleid = articleid });
        }

        [HttpPost]
        public async Task<ActionResult> Opposition(Guid articleid)
        {
            IArticleManger articleManger = new ArticleManger();
            await articleManger.OppositionArticle(articleid);
            return RedirectToAction("DetailArticle", new { articleid = articleid });
        }

        public async Task<ActionResult> RemoveCommentByUser(Guid commentid)
        {
            IArticleManger articleManger = new ArticleManger();
            await articleManger.RemoveCommentByUserId(commentid);
            return Content("<script>alert('删除成功');history.go(-1);</script>");
        }
        public async Task<ActionResult> EditCommentByuser(Guid id)  //获取个人评论信息
        {
            IArticleManger articleManger = new ArticleManger();
            var data =await articleManger.GetAllCommentByUser(id);
            ViewBag.id = data.Id;
            ViewBag.conten = data.Content;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EditCommentByuser(Guid cid, string content)  //修改个人评论信息
        {
            IArticleManger articleManger = new ArticleManger();
            await articleManger.EditComment(cid,content);
            return View();
        }

        public async Task<ActionResult> EditCategoryByuser(Guid id)  //获取个人文章类别信息
        {
            IArticleManger articleManger = new ArticleManger();
            var data =await articleManger.GetOneCategoryByUser(id);
            ViewBag.id = data.Id;
            ViewBag.categoryname = data.CategoryName;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EditCategoryByuser(Guid cateid,string categoryname) //修改个人文章类别信息
        {
            IArticleManger articleManger = new ArticleManger();
            await articleManger.EditCategory(cateid, categoryname);
            return View();
        }

        public async Task<ActionResult> RemoveCategoryByUser(Guid cateid)
        {
            IArticleManger articleManger = new ArticleManger();
            var data= await articleManger.RemoveCategory(cateid);
            if (data==1)
            {
                return Content
                    (   "<script>alert('删除成功');" +
                        "history.go(-1);" +
                        "window.location.reload();" +
                        "</script>"
                    );
            }
            else
            {
                return Content("<script>alert('此类别下还存在部分文章哦,请先移除文章对应的类别');history.go(-1);</script>");
            }
        }

        public async Task<ActionResult> RemoveArticleById(Guid articleid)
        {
            IArticleManger articleManger = new ArticleManger();
            var article= await articleManger.RemoveArticle(articleid);
            if (article==1)
            {
                return Content
                    ("<script>alert('删除成功');" +
                        "history.go(-1);" +
                        "window.location.reload();" +
                        "</script>"
                    );
            }
            else
            {
                return Content("<script>alert('该文章已被分类,请先移除对应的分类');history.go(-1);</script>");
            }
        }

        public async Task<JsonResult> InitData()    //Ajax随机获取用户数据
        {
            IArticleManger articleManger = new ArticleManger();
            var ajaxdata = await articleManger.GetUserRandom();
            return Json(ajaxdata);
        }

        [HttpPost]
        public async Task<JsonResult> CreateCollectByUser(Guid userid,Guid articleid)
        {
            IArticleManger article=new ArticleManger();
            await article.CreateArticleCollect(userid, articleid);
            return Json(new {status=true});
        }
        [HttpPost]
        public async Task<JsonResult> CancelCollectByUser(Guid userid, Guid articleid)
        {
            IArticleManger articleManger=new ArticleManger();
            await articleManger.CancelCollect(userid, articleid);
            return Json(new { status = true });
        }


    }
}