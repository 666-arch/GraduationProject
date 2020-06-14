namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsBeReportUserConfirm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CommentReports", "IsBeReportUserConfirm", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CommentReports", "IsBeReportUserConfirm");
        }
    }
}
