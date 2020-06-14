namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createFieldIsConfirmToCommentReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommentReports", "IsConfirm", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommentReports", "IsConfirm");
        }
    }
}
