using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace DatingApp.API.Data
{
    // public class DataContext : DbContext
    // Configuration of Identity e.g. <int> means using int for Ids instead of strings (GUID). Just a personal preference, not a must-have.
    public class DataContext : IdentityDbContext<User, Role, int, 
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {      

        }

        public DbSet<Value> Values { get; set; }
        // public DbSet<User> Users { get; set; } // Provided by Identity already
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Configure Schema for Identity Framework
            base.OnModelCreating(builder);
            builder.Entity<UserRole>(userRole => 
            {
                // Defining cross table UserRole
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});
                // Many To Many Relationship between Users and Roles
                userRole.HasOne(ur => ur.Role)
                        .WithMany (r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();                
                userRole.HasOne(ur => ur.User)
                        .WithMany (r => r.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();
            });

            // Releations between the standard tables
            builder.Entity<Like>()
                .HasKey(k => new {k.LikerId, k.LikeeId});
            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>()
                .HasOne(u => u.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(u => u.LikerId)
                .OnDelete(DeleteBehavior.Restrict);            
            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}