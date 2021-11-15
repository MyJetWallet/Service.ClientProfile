using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Postgres;
using Service.ClientProfile.Domain.Models;

namespace Service.ClientProfile.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "clientprofiles";

        public const string ProfilesTableName = "profiles";
        public const string BlockerTableName = "blockers";

        public DbSet<Domain.Models.ClientProfile> ClientProfiles { get; set; }
        public DbSet<Blocker> Blockers { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.Entity<Domain.Models.ClientProfile>().ToTable(ProfilesTableName);
            modelBuilder.Entity<Domain.Models.ClientProfile>().HasKey(e => e.ClientId);
            
            modelBuilder.Entity<Domain.Models.ClientProfile>().HasIndex(e => e.ClientId);
            modelBuilder.Entity<Domain.Models.ClientProfile>().Property(e => e.ClientId).HasMaxLength(128);
            modelBuilder.Entity<Domain.Models.ClientProfile>().Property(e => e.Status2FA).HasDefaultValue(Status2FA.NotSet);
            modelBuilder.Entity<Domain.Models.ClientProfile>().Property(e => e.KYCPassed).HasDefaultValue(false);
            modelBuilder.Entity<Domain.Models.ClientProfile>().Property(e => e.ReferralCode).HasMaxLength(128);
            modelBuilder.Entity<Domain.Models.ClientProfile>().Property(e => e.ReferrerClientId).HasMaxLength(128);
            
            
            modelBuilder.Entity<Domain.Models.ClientProfile>().HasIndex(e => e.ReferrerClientId);
            modelBuilder.Entity<Domain.Models.ClientProfile>().HasIndex(e => e.ReferralCode);

            modelBuilder.Entity<Domain.Models.ClientProfile>().HasIndex(e => e.LastTs);

            modelBuilder.Entity<Blocker>().ToTable(BlockerTableName);
            modelBuilder.Entity<Blocker>().HasKey(e => e.BlockerId);
            modelBuilder.Entity<Blocker>().Property(e => e.BlockerId).UseIdentityColumn();
            modelBuilder.Entity<Blocker>().HasIndex(e => e.LastTs);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> UpsertAsync(IEnumerable<Domain.Models.ClientProfile> entities)
        {
            var result = await ClientProfiles.UpsertRange(entities).WhenMatched((oldEntity, newEntity) => new Domain.Models.ClientProfile
            {
                ClientId = newEntity.ClientId,
                Status2FA = newEntity.Status2FA,
                EmailConfirmed = newEntity.EmailConfirmed,
                PhoneConfirmed = newEntity.PhoneConfirmed,
                KYCPassed = newEntity.KYCPassed,
                ReferralCode = newEntity.ReferralCode,
                ReferrerClientId = newEntity.ReferrerClientId
            }).RunAsync();
            return result;
        }
        
        public async Task<int> UpsertAsync(Domain.Models.ClientProfile entity)
        {
            var result = await ClientProfiles.Upsert(entity).On(e => e.ClientId).AllowIdentityMatch().RunAsync();
            return result;
        }

        
    }
}
