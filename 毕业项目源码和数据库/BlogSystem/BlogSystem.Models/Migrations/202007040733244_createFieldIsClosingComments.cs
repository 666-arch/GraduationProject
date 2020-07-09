namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createFieldIsClosingComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "IsClosingComments", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "IsClosingComments");
        }
    }
}
