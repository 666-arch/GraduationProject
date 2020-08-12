using BlogSystem.Dto;
using BlogSystem.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.Models;
using System.Data.Entity;
using BlogSystem.DAL;
using BlogSystem.IDAL;

namespace BlogSystem.BLL
{
    public class UserManger : IUserMnager
    {
        public async Task Register(string email, string nickname, string password)  //注册
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var data=await userSvc.GetAllAsync().FirstOrDefaultAsync(m => m.Eamil == email);
                if (data==null)     //为 null 说明该邮箱未注册过
                {
                    await userSvc.CreateAsync(new User()
                    {
                        Eamil = email,
                        NickName = nickname,
                        Password = password, 
                        ImagePath = "0.png",
                        PersonalDescription = "个人说明可以让学者更多人了解你哦..."
                    });
                }
            }
        }

        public bool Login(string email, string password,out Guid userid,out string nickname,out string imagepath)  //抛出登录用户id
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var user = userSvc.GetAllAsync().FirstOrDefaultAsync(m => m.Eamil == email && m.Password == password);
                user.Wait();
                var data = user.Result;
                if (data == null)
                {
                    userid = new Guid();  //null 创建新的用户id
                    nickname = "";
                    imagepath = "";
                    return false;
                }
                else
                {
                    userid = data.Id;  //获得用户id
                    nickname = data.NickName;
                    imagepath = data.ImagePath;
                    return true;
                }
            }
        }

        public async Task ChangePassword(string email, string newPwd, string oldPwd)  //修改密码
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var data = await userSvc.GetAllAsync().AnyAsync(m => m.Eamil == email && m.Password == oldPwd); //判断是否存在该用户
                if (data)
                {
                    //找到对应的用户修改密码
                    var user = await userSvc.GetAllAsync().FirstAsync(m => m.Eamil == email);
                    user.Password = newPwd;
                    await userSvc.EditAsync(user);
                }
                else
                {
                    throw new ArgumentException("旧密码错误");
                }
            }
        }

        public async Task ChangeUserInformation(string email, string nickname, string ImagePath, string psersonDesc)  //修改个人信息
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var user = await userSvc.GetAllAsync().FirstAsync(m => m.Eamil == email);
                user.NickName = nickname;
                user.ImagePath = ImagePath;
                user.PersonalDescription = psersonDesc;
                await userSvc.EditAsync(user);
            }
        }



        public async Task<UserInformation> GetByEmail(string email)   //查询用户,将Dto数据显示在页面
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                if (await userSvc.GetAllAsync().AnyAsync(m => m.Eamil == email))
                {
                    return await userSvc.GetAllAsync().Where(m => m.Eamil == email).Select(m => new Dto.UserInformation()  //通过Select,这里从Models数据转到Dto
                    {
                        Id = m.Id,
                        Eamil = m.Eamil,
                        NickName = m.NickName,   //昵称
                        ImagePath = m.ImagePath,
                        FansCount = m.FansCount,
                        FocusCount = m.FocusCount,
                        PersonalDescription = m.PersonalDescription, //个人说明
                        CreateTime = m.CreateTime
                    }).FirstAsync(); //select获取的是集合, 这里取其中一个
                }
                else
                {
                    return null;
                }
            }
        }
        public async Task<UserInformation> GetById(Guid userid)  
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                if (await userSvc.GetAllAsync().AnyAsync(m => m.Id == userid))
                {
                    return await userSvc.GetAllAsync().Where(m => m.Id == userid).Select(m => new Dto.UserInformation()  
                    {
                        Id = m.Id,
                        Eamil = m.Eamil,
                        Password=m.Password,
                        NickName = m.NickName,   
                        ImagePath = m.ImagePath,
                        FansCount = m.FansCount,
                        FocusCount = m.FocusCount,
                        PersonalDescription = m.PersonalDescription, 
                        CreateTime = m.CreateTime,
                        IsFreeze=m.IsFreeze
                    }).FirstAsync(); 
                }
                else
                {
                    throw new ArgumentException("邮箱地址不存在");
                }
            }
        }

        public async Task ChangeUserInformationById(Guid id, string email, string nickname, string psersonDesc)
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var setuser = await userSvc.GetAllAsync().FirstAsync(m => m.Id == id);
                setuser.Eamil = email;
                setuser.NickName = nickname;
                setuser.PersonalDescription = psersonDesc;
                await userSvc.EditAsync(setuser);
            }
        }

        public async Task ChangeUserImagePathById(Guid id, string ImagePath)  //用户修改头像
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var user = await userSvc.GetAllAsync().FirstAsync(m => m.Id == id);
                user.ImagePath = ImagePath;
                await userSvc.EditAsync(user);
            }
        }

        public async Task CreateFans(Guid userid, Guid focususerId)  //用户关注
        {
            using (IDAL.IFansService fansSvc = new FansService())
            {
                    await fansSvc.CreateAsync(new Fans()
                    {
                        UserId = userid,
                        FocusUserId = focususerId,
                    });
            }
            //using (IDAL.IUserService userService = new UserService())
            //{
            //    var datafocus=await userService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == userid);
            //    var datafans = await userService.GetAllAsync().FirstOrDefaultAsync(m => m.Id == focususerId);
            //    if (datafocus != null)
            //    {
            //        datafocus.FocusCount += 1;
            //    }
            //    if (datafans!=null)
            //    {
            //        datafans.FansCount += 1;
            //    }
            //    userService.EditAsync(da)
            //}
        }

        public async Task<List<FansDto>> GetAllFansByuserId(Guid userid, Guid focususerId) //查询关注的人
        {
            using (IDAL.IFansService fansvc = new FansService())
            {
                var data= await fansvc.GetAllAsync()
                               .Where(m => m.UserId == userid)
                               .Where(m => m.FocusUserId == focususerId)
                               .Select(m => new FansDto()
                               {
                                   UserId = userid,
                                   FocusUserId = focususerId,
                                   NickName=m.User.NickName,
                                   ImagePath=m.User.ImagePath
                               }).ToListAsync();
                return data;
            }
        }

        public async Task<List<FansDto>> GetAllFansByFocususerId(Guid focususerId)  //对应用户的粉丝数
        {
            using (IDAL.IFansService fansService = new FansService())
            {
               return await fansService.GetAllAsync()
                     .Where(m => m.FocusUserId == focususerId)
                     .Where(m => m.IsRemoved == false)
                     .Select(m => new FansDto()
                     {
                        UserId=m.UserId,
                        FocusUserId=m.FocusUserId,
                        NickName = m.User.NickName,
                        ImagePath = m.User.ImagePath
                     }).ToListAsync();
            }
        }
        public async Task<List<FansDto>> GetAllFocusByUserid(Guid userid)  //对应用户的关注数
        {
            using (IDAL.IFansService fansService = new FansService())
            {
                return await fansService.GetAllAsync()
                    .Where(m => m.UserId == userid)
                    .Where(m=>m.IsRemoved==false)
                    .Select(m => new FansDto()
                {
                    UserId = m.UserId,
                    Desc=m.User.PersonalDescription,
                    FocusUserId = m.FocusUserId,
                    NickName = m.FocusUser.NickName,
                    ImagePath = m.FocusUser.ImagePath
                }).ToListAsync();
            }
        }

        public async Task Unfollow(Guid userid, Guid focusid)  //用户取消关注,伪删除关注记录
        {
            using (IDAL.IFansService fansService = new FansService())
            {
                var data =await fansService.GetAllAsync()
                    .Where(m => m.UserId == userid)
                    .Where(m => m.FocusUserId == focusid).FirstAsync();
                data.IsRemoved = true; //伪删除
                await fansService.EditAsync(data);
            }
        }

        public async Task<List<UserInformation>> GetAllUserByAdmin(string email,string nickname, string desc, string isfreeze)      //管理员查询用户
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
               var data= await userSvc.GetAllAsync()
                    .Where(m => string.IsNullOrEmpty(email)&string.IsNullOrEmpty(nickname)&string.IsNullOrEmpty(desc)&string.IsNullOrEmpty(isfreeze) || m.Eamil.Contains(email)&m.NickName.Contains(nickname)&m.PersonalDescription.Contains(desc)&m.IsFreeze.ToString().Contains(isfreeze))
                    .Select(m => new UserInformation()
                {
                    Id=m.Id,
                    Eamil = m.Eamil,
                    NickName = m.NickName,
                    Password = m.Password,
                    ImagePath = m.ImagePath,
                    PersonalDescription = m.PersonalDescription,
                    CreateTime = m.CreateTime,
                    IsFreeze=m.IsFreeze
                 }).ToListAsync();
                return data;
            }
        }
        public async Task<List<UserInformation>> GetAllUserByAdmin()        //管理员查询所有用户
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var data= await userSvc
                     .GetAllAsync()
                     .OrderByDescending(m => m.FansCount)
                     .Select(m => new UserInformation()
                     {
                         Id = m.Id,
                         Eamil = m.Eamil,
                         NickName = m.NickName,
                         Password = m.Password,
                         ImagePath = m.ImagePath,
                         PersonalDescription = m.PersonalDescription,
                         CreateTime = m.CreateTime,
                         FansCount = m.FansCount,
                         FocusCount = m.FocusCount
                     }).ToListAsync();
                return data;
            }
        }
        public async Task<List<UserInformation>> GetAllUserByAdminUnder()        //管理员查询所有用户
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var data = await userSvc
                     .GetAllAsync()
                     .Where(m=>m.IsFreeze==false)
                     .OrderByDescending(m => m.FansCount)
                     .Select(m => new UserInformation()
                     {
                         Id = m.Id,
                         Eamil = m.Eamil,
                         NickName = m.NickName,
                         Password = m.Password,
                         ImagePath = m.ImagePath,
                         PersonalDescription = m.PersonalDescription,
                         CreateTime = m.CreateTime,
                         FansCount = m.FansCount,
                         FocusCount = m.FocusCount
                     }).ToListAsync();
                return data;
            }
        }
        
        public async Task<List<UserInformation>> GetAllUserByAdminLayUi(int pageSize, int pageIndex)
        {
            using (IUserService userSvc = new UserService())
            {
                var data = await userSvc
                    .GetAllByPageOrderAsync(pageSize,pageIndex)
                    .Select(m => new UserInformation()
                    {
                        Id = m.Id,
                        Eamil = m.Eamil,
                        NickName = m.NickName,
                        Password = m.Password,
                        ImagePath = m.ImagePath,
                        PersonalDescription = m.PersonalDescription,
                        CreateTime = m.CreateTime,
                        FansCount = m.FansCount,
                        FocusCount = m.FocusCount
                    }).ToListAsync();
                return data;
            }
        }

        public async Task<int> GetDataCount()
        {
            using (IUserService userService = new UserService())
            {
                return await userService.GetAllAsync().CountAsync();
            }
        }

        public async Task ChangeUserFanscunt(Guid id, int focus, int fans)
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var data=await userSvc.GetAllAsync().FirstOrDefaultAsync(m => m.Id == id);
                data.FocusCount = focus;
                data.FansCount = fans;
                await userSvc.EditAsync(data);
            }
        }
        public async Task<List<UserInformation>> GetAllUserlike(string email, string nickname)
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
               return await userSvc.GetAllAsync().Where(m => m.Eamil == email || m.NickName == nickname).
                        Select(m => new UserInformation()
                        {
                            Eamil = m.Eamil,
                            NickName = m.NickName,
                            Password = m.Password,
                            ImagePath = m.ImagePath,
                            PersonalDescription = m.PersonalDescription,
                            CreateTime = m.CreateTime
                        }).ToListAsync();
            }
        }

        public async Task ForgetPassword(string eamil, string newPwd)   //忘记密码
        {
            using (IDAL.IUserService userService=new UserService())
            {
                var data=await userService.GetAllAsync().FirstOrDefaultAsync(m => m.Eamil == eamil);
                if (data!=null)
                {
                    data.Password = newPwd;
                    await userService.EditAsync(data);
                }
            }
        }

        public async Task<List<ArticleCollectDto>> GetAllArticleCollectByUser(Guid userid)  //查询我的收藏
        {
            using (IArticleCollectService artCollectService = new ArticleCollectService())
            {
                var data=await artCollectService.GetAllAsync()
                    .Where(m => m.UserId == userid)
                    .Include(m => m.Article)
                    .Select(m => new ArticleCollectDto()
                    {
                        Title = m.Article.Title,
                        ArticleId = m.ArticleId
                    }).ToListAsync();
                return data;
            }
        }

        public async Task<List<ArticleDto>> GetAllArticleByTime(DateTime times, Guid userid)     //查询当天发布的文章
        {
            using (IArticleService artSvc = new ArticleService())
            {
             
                var data=await artSvc.GetAllAsync()
                    .Where(m => m.CreateTime.Day == times.Day)
                    .Where(m=>m.UserId==userid)
                    .Select(m => new ArticleDto()
                    {
                        Title = m.Title,
                        Id = m.Id,
                        CreateTime = m.CreateTime
                    }).ToListAsync();
                //2020/6/4 19:29:56
                return data;
            }
        }

        public async Task<List<CommentReportDto>> GetAllReportFeedBackInfo(Guid userid)
        {
            using (ICommentReportService commentReportService = new CommentReportService())
            {
               var data= await commentReportService.GetAllAsync()
                    .Include(m => m.User)
                    .Include(m=>m.Comment.Article)
                    .Where(m => m.UserId == userid&m.IsHandle==true&m.IsConfirm==false)        //true管理员已处理、用户未确认提供反馈信息
                    .Select(m => new CommentReportDto()
                    {
                        IsHandle = m.IsHandle,
                        NickName = m.Comment.User.NickName, //被举报人
                        Content = m.Content,
                        Title = m.Comment.Article.Title,
                        ReportUserId=m.Comment.UserId,
                        ArticleId=m.Comment.ArticleId,
                        CommentContent=m.Comment.Content,
                        CreateTime = m.CreateTime
                    }).ToListAsync();
               return data;
            }
        }

        public async Task EditIsConfirmByUser(Guid userId)          //用户确认反馈信息
        {
            using (ICommentReportService comSvc = new CommentReportService())
            {
                var data=await comSvc.GetAllAsync().FirstOrDefaultAsync(m => m.UserId == userId);
                data.IsConfirm = true;
                await comSvc.EditAsync(data);
            }
        }

        public async Task<List<CommentReportDto>> Bereported(Guid beRepuserid)
        {
            using (ICommentReportService comSvc = new CommentReportService())
            {
                 var data=await comSvc.GetAllAsync()
                    .Include(m => m.Comment)
                    .Include(m => m.User)
                    .Include(m=>m.Comment.Article)
                    .Where(m => m.Comment.UserId == beRepuserid&m.IsBeReportUserConfirm==false)     //被举报人未确认提供反馈信息
                    .Select(m => new CommentReportDto()
                    {
                        NickName = m.User.NickName,     //举报人
                        CommentContent = m.Comment.Content,     //被举报的评论
                        Title = m.Comment.Article.Title,     //所在文章
                        UserId = m.UserId,  //举报人Id
                        ArticleId = m.Comment.ArticleId     //被评论所在文章Id
                    }).ToListAsync();
                return data;
            }
        }

        public async Task EditeIsConfirmByBeReportUser(Guid userId)
        {
            using (ICommentReportService comSvc = new CommentReportService())
            {
                var data=await comSvc
                    .GetAllAsync()
                    .Include(m=>m.Comment)
                    .FirstAsync(m => m.Comment.UserId == userId);
                if (data!=null)
                {
                    data.IsBeReportUserConfirm = true;
                    await comSvc.EditAsync(data);
                }
            }
        }

        public bool CheckUserIsFreeze(string eamil)
        {
            using (IUserService userSvc = new UserService())
            {
                var result=userSvc.GetAllAsync().Any(m => m.Eamil == eamil&&m.IsFreeze==true);
                return result;
            }
        }
    }
}