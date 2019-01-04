namespace E_Hospital.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class System_Models : DbContext
    {
        public System_Models()
            : base("name=System_Models")
        {
        }

        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>()
                .Property(e => e.FirstName)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.LastName)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.Gender)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .Property(e => e.HospitalId)
                .IsUnicode(false);

            modelBuilder.Entity<Doctor>()
                .HasMany(e => e.Reservations)
                .WithRequired(e => e.Doctor)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Hospital>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Hospital>()
                .Property(e => e.HospitalName)
                .IsUnicode(false);

            modelBuilder.Entity<Hospital>()
                .Property(e => e.Latitude)
                .HasPrecision(10, 6);

            modelBuilder.Entity<Hospital>()
                .Property(e => e.Longitude)
                .HasPrecision(11, 6);

            modelBuilder.Entity<Hospital>()
                .HasMany(e => e.Doctors)
                .WithRequired(e => e.Hospital)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Reservation>()
                .Property(e => e.Status)
                .IsUnicode(false);
        }
    }
}
