using BlogSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystem.IDAL
{
    public interface IBaseService<T>:IDisposable where T: BaseEntity   //基础服务
    {
        Task CreateAsync(T model, bool save = true);  //创建, 自动保存

        Task EditAsync(T model, bool save = true);  

        Task RemoveAsync(Guid id, bool save=true);

        Task RemoveAsync(T model, bool save = true);

        Task Saved();  //保存方法

        Task<T> GetOneByIdAsync(Guid id);  //根据id查询

        IQueryable<T> GetAllAsync();  //查询所有

        IQueryable<T> GetAllByPageAsync(int PageSize =10, int PageIndex = 0);  //分页查询

        IQueryable<T> GetAllOrderAsync(bool asc = true);  //默认true升序

    }
}
