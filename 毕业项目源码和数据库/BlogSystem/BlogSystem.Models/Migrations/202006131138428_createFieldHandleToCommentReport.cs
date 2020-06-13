namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createFieldHandleToCommentReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommentReports", "IsHandle", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommentReports", "IsHandle");
        }
    }
}
