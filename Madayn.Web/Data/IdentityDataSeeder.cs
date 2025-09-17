using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Madayn.Web.Data;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var ctx = services.GetRequiredService<ApplicationDbContext>();

        var roles = new[] { "Admin", "Employee", "Investor", "Guest" };
        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
            {
                await roleManager.CreateAsync(new IdentityRole(r));
            }
        }

        var adminEmail = "admin@madayn.om";
        var admin = await userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(admin, "Admin#12345");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
