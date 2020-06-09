using BlogSystem.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogSystem.IBLL
{
    public interface IUserMnager
    {
        Task Register(string email,string nickname,string password); //注册
        bool Login(string email, string password, out Guid userid, out string nickname, out string imagepath);  //登录
        Task ChangePassword(string email, string newPwd, string oldPwd);   //修改密码
        Task ForgetPassword(string eamil, string newPwd);   //忘记密码
        Task ChangeUserInformation(string email, string nickname, string ImagePath, string psersonDesc);  //根据邮箱修改个人信息

        Task ChangeUserInformationById(Guid id, string email, string nickname,  string psersonDesc);  //根据id修改个人信息

        Task ChangeUserImagePathById(Guid id, string ImagePath);  //根据用户id修改图片

        Task ChangeUserFanscunt(Guid id, int focus, int fans);

        Task<Dto.UserInformation> GetByEmail(string email);  //根据email得到用户信息

        Task<Dto.UserInformation> GetById(Guid userid);  //根据Id得到用户信息

        Task CreateFans(Guid userid,Guid focususerId);  //关注

        Task<List<FansDto>> GetAllFansByuserId(Guid userid, Guid focususerId);  //获取粉丝表所有

        Task<List<FansDto>> GetAllFansByFocususerId(Guid focususerId);  //查看我粉丝数

        Task<List<FansDto>> GetAllFocusByUserid(Guid userid);  //查看我关注数

        Task<List<ArticleCollectDto>> GetAllArticleCollectByUser(Guid userid);     //我的收藏

        Task Unfollow(Guid userid, Guid focusid);  //取消关注

        Task<List<Dto.UserInformation>> GetAllUserByAdmin(string email,string nickname);        //管理员查询用户
        Task<List<Dto.UserInformation>> GetAllUserByAdmin();        //管理员查询用户

        Task<List<Dto.UserInformation>> GetAllUserlike(string email,string nickname);   //模糊查询用户
    }
}
