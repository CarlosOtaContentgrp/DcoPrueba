namespace DemoScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mensaje1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Location_Desc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Location_Desc");
        }
    }
}
