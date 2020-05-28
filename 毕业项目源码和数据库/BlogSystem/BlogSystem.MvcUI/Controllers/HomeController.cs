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
    [BlogAuth]
    public class HomeController : Controller
    {
        // GET: Home
        //[MyAuthorize]
        public async Task<ActionResult> Index(int page=1)
        {
            int pagesize = 15;
            IArticleManger articleManger = new ArticleManger();
            IUserMnager userMnager = new UserManger();
            List<UserInformation> usList=await userMnager.GetAllUserByAdmin();
            ViewBag.alluser = usList;   //所有博主
            ViewBag.ucount = usList.Count();    //统计博客数量

            List<CommentDto> coList =await articleManger.GetAllComment();
            ViewBag.ccount = coList.Count();    //统计评论数量

            List<BlogCategoryDto> cateList =await articleManger.GetAllBlogcategory();
            ViewBag.catecount= cateList.Count();

            IAdminManger adminManger = new AdminManger();
            ViewBag.links=await adminManger.GetAllLink();


            var data = await articleManger.GetAllArticle();
            return View(data.ToPagedList<ArticleDto>(page,pagesize));
        }

    }
}