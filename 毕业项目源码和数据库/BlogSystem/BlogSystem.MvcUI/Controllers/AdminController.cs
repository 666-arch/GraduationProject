using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BlogSystem.IBLL;
using BlogSystem.BLL;
using BlogSystem.MvcUI.Models;
using System.Web.Routing;
using System.Web.WebPages;
using PagedList;
using BlogSystem.Dto;

namespace BlogSystem.MvcUI.Controllers
{
    
    public class AdminController : Controller
    {
        // GET: Admin
        //[MyAuthorize]
        //[HttpPost]
        public async Task<ActionResult> Index(int page=1)
        {
            int pagesize = 5;
            IAdminManger adminManger = new AdminManger();
            var adminList = await adminManger.GetAllAdminInfo();
            //ViewBag.admin = adminList.ToPagedList<AdminDto>(page, pagesize);
            return View(adminList.ToPagedList<AdminDto>(page, pagesize));
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string account, string password)
        {
            IAdminManger adminManger = new AdminManger();
            Guid adminid;
            var data= adminManger.Login(account, password,out adminid);
            Session["adminId"] = adminid;
            Session["account"] = account;
            DataResult result = new DataResult();
            if (data)
            {
                var payload = new Dictionary<string, object>
                {
                    { "account",account },
                    { "password", password }
                };
                result.Token = JwtHelp.SetJwtEncode(payload);
                result.Success = true;
                result.Message = "成功";
            }
            else
            {
                result.Token = "";
                result.Success = false;
                result.Message = "生成token失败";
            }
            return Json(result);

        }
        public ActionResult CreateAdmin()
        {
            return View();
        }
        //[MyAuthorize]
        [HttpPost]
        public async Task<ActionResult> CreateAdmin(string account, string password)
        {
            IAdminManger adminManger = new AdminManger();
            var data= await adminManger.CreateAdmin(account, password);
            if (data == 1)
            {
                return Json(new { status = 1, data = "该账号已存在,请重新填写！" });
            }
            else
            {
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public async Task<ActionResult> ModifyPasswd(Guid adminId)
        {
            IAdminManger adminManger = new AdminManger();
            var data=await adminManger.GetAllAdminById(adminId);
            ViewBag.account = data.Account;
            ViewBag.pwd = data.Password;
            ViewBag.id = data.Id;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ModifyPasswd(Guid adminId,string newPassword)   //修改密码
        {
            IAdminManger adminManger = new AdminManger();
            await adminManger.ChangePassword(adminId, newPassword);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> RemoveAdminById(Guid id)    //删除
        {
            IAdminManger adminManger = new AdminManger();
            await adminManger.RemoveAdminById(id);
            return View();
        }

        public async Task<ActionResult> EditAdmin(Guid id)
        {
            IAdminManger adminManger = new AdminManger();
            var data = await adminManger.GetAllAdminById(id);
            ViewBag.account = data.Account;
            ViewBag.pwd = data.Password;
            ViewBag.id = data.Id;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EditAdmin(Guid id,string account)       //修改管理员个人信息
        {
            IAdminManger adminManger = new AdminManger();
            await adminManger.EditAdminById(id, account);
            return View();
        }
        //[MyAuthorize]
        public async Task<ActionResult> LinkIndex(int page=1,string linkname="")
        {
            int pagesize = 10;
            ViewBag.linkname = linkname;
            IAdminManger adminManger = new AdminManger();
            var links=await adminManger.GetAllLink(linkname);
            return View(links.ToPagedList<LinkDto>(page, pagesize));
        }
        public ActionResult CreateLink()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateLink(string title,string url, string desc)
        {
            IAdminManger adminManger = new AdminManger();
            await adminManger.CreateLink(title,url,desc);
            return View();
        }

        public async Task<ActionResult> EditLink(Guid id)
        {
            IAdminManger adminManger = new AdminManger();
            var data= await adminManger.GetAllLinkById(id);
            ViewBag.id = data.Id;
            ViewBag.title = data.Title;
            ViewBag.url = data.Url;
            ViewBag.desc = data.Describe;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EditLink(Guid linkId, string title, string url, string desc)        //修改链接
        {
            IAdminManger adminManger = new AdminManger();
            await adminManger.EditLink(linkId, title, url, desc);
            return View();  
        }
        [HttpPost]
        public async Task<ActionResult> RemoveLink(Guid id)
        {
            IAdminManger adminManger = new AdminManger();
            await adminManger.RemoveLinkById(id);
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> ManyRemoveLink(string idStr)        //批量删除链接
        {
            IAdminManger adminManger = new AdminManger();
            string str = idStr.Substring(0, idStr.LastIndexOf(','));
            string[] ids = str.Split(',');
            foreach (var item in ids)
            {
                Guid linkId = Guid.Parse(item);
                await adminManger.RemoveLinkById(linkId);
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> ManyRemoveAdmin(string idStr)       //批量删除部分管理员
        {
            IAdminManger adminManger = new AdminManger();
            string str = idStr.Substring(0, idStr.LastIndexOf(','));
            string[] ids = str.Split(',');
            foreach (var item in ids)
            {
                Guid aid = Guid.Parse(item);
                await adminManger.RemoveAdminById(aid);
            }
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> UserIndex(int page=1,string email = "", string nickname = "")       //查询所有用户
        {
            int pagesize = 10;
            IUserMnager userMnager = new UserManger();
            var userByadmin=await userMnager.GetAllUserByAdmin(email, nickname);
            ViewBag.email = email;
            ViewBag.nickname = nickname;
            return View(userByadmin.ToPagedList<UserInformation>(page,pagesize));
        }

        public async Task<ActionResult> ArticleIndex(int page=1,string nickname="", string title = "", bool state=true) 
        {
            int pagesize = 20;
            IArticleManger articleManger = new ArticleManger();
            var articles = await articleManger.GetAllArticlesByNickName(nickname, title, state);
            ViewBag.nickname = nickname;
            ViewBag.titles = title;
            return View(articles.ToPagedList<ArticleDto>(page,pagesize));
        }

        public async Task<ActionResult> CommentIndex(int page=1,string nickname= "",string title="",string ishandle="")
        {
            int pageSize = 10;
            ViewBag.nickname = nickname;
            ViewBag.titles = title;
            ViewBag.handle = ishandle;
            IAdminManger adminManger=new AdminManger();
            var data= await adminManger.GetAllCommentReport(nickname, title,ishandle);
            return View(data.ToPagedList<CommentReportDto>(page,pageSize));
        }

        public async Task<ActionResult> Statistical(string search="")
        {
            IArticleManger articleManger=new ArticleManger();
            IUserMnager userMnager=new UserManger();
            List<CommentDto> coList = await articleManger.GetAllComment();
            ViewBag.ccount = coList.Count();//统计评论
            List<UserInformation> usList = await userMnager.GetAllUserByAdmin();
            ViewBag.ucount = usList.Count();    //统计用户数量
            List<BlogCategoryDto> cateList = await articleManger.GetAllBlogcategory();
            ViewBag.catecount = cateList.Count();   //统计随笔数量

            List<ArticleDto> arList = await articleManger.GetAllArticle(search);
            ViewBag.arcount = arList.Count();

            return View();
        }

        public async Task<ActionResult> EditUserAdmin(Guid id)
        {
            IAdminManger adminManger = new AdminManger();
            var data=await adminManger.EditUserIdByAdmin(id);
            ViewBag.id = data.Id;
            ViewBag.pwd = data.Password;
            ViewBag.email = data.Eamil;
            ViewBag.nick = data.NickName;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> EditUserAdmin(Guid id,string password)
        {
            IAdminManger adminManger = new AdminManger();
            await adminManger.EditUserByAdmin(id, password);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> HandleReport(Guid id,bool Ishandle=true)    //处理评论举报
        {

            IAdminManger admin=new AdminManger();
            await admin.EditHandleReport(id, Ishandle);
            return View();
        }

        public async Task<ActionResult> CommentManger(int page=1,string nickname = "",string title="")
        {
            int pageSize = 20;
            ViewBag.nickname = nickname;
            ViewBag.titles = title;
            IAdminManger admin=new AdminManger();
            var data= await admin.GetAllCommentByAdmin(nickname, title);
            return View(data.ToPagedList<CommentDto>(page, pageSize));
        }
        [HttpPost]
        public async Task<ActionResult> RemoveCommentByAdmin(Guid id)
        {
            IAdminManger adminManger=new AdminManger();
            await adminManger.RemoveCommentByAdmin(id);
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> RemoveCommentReportAdmin(Guid id)       //删除评论举报记录
        {
            IAdminManger admin=new AdminManger();
            await admin.RemoveCommentReportAdmin(id);
            return View();
        }

        public async Task<ActionResult> ManyRemoveReportJilu(string idStr)  //批量删除举报记录
        {
            IAdminManger admin = new AdminManger();
            string str = idStr.Substring(0, idStr.LastIndexOf(','));
            string[] idss = str.Split(',');
            foreach (var item in idss)
            {
                Guid repid = Guid.Parse(item);
                await admin.RemoveCommentReportAdmin(repid);
            }
            return View();
        }


        public async Task<ActionResult> ManyRemoveCommentManger(string idStr)   //批量删除评论Manger
        {
            IAdminManger adminManger = new AdminManger();
            string str = idStr.Substring(0, idStr.LastIndexOf(','));
            string[] ids = str.Split(',');
            foreach (var item in ids)
            {
                Guid aid = Guid.Parse(item);
                await adminManger.RemoveCommentByAdmin(aid);
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> DeleteArticleAdmin(Guid articleid)
        {
            IArticleManger articleManger=new ArticleManger();
            await articleManger.RemoveArticle(articleid);
            return View();
        }
    }
}