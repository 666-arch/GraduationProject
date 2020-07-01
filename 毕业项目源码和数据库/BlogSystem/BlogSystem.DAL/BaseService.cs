using BlogSystem.IDAL;
using BlogSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace BlogSystem.DAL
{
    public class BaseService<T> : IBaseService<T> where T : BaseEntity,new()
    {

        private readonly BlogContext _db;  //创建上下文对象（只读）
        public BaseService(BlogContext db) //通过构造函数获取上下文
        {
            _db = db;
        }
        public async Task CreateAsync(T model, bool save = true)  //新增
        {

            _db.Set<T>().Add(model);
            if (save)
            {
                await _db.SaveChangesAsync();  //主线程不会等待此内容
            }
        }

        public async Task EditAsync(T model, bool save = true)
        {
            //关闭验证
            _db.Configuration.ValidateOnSaveEnabled = false;
            _db.Entry(model).State = EntityState.Modified;
            if (save)
            {
                await _db.SaveChangesAsync();
                _db.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        public async Task RemoveAsync(Guid id,bool save=true)  //根据id删除
        {
            _db.Configuration.ValidateOnSaveEnabled = false;
            var t = new T()  //new一个实体对象 T  此时状态为游离态
            {
                Id = id
            };
            _db.Entry(t).State = EntityState.Unchanged; //此时状态为持久态
            t.IsRemoved = true; //伪删除(改状态)
            if (save)
            {
                await _db.SaveChangesAsync();   //save以后又是游离态
                _db.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        public async Task RemoveAsync(T model, bool save = true)  //删除对象
        {
             await RemoveAsync(model.Id,save);
        }

        public async Task<T> GetOneByIdAsync(Guid id)   //这里才真正得到数据，所以才有异步
        {
            return await GetAllAsync().FirstAsync(m => m.Id == id);
        }

        public IQueryable<T> GetAllAsync()  //查询所有未被删除数据（未被真正的执行，要看后面加不加条件）
        {
            return  _db.Set<T>().Where(m =>!m.IsRemoved).AsNoTracking();  //AsNoTracking性能比ToList<>快4.8倍
        }

        public IQueryable<T> GetAllOrderAsync(bool asc = true)  
        {
            var datas = GetAllAsync();  //得到所有数据
            if (asc)
            {
                datas.OrderBy(m => m.CreateTime);  //升序
            }
            else
            {
                datas.OrderByDescending(m => m.CreateTime);  //降序
            }
            return datas;
        }
        public IQueryable<T> GetAllByPageAsync(int PageSize = 10, int PageIndex = 0)
        {
            throw new NotImplementedException();
        }

        public async Task Saved()
        {
            await _db.SaveChangesAsync();
            //保存后还原验证
            _db.Configuration.ValidateOnSaveEnabled = true;
        }

        public void Dispose()   //释放数据库连接 ,用一次就断开
        {
            _db.Dispose();
        }
    }
}
