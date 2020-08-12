using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.IBLL;
using BlogSystem.Dto;
using BlogSystem.IDAL;
using BlogSystem.DAL;
using BlogSystem.Models;
using System.Data.Entity;

namespace BlogSystem.BLL
{

    public class AdminManger : IAdminManger
    {

        public bool Login(string account, string password,out Guid adminId)
        {
            using (IAdminService adminService = new AdminService())
            {
                var admin= adminService.GetAllAsync().FirstOrDefaultAsync(m => m.Account == account && m.Password == password);
                admin.Wait();
                var data = admin.Result;
                if (data==null)
                {
                    adminId = new Guid();
                    return false;
                }
                else
                {
                    adminId = data.Id;
                    account = data.Account;
                    password = data.Password;
                    return true;
                }
            }
        }
        public async Task ChangePassword(Guid adminId, string newPassword)
        {
            using (IAdminService adminService = new AdminService())
            {
                var data=await adminService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == adminId);
                data.Password = newPassword;
                await adminService.EditAsync(data);
            }
        }

        public async Task<int> CreateAdmin(string account, string password)
        {
            using (IAdminService adminService = new AdminService())
            {
                var data=await adminService.GetAllAsync().FirstOrDefaultAsync(m => m.Account == account);
                if (data==null)
                {
                    await adminService.CreateAsync(new Admin()
                    {
                        Account = account,
                        Password = password
                    });
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
        }

        public async Task<AdminDto> GetAllAdminById(Guid adminId)
        {
            using (IAdminService adminService = new AdminService())
            {
                return await adminService.GetAllAsync().Where(m => m.Id == adminId).Select(m => new AdminDto()
                {
                    Id = m.Id,
                    Account = m.Account,
                    Password = m.Password
                }).FirstOrDefaultAsync();
            }
        }

        public async Task<List<AdminDto>> GetAllAdminInfo(string account)
        {
            using (IAdminService adminService = new AdminService())
            {
               return await adminService.GetAllAsync()
                    .Where(m=>string.IsNullOrEmpty(account)||m.Account.Contains(account))
                    .Select(m => new AdminDto()
                {
                    Id = m.Id,
                    Account = m.Account,
                    Password = m.Password,
                    CreateTime=m.CreateTime
                }).ToListAsync();
            }
        }

        public async Task RemoveAdminById(Guid adminId)
        {
            using (IAdminService adminService = new AdminService())
            {
                var data=await adminService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == adminId);
                if (data!=null)
                {
                    await adminService.RemoveAsync(data);
                }
            }
        }


        public async Task EditAdminById(Guid adminId, string account)       //修改管理员个人信息
        {
            using (IAdminService adminService = new AdminService())
            {
                var data=await adminService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == adminId);
                if (data!=null)
                {
                    data.Account = account;
                    await adminService.EditAsync(data);
                }
            }
        }

        public async Task CreateLink(string title, string url, string describe)
        {
            using (ILinkService linkService = new LinkService())
            {
                 await linkService.CreateAsync(new Link()
                {
                    Title = title,
                    Url = url,
                    Describe = describe
                });
            }
        }

