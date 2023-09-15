using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace XRayHub.Models
{
    public partial class XRayHub_Models : DbContext
    {
        public XRayHub_Models()
            : base("name=XRayHub_Models")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Innovation> Innovations { get; set; }
        public virtual DbSet<MedicalPractitioner> MedicalPractitioners { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<XrayRecord> XrayRecords { get; set; }
        public virtual DbSet<XrayType> XrayTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.MedicalPractitioners)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.IdentityUserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.Patients)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.IdentityUserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MedicalPractitioner>()
                .HasMany(e => e.Appointments)
                .WithRequired(e => e.MedicalPractitioner)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patient>()
                .HasMany(e => e.Appointments)
                .WithRequired(e => e.Patient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patient>()
                .HasMany(e => e.Feedbacks)
                .WithRequired(e => e.Patient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Patient>()
                .HasMany(e => e.XrayRecords)
                .WithRequired(e => e.Patient)
                .WillCascadeOnDelete(false);
        }
    }
}
