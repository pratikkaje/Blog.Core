﻿using System.Threading.Tasks;
using Blog.Core.Models.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Blog.Core.Brokers.Storages
{
    public partial class StorageBroker : IStorageBroker
    {
        public DbSet<Profile> Profiles { get; set; }

        public async ValueTask<Profile> InsertProfileAsync(Profile profile)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Profile> profileEntityEntry =
                await broker.Profiles.AddAsync(profile);

            await broker.SaveChangesAsync();

            return profileEntityEntry.Entity;
        }
    }
}
