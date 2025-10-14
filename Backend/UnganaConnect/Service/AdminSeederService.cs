using Microsoft.EntityFrameworkCore;
using UnganaConnect.Data;
using UnganaConnect.Models.Users;
using BCrypt.Net;

namespace UnganaConnect.Service
{
    public class AdminSeederService
    {
        private readonly UnganaConnectDbcontext _context;
        private readonly ILogger<AdminSeederService> _logger;

        public AdminSeederService(UnganaConnectDbcontext context, ILogger<AdminSeederService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAdminUsersAsync()
        {
            try
            {
                // Check if any admin users already exist
                var adminExists = await _context.Users.AnyAsync(u => u.Role == "Admin");
                
                if (adminExists)
                {
                    _logger.LogInformation("Admin users already exist. Skipping seeding.");
                    return;
                }

                // Create default admin users
                var adminUsers = new List<User>
                {
                    new User
                    {
                        Email = "admin@ungana-afrika.org",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                        FirstName = "System",
                        LastName = "Administrator",
                        Role = "Admin",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Email = "superadmin@ungana-afrika.org",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin123!"),
                        FirstName = "Super",
                        LastName = "Administrator",
                        Role = "Admin",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };

                _context.Users.AddRange(adminUsers);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully seeded {Count} admin users.", adminUsers.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding admin users.");
                throw;
            }
        }

        public async Task SeedDefaultMemberAsync()
        {
            try
            {
                // Check if any member users exist
                var memberExists = await _context.Users.AnyAsync(u => u.Role == "Member");
                
                if (memberExists)
                {
                    _logger.LogInformation("Member users already exist. Skipping default member seeding.");
                    return;
                }

                // Create a default member user for testing
                var defaultMember = new User
                {
                    Email = "member@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member123!"),
                    FirstName = "Default",
                    LastName = "Member",
                    Role = "Member",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(defaultMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully seeded default member user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while seeding default member user.");
                throw;
            }
        }
    }
}
