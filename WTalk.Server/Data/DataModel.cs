namespace WTalk.Server.Data
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataModel : DbContext
    {
        public DataModel()
            : base("name=DataModel3")
        {
        }

        public virtual DbSet<addfriend> addfriends { get; set; }
        public virtual DbSet<friend> friends { get; set; }
        public virtual DbSet<presenceuser> presenceusers { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<waitforsending> waitforsendings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<addfriend>()
                .Property(e => e.TIme)
                .IsUnicode(false);

            modelBuilder.Entity<addfriend>()
                .Property(e => e.SenderId)
                .IsUnicode(false);

            modelBuilder.Entity<addfriend>()
                .Property(e => e.ReceiverId)
                .IsUnicode(false);

            modelBuilder.Entity<friend>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<friend>()
                .Property(e => e.FriendId)
                .IsUnicode(false);

            modelBuilder.Entity<presenceuser>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<presenceuser>()
                .Property(e => e.PresenceTime)
                .IsUnicode(false);

            modelBuilder.Entity<presenceuser>()
                .Property(e => e.IPAddress)
                .IsUnicode(false);

            modelBuilder.Entity<presenceuser>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<user>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<waitforsending>()
                .Property(e => e.MsgId)
                .IsUnicode(false);

            modelBuilder.Entity<waitforsending>()
                .Property(e => e.SendTime)
                .IsUnicode(false);

            modelBuilder.Entity<waitforsending>()
                .Property(e => e.SenderId)
                .IsUnicode(false);

            modelBuilder.Entity<waitforsending>()
                .Property(e => e.ReceiverId)
                .IsUnicode(false);

            modelBuilder.Entity<waitforsending>()
                .Property(e => e.Msg)
                .IsUnicode(false);
        }
    }
}
