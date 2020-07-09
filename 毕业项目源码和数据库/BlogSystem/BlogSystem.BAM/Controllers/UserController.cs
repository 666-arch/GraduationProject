using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using BlogSystem.BAM.Models.UserJsonModel;
using BlogSystem.BLL;
using BlogSystem.Dto;
using BlogSystem.IBLL;
using Microsoft.Ajax.Utilities;

namespace BlogSystem.BAM.Controllers
{
    public class UserController : Controller
    {
        // GET: User

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="limit">一页总条数</param>
        /// <returns></returns>

        public async Task<JsonResult> UserIndex(int page, int limit)   //用户数据接口
        {
            IUserMnager userManger = new UserManger();
            List<UserInformation> userData = await userManger.GetAllUserByAdminLayUi(limit, page-1);
            UserJson userJson = new UserJson
            {
                code = 0,
                msg = "msg",
                count = userData.Count,  //此为数据的总数
                data = userData   //你所需要显示的数据源
            };
            return Json(userJson , JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DeleteUserByAdmin(Guid id)
        {
            IAdminManger adminManger=new AdminManger();
            await adminManger.RemoveUserByAdmin(id);
            return View();
        }
    }
}