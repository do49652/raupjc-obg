using System.Data;
using System.Data.Entity;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Data
{
    public class GameDbContext : DbContext
    {
        public IDbSet<GameModel> Games { get; set; }
        public IDbSet<EventModel> Events { get; set; }
        public IDbSet<ItemModel> Items { get; set; }

        public GameDbContext(string cnnstr) : base(cnnstr)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GameModel>().HasKey(g => g.Id);
            modelBuilder.Entity<GameModel>().Property(g => g.UserId).IsRequired();
            modelBuilder.Entity<GameModel>().Property(g => g.Name).IsRequired();
            modelBuilder.Entity<GameModel>().Property(g => g.Description);
            modelBuilder.Entity<GameModel>().Property(g => g.Private);
            modelBuilder.Entity<GameModel>().Property(g => g.Standalone);
            modelBuilder.Entity<GameModel>().Property(g => g.StartingMoney);
            modelBuilder.Entity<GameModel>().Property(g => g.MiniEvents);
            modelBuilder.Entity<GameModel>().Property(g => g.SetEvents);
            modelBuilder.Entity<GameModel>().HasMany(g => g.Dependencies);
            modelBuilder.Entity<GameModel>().HasMany(g => g.Events).WithRequired(e => e.Game);
            modelBuilder.Entity<GameModel>().HasMany(g => g.Items).WithRequired(i => i.Game);

            modelBuilder.Entity<EventModel>().HasKey(e => e.Id);
            modelBuilder.Entity<EventModel>().Property(e => e.UserId).IsRequired();
            modelBuilder.Entity<EventModel>().Property(e => e.Name).IsRequired();
            modelBuilder.Entity<EventModel>().Property(e => e.Description);
            modelBuilder.Entity<EventModel>().Property(e => e.HappensOnce);
            modelBuilder.Entity<EventModel>().Property(e => e.Repeat);
            modelBuilder.Entity<EventModel>().Property(e => e.Behaviour);
            modelBuilder.Entity<EventModel>().Property(e => e.Items);
            modelBuilder.Entity<EventModel>().HasRequired(e => e.Game).WithMany(g => g.Events);
            modelBuilder.Entity<EventModel>().HasOptional(e => e.NextEvent);

            modelBuilder.Entity<ItemModel>().HasKey(i => i.Id);
            modelBuilder.Entity<ItemModel>().Property(i => i.UserId).IsRequired();
            modelBuilder.Entity<ItemModel>().Property(i => i.Name).IsRequired();
            modelBuilder.Entity<ItemModel>().Property(i => i.Description);
            modelBuilder.Entity<ItemModel>().Property(i => i.Category);
            modelBuilder.Entity<ItemModel>().Property(i => i.Behaviour);
            modelBuilder.Entity<ItemModel>().HasRequired(i => i.Game).WithMany(g => g.Items);

        }
    }
}