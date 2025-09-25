using HarmanKnowledgeHubPortal.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmanKnowledgeHubPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        //public DbSet<Report> Reports { get; set; }
        public DbSet<Rating> Ratings { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique Email for Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // User ↔ Role many-to-many (skip navigation is fine here)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany();

            // Article ↔ ArticleTag ↔ Tag many-to-many (explicit join)
            modelBuilder.Entity<ArticleTag>()
                .HasKey(at => new { at.ArticleId, at.TagId });

            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Article)
                .WithMany(a => a.ArticleTags)
                .HasForeignKey(at => at.ArticleId);

            modelBuilder.Entity<ArticleTag>()
                .HasOne(at => at.Tag)
                .WithMany(t => t.ArticleTags)
                .HasForeignKey(at => at.TagId);

            // Article ↔ Category (1-to-many)
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles) // make sure Category has ICollection<Article>
                .HasForeignKey(a => a.CategoryId);

            // Soft delete filter on Category
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }

    }
}
