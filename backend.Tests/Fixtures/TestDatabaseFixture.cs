using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace backend.Tests.Fixtures;

public class TestDatabaseFixture : IDisposable
{
    private readonly DbContextOptions<MarketplaceContext> _options;

    public TestDatabaseFixture()
    {
        _options = new DbContextOptionsBuilder<MarketplaceContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        SeedTestData();
    }

    public MarketplaceContext CreateContext() => new MarketplaceContext(_options);

    private void SeedTestData()
    {
        using var context = new MarketplaceContext(_options);
        context.Database.EnsureCreated();

        // Seed Categories
        var categories = new[]
        {
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Books" },
            new Category { Id = 3, Name = "Clothing" }
        };
        context.Categories.AddRange(categories);

        // Seed Products
        var products = new[]
        {
            new Product 
            { 
                Id = 1, 
                Name = "Wireless Headphones", 
                Description = "Noise-cancelling headphones", 
                Price = 149.99m,
                CategoryId = 1,
                ImageUrl = "https://placehold.co/400x300?text=Headphones",
                CreatedAt = DateTime.UtcNow
            },
            new Product 
            { 
                Id = 2, 
                Name = "Mechanical Keyboard", 
                Description = "RGB keyboard", 
                Price = 89.99m,
                CategoryId = 1,
                ImageUrl = "https://placehold.co/400x300?text=Keyboard",
                CreatedAt = DateTime.UtcNow
            },
            new Product 
            { 
                Id = 3, 
                Name = "Clean Code", 
                Description = "Software craftsmanship book", 
                Price = 34.99m,
                CategoryId = 2,
                ImageUrl = "https://placehold.co/400x300?text=CleanCode",
                CreatedAt = DateTime.UtcNow
            },
            new Product 
            { 
                Id = 4, 
                Name = "Classic Hoodie", 
                Description = "Comfortable hoodie", 
                Price = 49.99m,
                CategoryId = 3,
                ImageUrl = "https://placehold.co/400x300?text=Hoodie",
                CreatedAt = DateTime.UtcNow
            }
        };
        context.Products.AddRange(products);

        // Seed Users for integration tests
        var hasher = new PasswordHasher<User>();
        var users = new[]
        {
            new User 
            { 
                Id = 1, 
                Username = "userA", 
                PasswordHash = hasher.HashPassword(null, "password"),
                Role = "Customer"
            },
            new User 
            { 
                Id = 2, 
                Username = "userB", 
                PasswordHash = hasher.HashPassword(null, "password"),
                Role = "Customer"
            }
        };
        context.Users.AddRange(users);

        context.SaveChanges();
    }

    public void Dispose()
    {
        using var context = new MarketplaceContext(_options);
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}
