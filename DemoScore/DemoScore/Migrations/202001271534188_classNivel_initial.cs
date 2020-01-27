namespace DemoScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class classNivel_initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Nivel",
                c => new
                    {
                        Nivel_Id = c.Int(nullable: false, identity: true),
                        Nivel_Name = c.String(),
                    })
                .PrimaryKey(t => t.Nivel_Id);
            
            AddColumn("dbo.Users", "Nivel_Nivel_Id", c => c.Int());
            CreateIndex("dbo.Users", "Nivel_Nivel_Id");
            AddForeignKey("dbo.Users", "Nivel_Nivel_Id", "dbo.Nivel", "Nivel_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Nivel_Nivel_Id", "dbo.Nivel");
            DropIndex("dbo.Users", new[] { "Nivel_Nivel_Id" });
            DropColumn("dbo.Users", "Nivel_Nivel_Id");
            DropTable("dbo.Nivel");
        }
    }
}
