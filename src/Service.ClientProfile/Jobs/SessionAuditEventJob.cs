using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.ServiceBus.SessionAudit.Models;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.ClientProfile.Postgres;
using Service.ClientProfile.Services;

namespace Service.ClientProfile.Jobs
{
	public class SessionAuditEventJob
	{
		private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
		private readonly ProfileCacheManager _cache;
		private readonly ClientProfileService _clientProfileService;
		private readonly ILogger<SessionAuditEventJob> _logger;

		public SessionAuditEventJob(ISubscriber<SessionAuditEvent> subscriber, 
			DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder, 
			ProfileCacheManager cache, 
			ClientProfileService clientProfileService,
			ILogger<SessionAuditEventJob> logger)
		{
			_dbContextOptionsBuilder = dbContextOptionsBuilder;
			_cache = cache;
			_clientProfileService = clientProfileService;
			_logger = logger;
			subscriber.Subscribe(HandleEvents);
		}

		private async ValueTask HandleEvents(SessionAuditEvent message)
		{
			if (message.Action != SessionAuditEvent.SessionAction.Login)
				return;

			UserAgentInfo userAgentInfo = message.UserAgentInfo;
			string clientId = message.Session.TraderId;

			var result = await _clientProfileService.SetUserAgentDataAsync(
				clientId, userAgentInfo.DeviceOperationSystem, userAgentInfo.IsMobile);

			_logger.LogInformation("Updated user-agent info for clientId: {clientId}. Status: {status}. Info: {@agentInfo}", clientId, result.IsSuccess.ToString(), userAgentInfo);
		}
	}
}