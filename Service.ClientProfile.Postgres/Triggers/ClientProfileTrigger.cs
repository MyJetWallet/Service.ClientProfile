using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;

namespace Service.ClientProfile.Postgres.Triggers;

public class ClientProfileTrigger : IBeforeSaveTrigger<Domain.Models.ClientProfile>
{
    public Task BeforeSave(ITriggerContext<Domain.Models.ClientProfile> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added || context.ChangeType == ChangeType.Modified)
        {
            context.Entity.LastTs = DateTime.Now;
        }
        
        return Task.CompletedTask;
    }
}