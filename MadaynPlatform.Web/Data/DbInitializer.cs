using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MadaynPlatform.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>()!;
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>()!;
        var db = scope.ServiceProvider.GetRequiredService<MadaynDbContext>()!;
        await db.Database.MigrateAsync();

        foreach (var role in new[] { "Admin", "Investor", "Guest" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "admin@madayn.local";
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(admin, "Admin@123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Seed extended profile user if missing
        if (!await db.UsersExt.AnyAsync())
        {
            var extUser = new Models.User
            {
                Name = "مدير النظام",
                Email = adminEmail,
                PasswordHash = string.Empty,
                Role = "Admin",
                IsActive = true
            };
            db.UsersExt.Add(extUser);
            await db.SaveChangesAsync();

            // Seed a sample survey
            var survey = new Models.Survey
            {
                Title = "استطلاع رضا المستثمرين",
                Description = "نقدّر مشاركتكم لتحسين خدماتنا.",
                CreatedByUserId = extUser.UserId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(14),
                IsActive = true
            };
            db.Surveys.Add(survey);
            await db.SaveChangesAsync();

            db.Questions.AddRange(new[]
            {
                new Models.Question { SurveyId = survey.SurveyId, QuestionText = "كيف تقيم تجربة الاستثمار بشكل عام؟", QuestionType = "Rating", OrderIndex = 1 },
                new Models.Question { SurveyId = survey.SurveyId, QuestionText = "ما هي أبرز التحديات التي تواجهك؟", QuestionType = "Text", OrderIndex = 2 },
                new Models.Question { SurveyId = survey.SurveyId, QuestionText = "هل توصي بالمنصة لغيرك؟", QuestionType = "YesNo", OrderIndex = 3 }
            });

            // Seed a sample news
            db.News.Add(new Models.News
            {
                Title = "إطلاق منصة مدائن للاستطلاعات",
                Content = "يسرنا الإعلان عن إطلاق المنصة لدعم المستثمرين.",
                CreatedByUserId = extUser.UserId,
                IsPublished = true,
                PublishedAt = DateTime.Now
            });

            await db.SaveChangesAsync();
        }
    }
}

