using System.Data.Entity;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ArtGalleryApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Order> Orders { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USER - ARTWORK
            // Cart
            modelBuilder.Entity<User>()
                .HasMany(u => u.Cart)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("UserCart");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("ArtworkId");
                });

            // Favorite artworks
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteArtworks)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("UserFavoriteArtworks");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("ArtworkId");
                });

            // Purchased artworks
            modelBuilder.Entity<User>()
                .HasMany(u => u.PurchasedArtworks)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("UserPurchasedArtworks");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("ArtworkId");
                });

            // USER - ARTIST

            modelBuilder.Entity<User>()
                .HasMany(u => u.FollowedArtists)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("UserFollowedArtists");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("ArtistId");
                });

            // ARTIST - ARTWORK

            modelBuilder.Entity<Artwork>()
                .HasOptional(a => a.Artist)
                .WithMany(ar => ar.Artworks)
                .HasForeignKey(a => a.ArtistId)
                .WillCascadeOnDelete(false);

            // ARTIST - EVENT

            modelBuilder.Entity<Event>()
                .HasOptional(e => e.Artist)
                .WithMany(a => a.Events)
                .HasForeignKey(e => e.ArtistId)
                .WillCascadeOnDelete(false);


            // USER - ORDER

            modelBuilder.Entity<Order>()
                .HasOptional(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);

            // Order - Artworks many-to-many
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Artworks)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("OrderArtworks");
                    m.MapLeftKey("OrderId");
                    m.MapRightKey("ArtworkId");
                });

            // USER - TICKET

            modelBuilder.Entity<Ticket>()
                .HasOptional(t => t.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.UserId)
                .WillCascadeOnDelete(false);

            // EVENT - TICKET

            modelBuilder.Entity<Ticket>()
                .HasRequired(t => t.Event)
                .WithMany()
                .HasForeignKey(t => t.EventId)
                .WillCascadeOnDelete(true);

            // ORDER - OrderArtwork

            modelBuilder.Entity<OrderArtwork>()
                .HasRequired(oa => oa.Order)
                .WithMany(o => o.OrderArtworks)
                .HasForeignKey(oa => oa.OrderId)
                .WillCascadeOnDelete(true);
        }
    }
}