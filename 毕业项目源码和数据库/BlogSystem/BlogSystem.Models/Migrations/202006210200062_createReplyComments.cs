namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createReplyComments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReplyComments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ReplierId = c.Guid(nullable: false),
                        TargetToReplyId = c.Guid(nullable: false),
                        ReplyType = c.Int(nullable: false),
                        CommentParentId = c.Guid(nullable: false),
                        ReplyToTargetCommentId = c.Guid(nullable: false),
                        ReplyContent = c.String(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                        IsRemoved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Comments", t => t.CommentParentId)
                .ForeignKey("dbo.Users", t => t.ReplierId)
                .ForeignKey("dbo.Users", t => t.TargetToReplyId)
                .Index(t => t.ReplierId)
                .Index(t => t.TargetToReplyId)
                .Index(t => t.CommentParentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReplyComments", "TargetToReplyId", "dbo.Users");
            DropForeignKey("dbo.ReplyComments", "ReplierId", "dbo.Users");
            DropForeignKey("dbo.ReplyComments", "CommentParentId", "dbo.Comments");
            DropIndex("dbo.ReplyComments", new[] { "CommentParentId" });
            DropIndex("dbo.ReplyComments", new[] { "TargetToReplyId" });
            DropIndex("dbo.ReplyComments", new[] { "ReplierId" });
            DropTable("dbo.ReplyComments");
        }
    }
}
