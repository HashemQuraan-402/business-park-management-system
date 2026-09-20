using BusinessParkCompaniesManagement.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace BusinessParkCompaniesManagement.Server.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Sector> Sectors => Set<Sector>();
        public DbSet<ComplaintType> ComplaintTypes => Set<ComplaintType>();

        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>()
                .HasIndex(c => c.Username)
                .IsUnique()
                .HasFilter("[Username] IS NOT NULL");

            modelBuilder.Entity<Company>()
                .HasOne(c => c.Sector)
                .WithMany(s => s.Companies)
                .HasForeignKey(c => c.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasOne(c => c.CreatedByAdminUser)
                .WithMany(a => a.Companies)
                .HasForeignKey(c => c.CreatedByAdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(co => co.Company)
                .WithMany(c => c.Complaints)
                .HasForeignKey(co => co.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Complaint>()
                .HasOne(co => co.ComplaintType)
                .WithMany(ct => ct.Complaints)
                .HasForeignKey(co => co.ComplaintTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminUser>()
                .HasIndex(c => c.Username)
                .IsUnique();

            modelBuilder.Entity<Company>()
                .Property(c => c.Size)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Complaint>()
                .Property(c => c.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Complaint>()
                .Property(c => c.Priority)
                .HasConversion<string>()
                .HasMaxLength(20);

            // ----- Seed data: Sectors -----
            modelBuilder.Entity<Sector>().HasData(
              new Sector { Id = 1, NameEn = "Information Technology", NameAr = "تكنولوجيا المعلومات" },
              new Sector { Id = 2, NameEn = "Telecommunications", NameAr = "الاتصالات" },
              new Sector { Id = 3, NameEn = "Trade & Retail", NameAr = "التجارة والبيع بالتجزئة" },
              new Sector { Id = 4, NameEn = "Manufacturing", NameAr = "الصناعة" },
              new Sector { Id = 5, NameEn = "Financial Services", NameAr = "الخدمات المالية" },
              new Sector { Id = 6, NameEn = "Healthcare", NameAr = "الرعاية الصحية" },
              new Sector { Id = 7, NameEn = "Education", NameAr = "التعليم" },
              new Sector { Id = 8, NameEn = "Logistics & Transportation", NameAr = "الخدمات اللوجستية والنقل" },
              new Sector { Id = 9, NameEn = "Consulting Services", NameAr = "الخدمات الاستشارية" },
              new Sector { Id = 10, NameEn = "Other", NameAr = "أخرى" }
          );


            // ----- Seed data: 10 Complaint Types (dropdown list) -----
            modelBuilder.Entity<ComplaintType>().HasData(
                new ComplaintType { Id = 1, NameEn = "Maintenance Issue", NameAr = "مشكلة صيانة" },
                new ComplaintType { Id = 2, NameEn = "Internet / Network Issue", NameAr = "مشكلة إنترنت / شبكة" },
                new ComplaintType { Id = 3, NameEn = "Electricity Issue", NameAr = "مشكلة كهرباء" },
                new ComplaintType { Id = 4, NameEn = "Water Supply Issue", NameAr = "مشكلة إمدادات المياه" },
                new ComplaintType { Id = 5, NameEn = "HVAC / Air Conditioning Issue", NameAr = "مشكلة تكييف الهواء" },
                new ComplaintType { Id = 6, NameEn = "Parking Issue", NameAr = "مشكلة مواقف السيارات" },
                new ComplaintType { Id = 7, NameEn = "Cleanliness & Sanitation", NameAr = "النظافة والصرف الصحي" },
                new ComplaintType { Id = 8, NameEn = "Security Concern", NameAr = "مخاوف أمنية" },
                new ComplaintType { Id = 9, NameEn = "Noise Complaint", NameAr = "شكوى ضوضاء" },
                new ComplaintType { Id = 10, NameEn = "General Suggestion for Improvement", NameAr = "اقتراح عام للتحسين" }
            );

        }
    }
}
