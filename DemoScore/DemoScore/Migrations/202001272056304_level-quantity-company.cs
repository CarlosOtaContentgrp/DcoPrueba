namespace DemoScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class levelquantitycompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Company", "qnivel1", c => c.Int(nullable: true));
            AddColumn("dbo.Company", "qnivel2", c => c.Int(nullable: true));
            AddColumn("dbo.Company", "qnivel3", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Company", "qnivel3");
            DropColumn("dbo.Company", "qnivel2");
            DropColumn("dbo.Company", "qnivel1");
        }
    }
}
