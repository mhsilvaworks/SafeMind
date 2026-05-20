using Microsoft.EntityFrameworkCore;
using SafeMind.Domain; 

namespace SafeMind.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Mapeamento das Entidades para Tabelas no Banco
        public DbSet<User> Users { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ValidationDocument> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configuração da Entidade User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).HasColumnType("varchar(100)").IsRequired();
                entity.Property(e => e.Email).HasColumnType("varchar(150)").IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnType("varchar(255)").IsRequired();
            });

            // 2. Configuração da Entidade Forum e Relacionamento (1 para Muitos com User)
            modelBuilder.Entity<Forum>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasColumnType("varchar(100)").IsRequired();
                entity.Property(e => e.Description).HasColumnType("varchar(500)");
                
                // Indexando FK para evitar deadlocks
                entity.HasIndex(e => e.OwnerId); 
                
                // Relação FORUMS -> USERS
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.OwnerId);
            });

            // 3. Configuração da Entidade Post e Relacionamentos
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).HasColumnType("varchar(2000)").IsRequired();
                
                // Indexando FKs
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ForumId);

                // Relação POSTS -> USERS
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey(e => e.UserId);

                // Relação POSTS -> FORUMS
                entity.HasOne<Forum>()
                      .WithMany()
                      .HasForeignKey(e => e.ForumId);
            });

            // 4. Configuração da Entidade ValidationDocument (Relação 1 para 0..1 com User)
            modelBuilder.Entity<ValidationDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileUrl).HasColumnType("varchar(500)").IsRequired();

                // Indexando FK Exclusiva (Única)
                entity.HasIndex(e => e.UserId).IsUnique(); 

                // Relação DOCUMENTS -> USERS
                entity.HasOne<User>()
                      .WithOne()
                      .HasForeignKey<ValidationDocument>(e => e.UserId);
            });
        }
    }
}