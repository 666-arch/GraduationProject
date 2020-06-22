namespace BlogSystem.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Data.Entity.ModelConfiguration.Conventions;
    public class BlogContext : DbContext
    {
        //您的上下文已配置为从您的应用程序的配置文件(App.config 或 Web.config)
        //使用“BlogContext”连接字符串。默认情况下，此连接字符串针对您的 LocalDb 实例上的
        //“BlogSystem.Models.BlogContext”数据库。
        // 
        //如果您想要针对其他数据库和/或数据库提供程序，请在应用程序配置文件中修改“BlogContext”
        //连接字符串。
        public BlogContext(): base("name=BlogContext")
        {
            Database.SetInitializer<BlogContext>(null);  //初始化数据库上下文
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();  //移除默认外键约束
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }

        //上下文注册
        public DbSet<User> Users { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleToCategory> ArticleToCategories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Fans> Fans { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<ArticleCollect> ArticleCollects { get; set; }
        public DbSet<CommentReport> CommentReports { get; set; }
        public DbSet<ReplyComments> ReplyComments { get; set; }
    }
}