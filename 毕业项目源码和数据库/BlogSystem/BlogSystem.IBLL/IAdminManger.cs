using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.Dto;
namespace BlogSystem.IBLL
{
    public interface IAdminManger
    {
        bool Login(string account, string password, out Guid adminId);    //管理员登录

        Task ChangePassword(Guid adminId,string newPassword);      //修改管理员密码

        Task<int> CreateAdmin(string account, string password);      //添加管理员

        Task<AdminDto> GetAllAdminById(Guid adminId);       //查询管理员个人基本信息

        Task<List<AdminDto>> GetAllAdminInfo();     //查询所有管理员信息

        Task RemoveAdminById(Guid adminId);     //根据管理员Id删除

        Task EditAdminById(Guid adminId, string account);       //修改管理员基本信息

        Task CreateLink(string title, string url, string describe);     //添加链接

        Task RemoveLinkById(Guid id);       //删除链接

        Task<List<LinkDto>> GetAllLink(string linkname);       //查询所有链接

        Task<LinkDto> GetAllLinkById(Guid id);  //查询单个链接

        Task EditLink(Guid id, string title, string url, string describe);      //修改链接

        Task EditUserByAdmin(Guid userid, string newpassword);  //管理员修改用户密码
        Task<UserInformation> EditUserIdByAdmin(Guid userid);

        Task<List<CommentDto>> GetAllCommentByAdmin(string nickname,string title);  //评论管理

        Task<List<CommentReportDto>> GetAllCommentReport(string nickname, string title);

        Task EditHandleReport(Guid id, bool Ishandle = true); //处理举报信息
    }
}
