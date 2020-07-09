using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogSystem.IBLL;
using BlogSystem.BLL;
using System.Threading.Tasks;
using BlogSystem.MvcUI.Models;
using BlogSystem.Dto;
using PagedList;
using BlogSystem.MvcUI.Filters;

namespace BlogSystem.MvcUI.Controllers
{
    
    public class HomeController : Controller
    {
        // GET: Home
        //[MyAuthorize]
        [BlogAuth]
        public async Task<ActionResult> Index(int page=1,string search = "")
        {
            int pagesize =36;
            IArticleManger articleManger = new ArticleManger();
            IUserMnager userMnager = new UserManger();
            List<UserInformation> usList=await userMnager.GetAllUserByAdmin();
            ViewBag.alluser = usList;   //所有博主
            ViewBag.ucount = usList.Count();    //统计博客数量
            List<CommentDto> coList =await articleManger.GetAllComment();
            ViewBag.coList = coList;
            ViewBag.ccount = coList.Count();    //统计评论数量
            List<BlogCategoryDto> cateList =await articleManger.GetAllBlogcategory();
            ViewBag.catecount= cateList.Count();
            ViewBag.articleTocateList = await articleManger.GetAllArticleTocate();
            IAdminManger adminManger = new AdminManger();
            string linkname = "";
            string desc = "";
            ViewBag.links=await adminManger.GetAllLink(linkname, desc);
            var data = await articleManger.GetAllArticle(search);
            ViewBag.catelist= await articleManger.GetAllBlogcategory();

            return View(data.ToPagedList<ArticleDto>(page,pagesize));
        }

        public async Task<ActionResult> ArticleByIndexCateId(Guid blogcateid)
        {
            IArticleManger articleManger = new ArticleManger();
            ViewBag.list=await articleManger.GetAllArticlesByCategoryId(blogcateid);
            return View();
        }

    }
}