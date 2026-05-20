using Microsoft.EntityFrameworkCore;
using SafeMind.Domain;

namespace SafeMind.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Forum> Forums { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<ValidationDocument> ValidationDocuments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator<TipoConta>("AccountType")
                .HasValue<UsuarioNeurodivergente>(TipoConta.Neurodivergente)
                .HasValue<Profissional>(TipoConta.Profissional)
                .HasValue<Empresa>(TipoConta.Empresa)
                .HasValue<Administrador>(TipoConta.Administrador);

            modelBuilder.Entity<Forum>()
                .HasOne(f => f.Owner)
                .WithMany(u => u.Forums)
                .HasForeignKey(f => f.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Forum)
                .WithMany(f => f.Posts)
                .HasForeignKey(p => p.ForumId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ValidationDocument>()
                .HasOne(d => d.User)
                .WithOne(u => u.ValidationDocument)
                .HasForeignKey<ValidationDocument>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}