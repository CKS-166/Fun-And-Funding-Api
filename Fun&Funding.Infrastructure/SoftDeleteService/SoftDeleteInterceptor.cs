﻿using Microsoft.EntityFrameworkCore.Diagnostics;
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
            return HandleSavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            return await HandleSavingChanges(eventData, result, cancellationToken);
        }

        private InterceptionResult<int> HandleSavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            if (eventData.Context == null)
            {
                Console.WriteLine("EventData.Context is null");
                return result;
            }

            // Log the entries in the ChangeTracker
            var entries = eventData.Context.ChangeTracker.Entries();
            Console.WriteLine($"Number of tracked entries: {entries.Count()}");

            foreach (var entry in entries)
            {
                // Log the entity type and state
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");

                if (entry is not { State: EntityState.Deleted, Entity: ISoftDelete delete }) continue;

                Console.WriteLine($"Soft deleting entity: {entry.Entity.GetType().Name}");

                entry.State = EntityState.Modified;
                delete.IsDeleted = true;
                delete.DeletedAt = DateTimeOffset.UtcNow;
            }

            return result;
        }

        private async ValueTask<InterceptionResult<int>> HandleSavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken)
        {
            if (eventData.Context == null)
            {
                Console.WriteLine("EventData.Context is null");
                return result;
            }

            // Log the entries in the ChangeTracker
            var entries = eventData.Context.ChangeTracker.Entries();
            Console.WriteLine($"Number of tracked entries: {entries.Count()}");

            foreach (var entry in entries)
            {
                // Log the entity type and state
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");

                if (entry is not { State: EntityState.Deleted, Entity: ISoftDelete delete }) continue;

                Console.WriteLine($"Soft deleting entity: {entry.Entity.GetType().Name}");

                entry.State = EntityState.Modified;
                delete.IsDeleted = true;
                delete.DeletedAt = DateTimeOffset.UtcNow;
            }

            return await Task.FromResult(result);
        }
    }



}
