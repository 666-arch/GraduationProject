namespace BlogSystem.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createFieldIsStick : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "IsStick", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "IsStick");
        }
    }
}
