using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fun_Funding.Domain.Entity;

namespace Fun_Funding.Infrastructure.SoftDeleteService
{

    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context == null) return result;

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                // Log entity and state for debugging purposes
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");

                if (entry is not { State: EntityState.Deleted, Entity: ISoftDelete delete }) continue;

                Console.WriteLine($"Soft deleting entity: {entry.Entity.GetType().Name}");

                entry.State = EntityState.Modified;
                delete.IsDeleted = true;
                delete.DeletedAt = DateTimeOffset.UtcNow;
            }

            return result;
        }
    }

}
