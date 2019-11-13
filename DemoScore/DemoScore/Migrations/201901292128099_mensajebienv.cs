namespace DemoScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mensajebienv : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LocaId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LocaId");
        }
    }
}
