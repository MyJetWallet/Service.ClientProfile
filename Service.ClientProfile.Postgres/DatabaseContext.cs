using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.ClientProfile.Domain.Models;

namespace Service.ClientProfile.Postgres
{
    public class DatabaseContext : DbContext
    {
        public const string Schema = "clientprofiles";

        public const string ProfilesTableName = "profiles";
        public const string BlockerTableName = "blockers";

        public DbSet<Domain.Models.ClientProfile> ClientProfiles { get; set; }
        public DbSet<Blocker> Blockers { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public static ILoggerFactory LoggerFactory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<Domain.Models.ClientProfile>().ToTable(ProfilesTableName);
            modelBuilder.Entity<Domain.Models.ClientProfile>().HasKey(e => e.ClientId);
            
            modelBuilder.Entity<Domain.Models.ClientProfile>().HasIndex(e => e.ClientId);
            modelBuilder.Entity<Domain.Models.ClientProfile>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<Domain.Models.ClientProfile>().Property(e => e.Status2FA).HasDefaultValue(Status2FA.NotSet);
            
            modelBuilder.Entity<Blocker>().ToTable(BlockerTableName);
            modelBuilder.Entity<Blocker>().HasKey(e => e.BlockerId);
            modelBuilder.Entity<Blocker>().Property(e => e.BlockerId).UseIdentityColumn();
            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> UpsetAsync(IEnumerable<Domain.Models.ClientProfile> entities)
        {
            var result = await ClientProfiles.UpsertRange(entities).On(e => e.ClientId).NoUpdate().RunAsync();
            return result;
        }

        
    }
}
