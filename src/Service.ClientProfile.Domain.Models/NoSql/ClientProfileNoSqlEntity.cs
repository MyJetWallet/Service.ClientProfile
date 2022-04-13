
using MyNoSqlServer.Abstractions;

namespace Service.ClientProfile.Domain.Models.NoSql
{
    public class ClientProfileNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-clientprofile";

        public static string GeneratePartitionKey() => "ClientProfiles";

        public static string GenerateRowKey(string clientId) => clientId;
        
        public ClientProfile ClientProfile { get; set; }

        public static ClientProfileNoSqlEntity Create(ClientProfile clientProfile) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(clientProfile.ClientId),
                ClientProfile = clientProfile
            };
    }
}