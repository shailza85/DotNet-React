using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MVC_4Point1.Models;

namespace MVC_4Point1.Models
{

    public partial class PersonContext : DbContext
    {
        public PersonContext()
        {
        }

        public PersonContext(DbContextOptions<PersonContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;database=mvc_person", x => x.ServerVersion("10.4.14-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.LastName)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

               entity.HasData(
                    new Person()
                    {
                        ID = -1,
                        FirstName = "John",
                        LastName = "Doe"
                    },
                    new Person()
                    {
                        ID = -2,
                        FirstName = "Jane",
                        LastName = "Doe"
                    },
                    new Person()
                    {
                        ID = -3,
                        FirstName = "Todd",
                        LastName = "Smith"
                    },
                    new Person()
                    {
                        ID = -4,
                        FirstName = "Sue",
                        LastName = "Smith"
                    },
                    new Person()
                    {
                        ID = -5,
                        FirstName = "Joe",
                        LastName = "Smithserson"
                    }
                );
            });

            modelBuilder.Entity<PhoneNumber>(entity =>
            {
                entity.HasIndex(e => e.PersonID)
                    .HasName("FK_" + nameof(PhoneNumber) + "_" + nameof(Person));

                entity.Property(e => e.Number)
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                // Always in the one with the foreign key.
                entity.HasOne(child => child.Person)
                    .WithMany(parent => parent.PhoneNumbers)
                    .HasForeignKey(child => child.PersonID)
                    // When we delete a record, we can modify the behaviour of the case where there are child records.
                    // Restrict: Throw an exception if we try to orphan a child record.
                    // Cascade: Remove any child records that would be orphaned by a removed parent.
                    // SetNull: Set the foreign key field to null on any orphaned child records.
                    // NoAction: Don't commit any deletions of parents which would orphan a child.
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_" + nameof(PhoneNumber) + "_" + nameof(Person));

               entity.HasData(
                        new PhoneNumber()
                        {
                            ID = -1,
                            Number = "800-234-4567",
                            PersonID = -1
                        },
                        new PhoneNumber()
                        {
                            ID = -2,
                            Number = "800-234-4567",
                            PersonID = -2
                        },
                        new PhoneNumber()
                        {
                            ID = -3,
                            Number = "800-345-5678",
                            PersonID = -2
                        },
                        new PhoneNumber()
                        {
                            ID = -4,
                            Number = "800-456-6789",
                            PersonID = -3
                        },
                        new PhoneNumber()
                        {
                            ID = -5,
                            Number = "800-987-7654",
                            PersonID = -4
                        },
                        new PhoneNumber()
                        {
                            ID = -6,
                            Number = "800-876-6543",
                            PersonID = -5
                        },
                        new PhoneNumber()
                        {
                            ID = -7,
                            Number = "800-765-5432",
                            PersonID = -5
                        }
                    );
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
