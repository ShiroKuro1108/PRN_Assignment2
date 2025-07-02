using Microsoft.EntityFrameworkCore;
using QuangThienDung.DataAccess.Models;

namespace QuangThienDung.DataAccess.Data
{
    public class FUNewsManagementContext : DbContext
    {
        public FUNewsManagementContext(DbContextOptions<FUNewsManagementContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<NewsTag> NewsTags { get; set; }
        public DbSet<SystemAccount> SystemAccounts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names to match database
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<NewsArticle>().ToTable("NewsArticle");
            modelBuilder.Entity<NewsTag>().ToTable("NewsTag");
            modelBuilder.Entity<SystemAccount>().ToTable("SystemAccount");
            modelBuilder.Entity<Tag>().ToTable("Tag");

            // Configure composite primary key for NewsTag
            modelBuilder.Entity<NewsTag>()
                .HasKey(nt => new { nt.NewsArticleID, nt.TagID });

            // Configure relationships
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NewsArticle>()
                .HasOne(na => na.Category)
                .WithMany(c => c.NewsArticles)
                .HasForeignKey(na => na.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NewsArticle>()
                .HasOne(na => na.CreatedBy)
                .WithMany(sa => sa.CreatedNewsArticles)
                .HasForeignKey(na => na.CreatedByID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NewsArticle>()
                .HasOne(na => na.UpdatedBy)
                .WithMany(sa => sa.UpdatedNewsArticles)
                .HasForeignKey(na => na.UpdatedByID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.NewsArticle)
                .WithMany(na => na.NewsTags)
                .HasForeignKey(nt => nt.NewsArticleID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NewsTag>()
                .HasOne(nt => nt.Tag)
                .WithMany(t => t.NewsTags)
                .HasForeignKey(nt => nt.TagID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
