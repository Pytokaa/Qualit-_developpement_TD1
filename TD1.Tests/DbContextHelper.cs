using System;
using Microsoft.EntityFrameworkCore;
using TD1.Models.EntityFramework;

namespace TD1.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static AppDbContext CreateInMemoryContext(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);
            return context;
        }
    }
}