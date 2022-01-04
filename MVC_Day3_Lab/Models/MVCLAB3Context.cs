using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MVC_Day3_Lab.Models
{
    public partial class MVCLAB3Context : DbContext
    {
        public MVCLAB3Context()
            : base("name=MVCLAB3Context")
        {
        }

        public virtual DbSet<Catalog> Catalogs { get; set; }
        public virtual DbSet<New> News { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Skills)
                .Map(m => m.ToTable("User_Skill").MapLeftKey("SkillId").MapRightKey("UserId"));
        }
    }
}
