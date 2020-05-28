using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.Models;
using BlogSystem.IDAL;
namespace BlogSystem.DAL
{
    public class AdminService:BaseService<Admin>,IAdminService
    {
        public AdminService():base(new BlogContext())
        {

        }
    }
}
