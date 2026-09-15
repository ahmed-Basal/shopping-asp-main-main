using core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace inftastructer.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        internal readonly object Comments;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<category> Categories { get; set; }
        public DbSet<product> Products { get; set; }
        public DbSet<photo> Photos { get; set; }
        public virtual DbSet<Address> address { get; set; }
       
        public virtual DbSet<DeliveryMethod> deliveryMethods { get; set; }
        public DbSet<comment> Comment { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(256);
                entity.HasOne(e => e.AppUser)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(e => e.AppUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }

}
