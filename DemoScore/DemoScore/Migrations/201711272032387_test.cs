namespace DemoScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Area",
                c => new
                    {
                        AreaId = c.Int(nullable: false, identity: true),
                        AreaName = c.String(),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AreaId)
                .ForeignKey("dbo.Company", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Role = c.Int(),
                        Document = c.String(),
                        PositionId = c.Int(),
                        AreaId = c.Int(),
                        CityId = c.Int(),
                        LocationId = c.Int(),
                        lastAccess = c.DateTime(),
                        firstAccess = c.DateTime(),
                        TermsandConditions = c.Int(),
                        CompanyId = c.Int(),
                        Cargo_Id = c.Int(),
                        Area_Id = c.Int(),
                        Ubicacion_Id = c.Int(),
                        Ciudad_Id = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Area", t => t.Area_Id)
                .ForeignKey("dbo.Cargo", t => t.Cargo_Id)
                .ForeignKey("dbo.Ciudad", t => t.Ciudad_Id)
                .ForeignKey("dbo.Company", t => t.CompanyId)
                .ForeignKey("dbo.Ubicacion", t => t.Ubicacion_Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.CompanyId)
                .Index(t => t.Cargo_Id)
                .Index(t => t.Area_Id)
                .Index(t => t.Ubicacion_Id)
                .Index(t => t.Ciudad_Id);
            
            CreateTable(
                "dbo.Cargo",
                c => new
                    {
                        Posi_id = c.Int(nullable: false, identity: true),
                        Posi_Description = c.String(),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Posi_id)
                .ForeignKey("dbo.Company", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Company",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                    })
                .PrimaryKey(t => t.CompanyId);
            
            CreateTable(
                "dbo.Categoria",
                c => new
                    {
                        Cate_ID = c.Int(nullable: false, identity: true),
                        Cate_Description = c.String(),
                        Company_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Cate_ID)
                .ForeignKey("dbo.Company", t => t.Company_Id)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.MG_MultipleChoice",
                c => new
                    {
                        MuCh_ID = c.Int(nullable: false, identity: true),
                        MuCh_Description = c.String(),
                        MuCh_NameQuestion = c.String(),
                        MuCh_ImageQuestion = c.String(),
                        MuCh_Feedback = c.String(),
                        Cate_Id = c.Int(nullable: false),
                        SubC_Id = c.Int(nullable: false),
                        Sett_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MuCh_ID)
                .ForeignKey("dbo.Categoria", t => t.Cate_Id, cascadeDelete: true)
                .ForeignKey("dbo.MG_SettingMp", t => t.Sett_Id, cascadeDelete: true)
                .ForeignKey("dbo.SubCategoria", t => t.SubC_Id, cascadeDelete: true)
                .Index(t => t.Cate_Id)
                .Index(t => t.SubC_Id)
                .Index(t => t.Sett_Id);
            
            CreateTable(
                "dbo.MG_AnswerMultipleChoice",
                c => new
                    {
                        AnMul_ID = c.Int(nullable: false, identity: true),
                        AnMul_Description = c.String(),
                        AnMul_TrueAnswer = c.Int(nullable: false),
                        MuCh_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnMul_ID)
                .ForeignKey("dbo.MG_MultipleChoice", t => t.MuCh_ID, cascadeDelete: true)
                .Index(t => t.MuCh_ID);
            
            CreateTable(
                "dbo.MG_AnswerUser",
                c => new
                    {
                        AnUs_Id = c.Int(nullable: false, identity: true),
                        Attemps = c.Int(nullable: false),
                        Respuesta = c.Int(nullable: false),
                        FechaIngreso = c.DateTime(),
                        FechaEnvio = c.DateTime(),
                        User_Id = c.String(maxLength: 128),
                        AnMul_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnUs_Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.MG_AnswerMultipleChoice", t => t.AnMul_ID, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.AnMul_ID);
            
            CreateTable(
                "dbo.MG_SettingMp",
                c => new
                    {
                        Sett_Id = c.Int(nullable: false, identity: true),
                        Sett_Attemps = c.Int(nullable: false),
                        Sett_InitialDate = c.DateTime(nullable: false),
                        Sett_CloseDate = c.DateTime(nullable: false),
                        Sett_NumberOfQuestions = c.Int(nullable: false),
                        Sett_Instruction = c.String(),
                        Sett_Audio1 = c.String(),
                        Sett_Audio2 = c.String(),
                        Sett_Audio3 = c.String(),
                        Sett_Audio4 = c.String(),
                        Sett_Audio5 = c.String(),
                        Plan_Id = c.Int(nullable: false),
                        Company_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Sett_Id)
                .ForeignKey("dbo.Company", t => t.Company_Id, cascadeDelete: true)
                .ForeignKey("dbo.MG_Template", t => t.Plan_Id, cascadeDelete: true)
                .Index(t => t.Plan_Id)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.MG_Template",
                c => new
                    {
                        Plant_Id = c.Int(nullable: false, identity: true),
                        Plant_Color = c.String(),
                        Plant_Img_instructions = c.String(),
                        Plant_Img_Questions = c.String(),
                        Company_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Plant_Id)
                .ForeignKey("dbo.Company", t => t.Company_Id)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.SubCategoria",
                c => new
                    {
                        SubC_ID = c.Int(nullable: false, identity: true),
                        SubC_Description = c.String(),
                        Company_Id = c.Int(nullable: false),
                        Cate_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubC_ID)
                .ForeignKey("dbo.Categoria", t => t.Cate_Id)
                .ForeignKey("dbo.Company", t => t.Company_Id)
                .Index(t => t.Company_Id)
                .Index(t => t.Cate_Id);
            
            CreateTable(
                "dbo.Ubicacion",
                c => new
                    {
                        Loca_Id = c.Int(nullable: false, identity: true),
                        Loca_Description = c.String(),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Loca_Id)
                .ForeignKey("dbo.Company", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Ciudad",
                c => new
                    {
                        City_Id = c.Int(nullable: false, identity: true),
                        City_Name = c.String(),
                    })
                .PrimaryKey(t => t.City_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.IdentityUser_Id)
                .Index(t => t.RoleId)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserLogins", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserClaims", "IdentityUser_Id", "dbo.Users");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Area", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Users", "Ubicacion_Id", "dbo.Ubicacion");
            DropForeignKey("dbo.Users", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Users", "Ciudad_Id", "dbo.Ciudad");
            DropForeignKey("dbo.Users", "Cargo_Id", "dbo.Cargo");
            DropForeignKey("dbo.Cargo", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.Ubicacion", "CompanyId", "dbo.Company");
            DropForeignKey("dbo.MG_MultipleChoice", "SubC_Id", "dbo.SubCategoria");
            DropForeignKey("dbo.SubCategoria", "Company_Id", "dbo.Company");
            DropForeignKey("dbo.SubCategoria", "Cate_Id", "dbo.Categoria");
            DropForeignKey("dbo.MG_MultipleChoice", "Sett_Id", "dbo.MG_SettingMp");
            DropForeignKey("dbo.MG_SettingMp", "Plan_Id", "dbo.MG_Template");
            DropForeignKey("dbo.MG_Template", "Company_Id", "dbo.Company");
            DropForeignKey("dbo.MG_SettingMp", "Company_Id", "dbo.Company");
            DropForeignKey("dbo.MG_AnswerMultipleChoice", "MuCh_ID", "dbo.MG_MultipleChoice");
            DropForeignKey("dbo.MG_AnswerUser", "AnMul_ID", "dbo.MG_AnswerMultipleChoice");
            DropForeignKey("dbo.MG_AnswerUser", "User_Id", "dbo.Users");
            DropForeignKey("dbo.MG_MultipleChoice", "Cate_Id", "dbo.Categoria");
            DropForeignKey("dbo.Categoria", "Company_Id", "dbo.Company");
            DropForeignKey("dbo.Users", "Area_Id", "dbo.Area");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "IdentityUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "IdentityUser_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "IdentityUser_Id" });
            DropIndex("dbo.Ubicacion", new[] { "CompanyId" });
            DropIndex("dbo.SubCategoria", new[] { "Cate_Id" });
            DropIndex("dbo.SubCategoria", new[] { "Company_Id" });
            DropIndex("dbo.MG_Template", new[] { "Company_Id" });
            DropIndex("dbo.MG_SettingMp", new[] { "Company_Id" });
            DropIndex("dbo.MG_SettingMp", new[] { "Plan_Id" });
            DropIndex("dbo.MG_AnswerUser", new[] { "AnMul_ID" });
            DropIndex("dbo.MG_AnswerUser", new[] { "User_Id" });
            DropIndex("dbo.MG_AnswerMultipleChoice", new[] { "MuCh_ID" });
            DropIndex("dbo.MG_MultipleChoice", new[] { "Sett_Id" });
            DropIndex("dbo.MG_MultipleChoice", new[] { "SubC_Id" });
            DropIndex("dbo.MG_MultipleChoice", new[] { "Cate_Id" });
            DropIndex("dbo.Categoria", new[] { "Company_Id" });
            DropIndex("dbo.Cargo", new[] { "CompanyId" });
            DropIndex("dbo.Users", new[] { "Ciudad_Id" });
            DropIndex("dbo.Users", new[] { "Ubicacion_Id" });
            DropIndex("dbo.Users", new[] { "Area_Id" });
            DropIndex("dbo.Users", new[] { "Cargo_Id" });
            DropIndex("dbo.Users", new[] { "CompanyId" });
            DropIndex("dbo.Users", "UserNameIndex");
            DropIndex("dbo.Area", new[] { "CompanyId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Ciudad");
            DropTable("dbo.Ubicacion");
            DropTable("dbo.SubCategoria");
            DropTable("dbo.MG_Template");
            DropTable("dbo.MG_SettingMp");
            DropTable("dbo.MG_AnswerUser");
            DropTable("dbo.MG_AnswerMultipleChoice");
            DropTable("dbo.MG_MultipleChoice");
            DropTable("dbo.Categoria");
            DropTable("dbo.Company");
            DropTable("dbo.Cargo");
            DropTable("dbo.Users");
            DropTable("dbo.Area");
        }
    }
}
