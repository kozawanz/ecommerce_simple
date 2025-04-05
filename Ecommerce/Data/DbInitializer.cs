using eCommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerce.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, ILogger logger)
        {
            logger.LogInformation("Starting database initialization...");

            try
            {
                // Seed categories and products
                await SeedCategoriesAndProducts(serviceProvider, logger);

                // Seed roles and users
                await SeedRolesAndUsers(serviceProvider, logger);

                logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during database initialization.");
                throw; // Re-throw to see the error
            }
        }

        private static async Task SeedCategoriesAndProducts(IServiceProvider serviceProvider, ILogger logger)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            logger.LogInformation("Checking for existing categories...");

            // Check if categories already exist
            if (!context.Categories.Any())
            {
                logger.LogInformation("No categories found. Adding categories...");

                // Add categories
                context.Categories.AddRange(
                    new Category {  Name = "Electronics", Description = "Electronic items like phones, laptops, etc." },
                    new Category {  Name = "Clothing", Description = "Clothing items for men, women, and children" },
                    new Category {  Name = "Books", Description = "Books of various genres" }
                );
                await context.SaveChangesAsync();

                logger.LogInformation("Categories added successfully.");
            }
            else
            {
                logger.LogInformation("Categories already exist. Skipping category seeding.");
            }

            logger.LogInformation("Checking for existing products...");

            // Check if products already exist
            if (!context.Products.Any())
            {
                logger.LogInformation("No products found. Adding products...");

                // Add products
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Smartphone",
                        Description = "Latest smartphone with advanced features",
                        Price = 699.99m,
                        StockQuantity = 50,
                        CategoryId = 1,
                        ImageUrl = "https://placehold.co/400?text=SmartPhone&font=roboto",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Laptop",
                        Description = "High-performance laptop for work and gaming",
                        Price = 1299.99m,
                        StockQuantity = 30,
                        CategoryId = 1,
                        ImageUrl = "https://placehold.co/400?text=Laptop&font=roboto",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "T-Shirt",
                        Description = "Comfortable cotton t-shirt",
                        Price = 19.99m,
                        StockQuantity = 100,
                        CategoryId = 2,
                        ImageUrl = "https://placehold.co/400?text=TShirt&font=roboto",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Novel",
                        Description = "Bestselling fiction novel",
                        Price = 14.99m,
                        StockQuantity = 75,
                        CategoryId = 3,
                        ImageUrl = "https://placehold.co/400?text=Novel&font=roboto",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                );
                await context.SaveChangesAsync();

                logger.LogInformation("Products added successfully.");
            }
            else
            {
                logger.LogInformation("Products already exist. Skipping product seeding.");
            }
        }

        private static async Task SeedRolesAndUsers(IServiceProvider serviceProvider, ILogger logger)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            logger.LogInformation("Checking for existing roles...");

            // Create roles
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    logger.LogInformation($"Creating role: {roleName}");
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        logger.LogError($"Failed to create role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            logger.LogInformation("Checking for admin user...");

            // Create admin user
            var adminEmail = "admin@admin.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                logger.LogInformation($"Creating admin user: {adminEmail}");

                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Kenji123456*");

                if (result.Succeeded)
                {
                    logger.LogInformation("Admin user created successfully.");

                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (!roleResult.Succeeded)
                    {
                        logger.LogError($"Failed to add admin user to Admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                    else
                    {
                        logger.LogInformation("Admin user added to Admin role successfully.");
                    }
                }
                else
                {
                    logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                logger.LogInformation("Admin user already exists. Skipping admin user creation.");
            }
        }
    }
}

