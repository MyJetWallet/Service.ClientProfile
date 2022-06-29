using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.ServiceBus.SessionAudit.Models;
using Service.ClientProfile.Postgres;
using Service.ClientProfile.Services;

namespace Service.ClientProfile.Jobs
{
	public class SessionAuditEventJob
	{
		private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
		private readonly ProfileCacheManager _cache;
		private readonly ILogger<SessionAuditEventJob> _logger;

		public SessionAuditEventJob(ISubscriber<SessionAuditEvent> subscriber, 
			DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, ProfileCacheManager cache, ILogger<SessionAuditEventJob> logger)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
			_cache = cache;
			_logger = logger;
			subscriber.Subscribe(HandleEvents);
		}

		private async ValueTask HandleEvents(SessionAuditEvent message)
		{
			if (message.Action != SessionAuditEvent.SessionAction.Login)
				return;

			UserAgentInfo userAgentInfo = message.UserAgentInfo;
			string clientId = message.Session.TraderId;

			await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

			Domain.Models.ClientProfile clientProfile = await context.ClientProfiles.FirstOrDefaultAsync(p => p.ClientId == clientId);
			if (clientProfile == null)
			{
				_logger.LogError("Can't find ClientProfile for clientId: {clientId}", clientId);
				return;
			}

			clientProfile.DeviceOperationSystem = userAgentInfo.DeviceOperationSystem;
			clientProfile.IsMobile = userAgentInfo.IsMobile;

			if (clientProfile.IsMobile == userAgentInfo.IsMobile && clientProfile.DeviceOperationSystem == userAgentInfo.DeviceOperationSystem)
				return;

			context.ClientProfiles.Update(clientProfile);
			await context.SaveChangesAsync();

			_logger.LogInformation("Updated user-agent info for clientId: {clientId}, {@agentInfo}", clientId, userAgentInfo);

			await _cache.AddOrUpdateClientProfile(clientProfile);
		}
	}
}