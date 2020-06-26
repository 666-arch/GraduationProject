using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BlogSystem.IBLL;
using BlogSystem.BLL;
using BlogSystem.MvcUI.Models;
using System.IO;
using BlogSystem.Dto;
using System.Net.Mail;
using System.Text;
using System.Net;
using BlogSystem.MvcUI.Filters;

namespace BlogSystem.MvcUI.Controllers
{
    
    public class UserController : Controller
    {
        // GET: User
        [BlogAuth]
        public async Task<ActionResult> Index()
        {
            Guid userid = Guid.Parse(Session["Userid"].ToString());
            IArticleManger articleManger = new ArticleManger();
            IUserMnager userMag = new UserManger();
            List<FansDto> focusListCount = await userMag.GetAllFocusByUserid(userid); //我关注的人集合
            ViewBag.focusList = focusListCount;
            ViewBag.focusListCount = focusListCount.Count(); //关注数
            List<FansDto> fansListCount = await userMag.GetAllFansByFocususerId(userid); //关注我的集合
            ViewBag.fansList = fansListCount;
            ViewBag.fansListCount = fansListCount.Count(); //博客粉丝数
            await userMag.ChangeUserFanscunt(userid, ViewBag.focusListCount, ViewBag.fansListCount);       //用户每登录一次都会更新一次粉丝关注数
            List<CommentDto> articleByCommentUidList = await articleManger.GetAllArticleByUserComment(userid);
            if (articleByCommentUidList==null)
            {
                ViewBag.articleByCommentUidCount = 0;   //如果集合为空，则表示用户没有评论文章数量 0
            }
            else
            {
                ViewBag.articleByCommentUid = articleByCommentUidList;  //保存评论的文章集合
                ViewBag.articleByCommentUidCount = articleByCommentUidList.Count(); //保存评论文章数
            }

            List<ArticleCollectDto> articleCollectList=await userMag.GetAllArticleCollectByUser(userid);    //我的收藏
            ViewBag.artCollect = articleCollectList;
            ViewBag.artCollectCount = articleCollectList.Count();

            return View(await articleManger.GetAllArticlesByUserId(userid));
        }
        public ActionResult Login()
        {

            HttpCookie cookie = Request.Cookies.Get("LoginName");
            HttpCookie cookieuid = Request.Cookies.Get("Userid");
            if (cookieuid!=null)
            {
                var name = cookie.Value;    //获取cookie值
                var uid = cookieuid.Value;
                ViewBag.name = name;
                ViewBag.uid = uid;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password,bool remberme, LoginViewModel model)  //登录 ViewModel
        {
            model.LoginName = email;
            model.LoginPwd = password;
            IUserMnager userMag = new UserManger();
            Guid userid;
            string nickname;
            string imagepath;
            if (userMag.Login(model.LoginName, model.LoginPwd, out userid, out nickname, out imagepath))
            {
                if (remberme)   //设置Cookie
                {
                    Response.Cookies.Add(
                        new HttpCookie("LoginName")
                    {
                        Value = model.LoginName,
                        Expires = DateTime.Now.AddDays(1)   
                    });
                    Response.Cookies.Add(
                        new HttpCookie("Userid")
                        {
                            Value = userid.ToString(),
                            Expires = DateTime.Now.AddDays(1)   
                        });
                }
                else
                {
                    Session["Userid"] = userid;
                    Session["Logname"] = model.LoginName;
                }
                Session["Nickname"] = nickname;
                Session["defaultPhoto"] = imagepath;
                return Json(new { status = true, data = "登录成功" });
            }
            else
            {
                return Json(new { status=false, data = "邮箱或密码错误请重新输入" });
            }
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<string> AjaxCheckRegisterEamil(string email)    //ajax请求用户邮箱已被注册
        {
            IUserMnager userMnager = new UserManger();
            var data=await userMnager.GetByEmail(email);
            if (data!=null)
            {
                return "此邮箱已被注册请换一个";
            }
            else
            {
                return "";
            }
        }
        [HttpPost]
        public async Task<string> AjaxCheckForgetEamil(string email)    //ajax请求用户邮箱已被注册
        {
            IUserMnager userMnager = new UserManger();
            var data = await userMnager.GetByEmail(email);
            if (data == null)
            {
                return "该邮箱未被注册,无法进行更换密码的操作!";
            }
            else
            {
                return "";
            }
        }
        [HttpPost]
        public async Task<ActionResult> Register(string email, string nickname, string password,string yanzhengma)
        {
            //Session["RegisterCode"]在生成验证码的时候设置其值
            if (string.IsNullOrWhiteSpace(yanzhengma) || Session["y"].ToString() == yanzhengma)
            {
                IUserMnager userMag = new UserManger();
                await userMag.Register(email, nickname, password);
                return Json(new { success = false });
            }
            else
            {
                //其它操作...
                return Json(new { success = true });
            }
        }
        [HttpPost]
        public ContentResult Validcode(string Email) //发送邮件
        {
            try
            {
                Session["y"] = randomnumber(4);//随机数存入session
                MailMessage _msg = new MailMessage("1437855583@qq.com", Email, "终于来啦!欢迎注册学智播客", "任何相关工作人员不会向您询问密码,请妥善保管!您的验证码是" + Session["y"]);//由第一个邮箱向第二个邮箱发送邮件
                SmtpClient _client = new SmtpClient("smtp.qq.com", 587);//qq邮箱的服务器和端口 
                _client.DeliveryMethod = SmtpDeliveryMethod.Network;//通过网络发送
                _client.Credentials = new NetworkCredential("1437855583@qq.com", "fapovvwphlxzibih");//发件人的邮箱和密码 SMTP码
                _client.Send(_msg);
                return Content("发送成功");
            }
            catch
            {
                return Content("发送失败");
            }
        }
        public static string randomnumber(int length)   //随机验证码
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            return result.ToString();
        }
        [BlogAuth]
        [HttpGet]
        public async Task<ActionResult> BasicSetting(Guid userid)  //显示个人信息
        {
            IUserMnager userMag = new UserManger();
            var user = await userMag.GetById(userid);
            ViewBag.eamil = user.Eamil;
            ViewBag.nickname = user.NickName;
            ViewBag.pwd = user.Password;
            ViewBag.perdesc = user.PersonalDescription;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> BasicSetting(Guid userid, string email, string nickname, string psersonDesc) //修改个人信息
        {
            IUserMnager userMag = new UserManger(); 
            await userMag.ChangeUserInformationById(userid, email, nickname, psersonDesc);

            return Json(new { data = "修改成功!" });
        }
        [BlogAuth]
        [HttpPost]
        public async Task<ActionResult> SetPassword(string newPassword, string pwd) //修改密码
        {
            string emails = Session["Logname"].ToString();
            IUserMnager userMag = new UserManger();
            await userMag.ChangePassword(emails, newPassword, pwd);
            Session.Abandon();  //修改密码重新登录之前清除Session
            return RedirectToAction("Login");
        }

        public ActionResult DestructionLog()  //销毁Session, 退出登录
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
        [BlogAuth]
        [HttpPost]
        public async Task<ActionResult> Upload(string imagepath) //上传图片
        {
            Guid userid = Guid.Parse(Session["Userid"].ToString());
            HttpPostedFileBase imgFile = Request.Files["file"];
            //处理图片
            if (imgFile != null)
            {
                //拿到图片的名称
                string fileName = Path.GetFileName(imgFile.FileName);
                //判断文件是否是图片
                string hz = Path.GetExtension(fileName);
                if (fileName.EndsWith("jpg") || fileName.EndsWith("png") || fileName.EndsWith("jpeg"))
                {
                    imgFile.SaveAs(Server.MapPath("~/FrontPage/images/" + fileName));
                    imagepath = fileName;
                    IUserMnager userMnager = new UserManger();
                    await userMnager.ChangeUserImagePathById(userid, imagepath);
                }
                return Json(new {data= fileName });
            }
            else
            {
                return View();
            }
        }

        [BlogAuth]
        public async Task<ActionResult> UserHome(Guid userid)  //显示个人主页信息
        {
            IUserMnager userMag = new UserManger();
            var user = await userMag.GetById(userid);
            ViewBag.nickname = user.NickName;
            ViewBag.perdesc = user.PersonalDescription;
            ViewBag.createtimes = user.CreateTime;
            ViewBag.createtime = user.CreateTime.Day;
            ViewBag.uid = user.Id;
            ViewBag.img = user.ImagePath;
            IArticleManger articleManger = new ArticleManger();
            Guid userids = Guid.Parse(Session["Userid"].ToString());
            List<FansDto> fansList = await userMag.GetAllFansByuserId(userids, userid);  //查看当前用户是否存在关注记录
            ViewBag.fansCount = fansList;
            ViewBag.fCount = fansList.Count();  //判断关注的集合是否为空,如果为空说明登录的人没有关注记录
            List<FansDto> fansListCount = await userMag.GetAllFansByFocususerId(userid);
            ViewBag.fansListCount = fansListCount.Count(); //粉丝数
            List<FansDto> focusListCount =await userMag.GetAllFocusByUserid(userid);
            ViewBag.focusListCount = focusListCount.Count(); //关注数
            List<CommentDto> commList = await articleManger.GetAllComment();
            ViewBag.commlist = commList;

            ViewBag.acatelist=await articleManger.GetAllBlogcategory();

            ViewBag.articleByUid=await articleManger.GetAllArticleTocateByUserId(ViewBag.uid);

            return View(await articleManger.GetAllArticlesByUserId(userid));
        }

        [HttpPost]
        public async Task<ActionResult> CreateAttention(Guid userid, Guid focusid)
        {
            IUserMnager userMnager = new UserManger();
            await userMnager.CreateFans(userid, focusid);
            return Json(new {status=true, data = "关注成功" });
        }

        [HttpPost]
        public async Task<ActionResult> Unfollow(Guid focusid)  //取消关注
        {
            IUserMnager userMnager = new UserManger();
            Guid userid = Guid.Parse(Session["Userid"].ToString());
            await userMnager.Unfollow(userid, focusid);
            return Json(new { data = "您已取消关注" });
        }
        public ActionResult Forgetpwd()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Forgetpwd(string email, string newpwd, string yanzhengma)       //重置密码
        {
            if (string.IsNullOrWhiteSpace(yanzhengma) || Session["y"].ToString() == yanzhengma)
            {
                IUserMnager userMnager = new UserManger();
                await userMnager.ForgetPassword(email, newpwd);
                return Json(new { success = false });
            }
            else
            {
                return Json(new { success = true });
            }
        }

        public ActionResult ArticleGpBytime(DateTime atime,Guid userid)
        {
            return View();   
        }

        public async Task<ActionResult> ArticleByUserHomeCateId(Guid cateid)
        {
            IArticleManger articleManger = new ArticleManger();
            ViewBag.list = await articleManger.GetAllArticlesByCategoryId(cateid);
            return Json(new{data= ViewBag.list},JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ArticleByUserHomeTime(DateTime times,Guid userid)
        {
            IUserMnager userMnager=new UserManger();
            ViewBag.articleTimelist= await userMnager.GetAllArticleByTime(times, userid);
            return Json(new {data = ViewBag.articleTimelist}, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ReportFeedback()     //处理举报后的反馈信息
        {
            Guid userid = Guid.Parse(Session["Userid"].ToString());
            IUserMnager userMnager=new UserManger();
            var datas= await userMnager.GetAllReportFeedBackInfo(userid);
            if (datas.Any())       //举报信息已核实，举报人登录后提供反馈机制
            {
                return Json(new {code=100, data = datas }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new {code=200}, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UserMessage()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> UserConfirmInfo()       //举报人确认反馈信息
        {
            Guid userid = Guid.Parse(Session["Userid"].ToString());
            IUserMnager user =new UserManger();
            await user.EditIsConfirmByUser(userid);
            return Json(new {code = 100});
        }

        public async Task<ActionResult> UserBeReportInfo()      //处理被举报人信息
        {
            Guid userid = Guid.Parse(Session["Userid"].ToString());
            IUserMnager userMnager=new UserManger();
            var Bedata=await userMnager.Bereported(userid);
            if (Bedata.Any())   //存在举报记录，被举报人登录后提供反馈机制
            {
                return Json(new{code=100,data= Bedata },JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new{code=200,data=""},JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> BeUserReportConfirmInfo()   //被举报人确认反馈信息
        {
            Guid userid = Guid.Parse(Session["Userid"].ToString());
            IUserMnager userMnager=new UserManger();
            await userMnager.EditeIsConfirmByBeReportUser(userid);
            return Json(new { code = 100 });
        }
    }
}