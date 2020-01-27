namespace DemoScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnnivelidmuch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MG_MultipleChoice", "Nivel_Id", c => c.Int(nullable: true));
            CreateIndex("dbo.MG_MultipleChoice", "Nivel_Id");
            AddForeignKey("dbo.MG_MultipleChoice", "Nivel_Id", "dbo.Nivel", "Nivel_Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MG_MultipleChoice", "Nivel_Id", "dbo.Nivel");
            DropIndex("dbo.MG_MultipleChoice", new[] { "Nivel_Id" });
            DropColumn("dbo.MG_MultipleChoice", "Nivel_Id");
        }
    }
}
