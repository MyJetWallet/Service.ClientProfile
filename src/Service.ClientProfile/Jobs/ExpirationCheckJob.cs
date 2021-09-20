using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.ClientProfile.Domain.Models.NoSql;
using Service.ClientProfile.Postgres;
using Service.ClientProfile.Services;

namespace Service.ClientProfile.Jobs
{
    
    public class ExpirationCheckJob: IDisposable
    {
        private readonly MyTaskTimer _timer;
        private readonly ILogger<ExpirationCheckJob> _logger;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly ProfileCacheManager _cache;

        public ExpirationCheckJob(ILogger<ExpirationCheckJob> logger, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ProfileCacheManager cache)
        {
            _logger = logger;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _cache = cache;
            _timer = new MyTaskTimer(typeof(ExpirationCheckJob),
                TimeSpan.FromMinutes(Program.ReloadedSettings(e => e.ExpirationCheckTimerInMin).Invoke()),
                logger, DoTime);
        }

        private async Task DoTime()
        {
            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
            
            var updatedProfiles = new List<Domain.Models.ClientProfile>();
            var profiles = context.ClientProfiles.Include(t=>t.Blockers).ToList();
            foreach (var profile in profiles)
            {
                var expiredBlockers = profile.Blockers?.RemoveAll(t => t.ExpiryTime < DateTime.UtcNow);
                if (expiredBlockers != 0)
                {
                    updatedProfiles.Add(profile);
                }
            }

            await context.SaveChangesAsync();
            await _cache.RemoveRange(updatedProfiles);
        }
        
        public void Dispose()
        {
            _timer?.Dispose();
        }
        
        public void Start()
        {
            _timer.Start();
        }
    }
}