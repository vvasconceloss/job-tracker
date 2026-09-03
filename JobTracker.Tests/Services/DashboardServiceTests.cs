using JobTracker.Enums;
using JobTracker.Models;
using JobTracker.Services;
using JobTracker.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Tests.Services;

public class DashboardServiceTests
{
    [Fact]
    public async Task Should_Calculate_Interview_Rate()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();

        var companyA = new Company { Name = "Integer Consulting", Website = "https://integer.pt" };
        var companyB = new Company { Name = "Acme Corp", Website = "https://acme.com" };
        context.Companies.AddRange(companyA, companyB);
        await context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // 4 applications total: 1 Interview => 25% rate
        // 3 within this month, 1 previous month to test applicationsThisMonth
        var apps = new[]
        {
            new JobApplication
            {
                Id = Guid.NewGuid(),
                CompanyId = companyA.Id,
                Company = companyA,
                Position = "Backend Developer",
                Status = ApplicationStatus.Interview,
                AppliedAt = now.Date, // this month
            },
            new JobApplication
            {
                Id = Guid.NewGuid(),
                CompanyId = companyA.Id,
                Company = companyA,
                Position = "Frontend Developer",
                Status = ApplicationStatus.Applied,
                AppliedAt = now.Date, // this month
            },
            new JobApplication
            {
                Id = Guid.NewGuid(),
                CompanyId = companyB.Id,
                Company = companyB,
                Position = "DevOps Engineer",
                Status = ApplicationStatus.Rejected,
                AppliedAt = firstDayOfMonth.AddDays(-5), // previous month
            },
            new JobApplication
            {
                Id = Guid.NewGuid(),
                CompanyId = companyB.Id,
                Company = companyB,
                Position = "QA Engineer",
                Status = ApplicationStatus.Offer,
                AppliedAt = now.Date, // this month
            },
        };
        context.JobApplications.AddRange(apps);
        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var dashboard = await service.GetDashboardAsync();

        // Assert - interviewRate = interviews / total * 100 = 1/4 *100 = 25.0
        Assert.Equal(4, dashboard.TotalApplications);
        Assert.Equal(3, dashboard.ApplicationsThisMonth);
        Assert.Equal(1, dashboard.Interviews);
        Assert.Equal(0, dashboard.TechnicalTests);
        Assert.Equal(1, dashboard.Offers);
        Assert.Equal(1, dashboard.Rejections);
        Assert.Equal(0, dashboard.Withdrawn);
        Assert.Equal(25.0, dashboard.InterviewRate);

        // Aggregations
        Assert.Equal(4, dashboard.ApplicationsByStatus.Count);
        Assert.Equal(1, dashboard.ApplicationsByStatus["Interview"]);
        Assert.Equal(1, dashboard.ApplicationsByStatus["Applied"]);
        Assert.Equal(1, dashboard.ApplicationsByStatus["Rejected"]);
        Assert.Equal(1, dashboard.ApplicationsByStatus["Offer"]);

        Assert.Equal(2, dashboard.ApplicationsByCompany.Count);
        var byCompanyA = dashboard.ApplicationsByCompany.FirstOrDefault(x => x.Company == "Integer Consulting");
        var byCompanyB = dashboard.ApplicationsByCompany.FirstOrDefault(x => x.Company == "Acme Corp");
        Assert.NotNull(byCompanyA);
        Assert.NotNull(byCompanyB);
        Assert.Equal(2, byCompanyA.Count);
        Assert.Equal(2, byCompanyB.Count);
    }

    [Fact]
    public async Task Should_Calculate_Interview_Rate_When_Empty()
    {
        // Arrange - edge case: no applications => rate 0, no division by zero
        using var context = TestDbContextFactory.Create();
        var service = new DashboardService(context);

        // Act
        var dashboard = await service.GetDashboardAsync();

        // Assert
        Assert.Equal(0, dashboard.TotalApplications);
        Assert.Equal(0, dashboard.InterviewRate);
        Assert.Equal(0, dashboard.ApplicationsThisMonth);
        Assert.Empty(dashboard.ApplicationsByStatus);
        Assert.Empty(dashboard.ApplicationsByCompany);
    }

    [Fact]
    public async Task Should_Calculate_Interview_Rate_Rounding()
    {
        // Arrange - test rounding: 1 interview / 3 total = 33.333... => 33.3
        using var context = TestDbContextFactory.Create();
        var company = new Company { Name = "Test Co" };
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        var apps = new[]
        {
            new JobApplication { Id = Guid.NewGuid(), CompanyId = company.Id, Company = company, Position = "Dev1", Status = ApplicationStatus.Interview, AppliedAt = DateTime.UtcNow.Date },
            new JobApplication { Id = Guid.NewGuid(), CompanyId = company.Id, Company = company, Position = "Dev2", Status = ApplicationStatus.Applied, AppliedAt = DateTime.UtcNow.Date },
            new JobApplication { Id = Guid.NewGuid(), CompanyId = company.Id, Company = company, Position = "Dev3", Status = ApplicationStatus.Applied, AppliedAt = DateTime.UtcNow.Date },
        };
        context.JobApplications.AddRange(apps);
        await context.SaveChangesAsync();

        var service = new DashboardService(context);

        // Act
        var dashboard = await service.GetDashboardAsync();

        // Assert
        Assert.Equal(3, dashboard.TotalApplications);
        Assert.Equal(1, dashboard.Interviews);
        Assert.Equal(33.3, dashboard.InterviewRate);
    }
}
