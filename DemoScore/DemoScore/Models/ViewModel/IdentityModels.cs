using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using DemoScore.Models.Game;
using System.Data.Entity.ModelConfiguration.Conventions;
using SCORM1.Models.MainGame;

namespace DemoScore.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Nombres")]
        public string FirstName { get; set; }
        [Display(Name = "Apellidos")]
        public string LastName { get; set; }
        [Display(Name = "Rol")]
        public ROLES Role { get; set; }
        [Display(Name = "Documento")]

        public string Document { get; set; }
        [Display(Name = "Cargo")]
        public int? PositionId { get; set; }
        [Display(Name = "Area")]
        public int? AreaId { get; set; }
        [Display(Name = "Ciudad")]
        public int? CityId { get; set; }
        [Display(Name = "Ubicación")]
        public int? LocationId { get; set; }
        public string Location_Desc { get; set; }
        [DataType(DataType.Date)]
        public DateTime? lastAccess { get; set; }
        [DataType(DataType.Date)]
        public DateTime? firstAccess { get; set; }
        [Display(Name = "Terminos y Condiciones")]
        public Terms_and_Conditions TermsandConditions { get; set; }


        [ForeignKey("Company")]
        public int? CompanyId { get; set; }
        public virtual Company Company { get; set; }

        [ForeignKey("Cargo")]
        public int? Cargo_Id { get; set; }
        public virtual Cargo Cargo { get; set; }


        [ForeignKey("Area")]
        public int? Area_Id { get; set; }
        public virtual Area Area { get; set; }

        [ForeignKey("Ubicacion")]
        public int? Ubicacion_Id { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }

        [ForeignKey("Ciudad")]
        public int? Ciudad_Id { get; set; }
        public virtual Ciudad Ciudad { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ConexionDCOtest", throwIfV1Schema: false)
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Cargo> Cargos { get; set; }
        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Ciudad> Ciudades { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<MG_AnswerMultipleChoice> MG_AnswerMultipleChoices { get; set; }
        public virtual DbSet<MG_AnswerUser> MG_AnswerUsers { get; set; }
        public virtual DbSet<MG_MultipleChoice> MG_MultipleChoices { get; set; }
        public virtual DbSet<MG_SettingMp> MG_SettingMps { get; set; }
        public virtual DbSet<MG_Template> MG_Templates { get; set; }
        public virtual DbSet<Nivel> Nivels { get; set; }
        public virtual DbSet<SubCategoria> SubCategorias { get; set; }
        public virtual DbSet<Ubicacion> Ubicaciones { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");

            modelBuilder.Entity<MG_Template>()
         .HasRequired(t => t.Company)
         .WithMany(m => m.MG_Template)
         .HasForeignKey(d => d.Company_Id)
         .WillCascadeOnDelete(false);

            modelBuilder.Entity<SubCategoria>()
       .HasRequired(t => t.Categoria)
       .WithMany(m => m.SubCategoria)
       .HasForeignKey(d => d.Cate_Id)
       .WillCascadeOnDelete(false);

            modelBuilder.Entity<Categoria>()
  .HasRequired(t => t.Company)
  .WithMany(m => m.Categoria)
  .HasForeignKey(d => d.Company_Id)
  .WillCascadeOnDelete(false);
            modelBuilder.Entity<SubCategoria>()
.HasRequired(t => t.Company)
.WithMany(m => m.SubCategoria)
.HasForeignKey(d => d.Company_Id)
.WillCascadeOnDelete(false);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}