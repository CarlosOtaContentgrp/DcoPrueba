namespace DemoScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mensajebienv1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "LocaId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "LocaId", c => c.String());
        }
    }
}