        public async Task RemoveLinkById(Guid id)       //删除单个链接
        {
            using (ILinkService linkService = new LinkService())
            {
                var data=await linkService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == id);
                if (data!=null)
                {
                    await linkService.RemoveAsync(data);
                }
            }
        }

        public async Task<List<LinkDto>> GetAllLink(string linkname,string desc)
        {
            using (ILinkService linkService = new LinkService())
            {
                return await linkService.GetAllAsync()
                    .Where(m=>string.IsNullOrEmpty(linkname)&string.IsNullOrEmpty(desc)||m.Title.Contains(linkname)&m.Describe.Contains(desc))
                    .Select(m => new LinkDto()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Url = m.Url,
                    Describe = m.Describe
                }).ToListAsync();
            }
        }

        public async Task EditLink(Guid id, string title, string url, string describe)
        {
            using (ILinkService linkService = new LinkService())
            {
                var data= await linkService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == id);
                if (data!=null)
                {
                    data.Title = title;
                    data.Url = url;
                    data.Describe = describe;
                    await linkService.EditAsync(data);
                }
            }
        }

        public async Task<LinkDto> GetAllLinkById(Guid id)
        {
            using (ILinkService linkService = new LinkService())
            {
                return await linkService.GetAllAsync().Where(m => m.Id == id).Select(m => new LinkDto()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Url = m.Url,
                    Describe = m.Describe
                }).FirstOrDefaultAsync();
            }
        }

        public async Task EditUserByAdmin(Guid userid, string newpassword)      //管理员修改用户密码
        {
            using (IUserService user = new UserService())
            {
                var data=await user.GetAllAsync().FirstOrDefaultAsync(m => m.Id == userid);
                if (data!=null)
                {
                    data.Password = newpassword;
                    await user.EditAsync(data);
                }
            }
        }

        public async Task<UserInformation> EditUserIdByAdmin(Guid userid)
        {
            using (IUserService user = new UserService())
            {
                var data = await user.GetAllAsync().Where(m => m.Id == userid)
                    .Select(m => new UserInformation()
                    {
                        Id = m.Id,
                        Eamil=m.Eamil,
                        Password = m.Password,
                        NickName=m.NickName,

                    }).FirstOrDefaultAsync();
                return data;
            }
        }

        public async Task<List<CommentDto>> GetAllCommentByAdmin(string nickname, string title,string content)      //评论管理
        {
            using (ICommentService commentService = new CommentService())
            {
                return await commentService.GetAllAsync()
                    .Include(m => m.User)
                    .Include(m => m.Article)
                    .Where(m=>string.IsNullOrEmpty(nickname)&string.IsNullOrEmpty(title) & string.IsNullOrEmpty(content) || m.User.NickName.Contains(nickname)&m.Article.Title.Contains(title)&m.Content.Contains(content))
                    .OrderByDescending(m=>m.CreateTime)
                    .Select(m => new CommentDto()
                {
                    Id=m.Id,
                    Content = m.Content,
                    Title = m.Article.Title,
                    ArticleId = m.ArticleId,
                    NickName = m.User.NickName,
                    CreateTime = m.CreateTime
                }).ToListAsync();
            }
        }

        public async Task<List<CommentReportDto>> GetAllCommentReport(string nickname, string title, string ishandle, string content)    //模糊查询
        {
            using (ICommentReportService creSvc = new CommentReportService())
            {
               var data= await creSvc.GetAllAsync()
                    .Where(m=>string.IsNullOrEmpty(nickname)&string.IsNullOrEmpty(title)&string.IsNullOrEmpty(ishandle) & string.IsNullOrEmpty(content) ||
                              m.User.NickName.Contains(nickname)&m.Comment.Article.Title.Contains(title)&m.IsHandle.ToString().Contains(ishandle)&m.Content.Contains(content))
                    .Where(m=>m.IsRemoved==false)
                    .Include(m => m.Comment)
                    .Include(m => m.User)
                    .Include(m=>m.Comment.Article)
                    .Select(m => new CommentReportDto()
                    {
                        Id=m.Id,
                        ReportUserId=m.Comment.User.Id,
                        Content = m.Content,    //举报原因
                        NickName = m.User.NickName,     //举报人
                        ReportUserNickName=m.Comment.User.NickName,  //被举报人
                        CommentContent = m.Comment.Content,      //被举报的评论
                        CreateTime=m.CreateTime,
                        Title=m.Comment.Article.Title,
                        IsHandle=m.IsHandle,     //是否已授理
                        IsFreeze=m.Comment.User.IsFreeze
                    }).ToListAsync();
               return data;
            }
        }

        public async Task EditHandleReport(Guid id,bool Ishandle=true)  //处理举报信息
        {
            using (ICommentReportService comRepSvc = new CommentReportService())
            {
                var data=await comRepSvc.GetAllAsync().FirstOrDefaultAsync(m => m.Id == id);
                if (data!=null)
                {
                    data.IsHandle = Ishandle;
                    await comRepSvc.EditAsync(data);
                }
            }
        }
        public async Task RemoveCommentByAdmin(Guid commentid)   //管理员删除评论
        {
            using (ICommentService commentService = new CommentService())
            {
                var data = await commentService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == commentid);
                await commentService.RemoveAsync(data);
            }
        }

        public async Task RemoveCommentReportAdmin(Guid reportId)       //管理员删除举报记录
        {
            using (ICommentReportService comRepSvc=new CommentReportService())
            {
                var data=await comRepSvc.GetAllAsync().FirstOrDefaultAsync(m => m.Id == reportId);
                if (data!=null)
                {
                    await comRepSvc.RemoveAsync(data);
                }
            }
        }

        public async Task RemoveUserByAdmin(Guid userId)
        {
            using (IUserService userService = new UserService())
            {
                var data=await userService.GetAllAsync().Where(m => m.Id == userId).FirstOrDefaultAsync();
                await userService.RemoveAsync(data);
            }
        }

        public async Task EditUserFreezeByAdmin(Guid userid)    //管理员有权冻结用户账号
        {
            using (IUserService userService = new UserService())
            {
                var data=await userService.GetAllAsync().FirstAsync(m => m.Id == userid);
                if (data!=null)
                {
                    data.IsFreeze = true;
                    await userService.EditAsync(data);
                }
            }
        }
        public async Task EditUserRestoreByAdmin(Guid userid)    //管理员有权冻结用户账号
        {
            using (IUserService userService = new UserService())
            {
                var data = await userService.GetAllAsync().FirstAsync(m => m.Id == userid);
                if (data != null)
                {
                    data.IsFreeze = false;
                    await userService.EditAsync(data);
                }
            }
        }
    }
}
