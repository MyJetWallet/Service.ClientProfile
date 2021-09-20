using System.Collections.Generic;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.ClientProfile.Domain.Models.NoSql;

namespace Service.ClientProfile.Services
{
    
    public class ProfileCacheManager
    {
        private readonly IMyNoSqlServerDataWriter<ClientProfileNoSqlEntity> _dataWriter;

        public ProfileCacheManager(IMyNoSqlServerDataWriter<ClientProfileNoSqlEntity> dataWriter)
        {
            _dataWriter = dataWriter;
        }

        public async Task AddOrUpdateClientProfile(Domain.Models.ClientProfile profile)
        {
            await _dataWriter.InsertOrReplaceAsync(ClientProfileNoSqlEntity.Create(profile));
            await _dataWriter.CleanAndKeepLastRecordsAsync(ClientProfileNoSqlEntity.GeneratePartitionKey(),
                Program.Settings.MaxCachedEntities);
        }

        public async Task RemoveRange(List<Domain.Models.ClientProfile> profiles)
        {
            var tasks = new List<Task>();
            foreach (var profile in profiles)
            {
                tasks.Add(_dataWriter.DeleteAsync(ClientProfileNoSqlEntity.GeneratePartitionKey(),
                    ClientProfileNoSqlEntity.GenerateRowKey(profile.ClientId)).AsTask());
            }
            await Task.WhenAll(tasks);
        }
    }
}