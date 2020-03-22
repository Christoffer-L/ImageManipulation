using ImageManipulationApi.Database;
using ImageManipulationApi.Entities;
using ImageManipulationApi.Hubs;
using ImageManipulationApi.Repository;
using ImageManipulationApi.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ImageManipulationTests
{
    public class DatabaseTest
    {
        private readonly UserManipulatedImage _Image1;
        private readonly UserManipulatedImage _Image2;

        public DatabaseTest() 
        {
            _Image1 = new UserManipulatedImage { Id = "123", EncryptedImage = new byte[10] };
            _Image2 = new UserManipulatedImage { Id = "321", EncryptedImage = new byte[5] };
        }

        [Fact]
        public async Task AddAsyncCorrectlySavesObjectsInDatabase()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApiDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new ApiDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                // Run the test against one instance of the context
                using (var context = new ApiDbContext(options))
                {
                    var service = new ImageManipulationRepository(context);
                    
                    await service.AddAsync(_Image1);
                    await service.AddAsync(_Image2);
                    await context.SaveChangesAsync();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new ApiDbContext(options))
                {
                    // Assert that we added ONLY 2 objects to the db by counting
                    Assert.Equal(2, await context.UserManipulatedImages.CountAsync());
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task GetAsyncReturnsValidDatabaseObjectData()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApiDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new ApiDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                // Run the test against one instance of the context
                using (var context = new ApiDbContext(options))
                {
                    var service = new ImageManipulationRepository(context);
                    
                    await service.AddAsync(_Image1);
                    await service.AddAsync(_Image2);
                    await context.SaveChangesAsync();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new ApiDbContext(options))
                {
                    var service = new ImageManipulationRepository(context);

                    // Assert that the class proprieties have CORRECT values
                    Assert.Equal("123", (await service.GetAsync("123") as UserManipulatedImage).Id);
                    Assert.Equal(5, (await service.GetAsync("321") as UserManipulatedImage).EncryptedImage.Length);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task GetAsyncWithInvalidParametersReturnsNullObject()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<ApiDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var context = new ApiDbContext(options))
                {
                    context.Database.EnsureCreated();
                }

                // Run the test against one instance of the context
                using (var context = new ApiDbContext(options))
                {
                    var service = new ImageManipulationRepository(context);

                    await service.AddAsync(_Image1);
                    await service.AddAsync(_Image2);
                    await context.SaveChangesAsync();
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var context = new ApiDbContext(options))
                {
                    var service = new ImageManipulationRepository(context);

                    // Invalid PK return NULL object
                    Assert.Null(await service.GetAsync("132"));
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
