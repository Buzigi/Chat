using Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.DAL
{
    public class ChatContext : DbContext
    {
        public ChatContext() : base("ChatDB")
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Configure user_details table in DB
            modelBuilder.Entity<User>().ToTable("user_details");
            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .IsRequired();

            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();


            //Configure conversations table in DB
            modelBuilder.Entity<Message>().ToTable("conversations");
            modelBuilder.Entity<Message>().Property(m => m.Text).HasMaxLength(150);

        }

    }
}
