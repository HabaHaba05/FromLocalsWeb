using FromLocalsToLocals.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace FromLocalsToLocals.Database
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
    
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options){
            ChangeTracker.Tracked += OnEntityTracked;
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<WorkHours> VendorWorkHours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Follower>()
                .HasKey(bc => new { bc.UserID, bc.VendorID });
            modelBuilder.Entity<Follower>()
              .HasOne(u => u.User)
              .WithMany(u => u.Following)
              .HasForeignKey(bc => bc.UserID).IsRequired().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Follower>()
                .HasOne(bc => bc.Vendor)
                .WithMany(c => c.Followers)
                .HasForeignKey(bc => bc.VendorID).IsRequired().OnDelete(DeleteBehavior.Restrict);
        }

        private void OnEntityTracked(object sender, EntityTrackedEventArgs e)
        {
            if (!e.FromQuery && e.Entry.State == EntityState.Added && e.Entry.Entity is Review review)
            {
                review.Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            }
            
            else if (!e.FromQuery && e.Entry.State == EntityState.Added && e.Entry.Entity is Vendor vendor)
            {
                vendor.DateCreated = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            }

            else if (!e.FromQuery && e.Entry.State == EntityState.Added && e.Entry.Entity is Post post)
            {
                post.Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }
}
