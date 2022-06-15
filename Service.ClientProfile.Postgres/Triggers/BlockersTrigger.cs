using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered;
using Service.ClientProfile.Domain.Models;

namespace Service.ClientProfile.Postgres.Triggers;

public class BlockersTrigger : IBeforeSaveTrigger<Blocker>
{
    
    public Task BeforeSave(ITriggerContext<Blocker> context, CancellationToken cancellationToken)
    {
        if (context.ChangeType == ChangeType.Added | context.ChangeType == ChangeType.Modified)
        {
            context.Entity.LastTs = DateTime.Now;
        }
        
        return Task.CompletedTask;
    }
}