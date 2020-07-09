namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createFieldIsFreeze : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsFreeze", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "IsFreeze");
        }
    }
}
