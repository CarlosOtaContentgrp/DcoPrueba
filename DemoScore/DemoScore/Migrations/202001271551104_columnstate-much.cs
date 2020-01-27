namespace DemoScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class columnstatemuch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MG_MultipleChoice", "state", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MG_MultipleChoice", "state");
        }
    }
}
