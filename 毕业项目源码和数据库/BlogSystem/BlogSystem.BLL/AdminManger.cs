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

        public async Task<List<AdminDto>> GetAllAdminInfo()
        {
            using (IAdminService adminService = new AdminService())
            {
               return await adminService.GetAllAsync().Select(m => new AdminDto()
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

        public async Task<List<LinkDto>> GetAllLink(string linkname)
        {
            using (ILinkService linkService = new LinkService())
            {
                return await linkService.GetAllAsync()
                    .Where(m=>string.IsNullOrEmpty(linkname)||m.Title.Contains(linkname))
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
    }
}
