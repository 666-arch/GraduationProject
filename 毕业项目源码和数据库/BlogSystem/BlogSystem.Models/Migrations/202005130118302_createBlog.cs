namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createBlog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Account = c.String(nullable: false, maxLength: 20, unicode: false),
                        Password = c.String(nullable: false, maxLength: 30, unicode: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false),
                        Content = c.String(nullable: false, storeType: "ntext"),
                        UserId = c.Guid(nullable: false),
                        GoodCounnt = c.Int(nullable: false),
                        BadCount = c.Int(nullable: false),
                        State = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Eamil = c.String(nullable: false, maxLength: 40, unicode: false),
                        NickName = c.String(nullable: false, maxLength: 20, unicode: false),
                        Password = c.String(nullable: false, maxLength: 30, unicode: false),
                        ImagePath = c.String(nullable: false, maxLength: 300, unicode: false),
                        PersonalDescription = c.String(),
                        FansCount = c.Int(nullable: false),
                        FocusCount = c.Int(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ArticleToCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        BlogCategoryId = c.Guid(nullable: false),
                        ArticleId = c.Guid(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleId)
                .ForeignKey("dbo.BlogCategories", t => t.BlogCategoryId)
                .Index(t => t.BlogCategoryId)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.BlogCategories",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CategoryName = c.String(),
                        UserId = c.Guid(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Content = c.String(nullable: false, maxLength: 600),
                        UserId = c.Guid(nullable: false),
                        ArticleId = c.Guid(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.Fans",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        FocusUserId = c.Guid(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.FocusUserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.FocusUserId);
            
            CreateTable(
                "dbo.Links",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Title = c.String(nullable: false, maxLength: 30, unicode: false),
                        Url = c.String(nullable: false, maxLength: 50, unicode: false),
                        Describe = c.String(nullable: false, maxLength: 200, unicode: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Fans", "UserId", "dbo.Users");
            DropForeignKey("dbo.Fans", "FocusUserId", "dbo.Users");
            DropForeignKey("dbo.Comments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Comments", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.ArticleToCategories", "BlogCategoryId", "dbo.BlogCategories");
            DropForeignKey("dbo.BlogCategories", "UserId", "dbo.Users");
            DropForeignKey("dbo.ArticleToCategories", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.Articles", "UserId", "dbo.Users");
            DropIndex("dbo.Fans", new[] { "FocusUserId" });
            DropIndex("dbo.Fans", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "ArticleId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropIndex("dbo.BlogCategories", new[] { "UserId" });
            DropIndex("dbo.ArticleToCategories", new[] { "ArticleId" });
            DropIndex("dbo.ArticleToCategories", new[] { "BlogCategoryId" });
            DropIndex("dbo.Articles", new[] { "UserId" });
            DropTable("dbo.Links");
            DropTable("dbo.Fans");
            DropTable("dbo.Comments");
            DropTable("dbo.BlogCategories");
            DropTable("dbo.ArticleToCategories");
            DropTable("dbo.Users");
            DropTable("dbo.Articles");
            DropTable("dbo.Admins");
        }
    }
}
