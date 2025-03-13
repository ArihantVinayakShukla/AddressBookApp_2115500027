using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Context
{
    public class AddressBookContext : DbContext
    {
        public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options)
        {
        }

        public DbSet<UserEntry> Users { get; set; }
        public DbSet<AddressBookEntry> AddressBooks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User-AddressBook One-to-Many Relationship
            modelBuilder.Entity<AddressBookEntry>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, delete their contacts

            base.OnModelCreating(modelBuilder);
        }
    }
}
