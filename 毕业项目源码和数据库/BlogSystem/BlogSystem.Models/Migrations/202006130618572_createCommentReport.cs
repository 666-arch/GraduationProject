namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createCommentReport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommentReports",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        CommentId = c.Guid(nullable: false),
                        Content = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Comments", t => t.CommentId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CommentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommentReports", "UserId", "dbo.Users");
            DropForeignKey("dbo.CommentReports", "CommentId", "dbo.Comments");
            DropIndex("dbo.CommentReports", new[] { "CommentId" });
            DropIndex("dbo.CommentReports", new[] { "UserId" });
            DropTable("dbo.CommentReports");
        }
    }
}
