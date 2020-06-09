namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createArticleCollect : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleCollects",
                c => new
                    {
                        Id = c.Guid(nullable: false),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArticleCollects", "UserId", "dbo.Users");
            DropForeignKey("dbo.ArticleCollects", "ArticleId", "dbo.Articles");
            DropIndex("dbo.ArticleCollects", new[] { "ArticleId" });
            DropIndex("dbo.ArticleCollects", new[] { "UserId" });
            DropTable("dbo.ArticleCollects");
        }
    }
}
