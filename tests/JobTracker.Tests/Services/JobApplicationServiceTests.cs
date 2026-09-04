using JobTracker.Data;
using JobTracker.DTOs.JobApplication;
using JobTracker.Enums;
using JobTracker.Models;
using JobTracker.Services;
using JobTracker.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Tests.Services;

public class JobApplicationServiceTests
{
    private static async Task<Company> SeedCompanyAsync(ApplicationDbContext context, string name = "Acme Corp")
    {
        var company = new Company { Name = name, Website = "https://example.com" };
        context.Companies.Add(company);
        await context.SaveChangesAsync();
        return company;
    }

    [Fact]
    public async Task Should_Create_Application()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(context);
        var service = new JobApplicationService(context);

        var dto = new CreateJobApplicationDto(
            CompanyId: company.Id,
            Position: "Backend Developer",
            Status: ApplicationStatus.Applied,
            Location: "Remote",
            SalaryMin: 3000,
            SalaryMax: 5000,
            JobUrl: "https://example.com/job/123",
            AppliedAt: DateTime.UtcNow.Date,
            Notes: "Applied via LinkedIn"
        );

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Backend Developer", result.Value.Position);
        Assert.Equal("Applied", result.Value.Status);
        Assert.Equal(company.Id, result.Value.Company.Id);

        // Verify persisted
        var persisted = await context.JobApplications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == result.Value.Id);
        Assert.NotNull(persisted);
        Assert.Equal(company.Id, persisted.CompanyId);
    }

    [Fact]
    public async Task Should_Reject_Unknown_Company()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var service = new JobApplicationService(context);

        var dto = new CreateJobApplicationDto(
            CompanyId: 999,
            Position: "Backend Developer",
            Status: ApplicationStatus.Applied,
            Location: null,
            SalaryMin: null,
            SalaryMax: null,
            JobUrl: null,
            AppliedAt: DateTime.UtcNow.Date,
            Notes: null
        );

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Contains("999", result.Error.Message);
    }

    [Fact]
    public async Task Should_Reject_Negative_Or_Inverted_Salary()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(context);
        var service = new JobApplicationService(context);

        // Case 1: negative SalaryMin
        var dtoNegativeMin = new CreateJobApplicationDto(
            company.Id, "Dev", ApplicationStatus.Applied, null, -100, 5000, null, DateTime.UtcNow.Date, null);
        var resultMin = await service.CreateAsync(dtoNegativeMin);
        Assert.True(resultMin.IsFailure);
        Assert.Equal(ErrorType.Validation, resultMin.Error!.Type);
        Assert.Contains("SalaryMin", resultMin.Error.Message);

        // Case 2: negative SalaryMax
        var dtoNegativeMax = new CreateJobApplicationDto(
            company.Id, "Dev", ApplicationStatus.Applied, null, 1000, -500, null, DateTime.UtcNow.Date, null);
        var resultMax = await service.CreateAsync(dtoNegativeMax);
        Assert.True(resultMax.IsFailure);
        Assert.Equal(ErrorType.Validation, resultMax.Error!.Type);
        Assert.Contains("SalaryMax", resultMax.Error.Message);

        // Case 3: inverted (SalaryMax < SalaryMin)
        var dtoInverted = new CreateJobApplicationDto(
            company.Id, "Dev", ApplicationStatus.Applied, null, 5000, 3000, null, DateTime.UtcNow.Date, null);
        var resultInverted = await service.CreateAsync(dtoInverted);
        Assert.True(resultInverted.IsFailure);
        Assert.Equal(ErrorType.Validation, resultInverted.Error!.Type);
        Assert.Contains("SalaryMax", resultInverted.Error.Message);
    }

    [Fact]
    public async Task Should_Reject_Invalid_Status_Transition()
    {
        // Arrange
        using var context = TestDbContextFactory.Create();
        var company = await SeedCompanyAsync(context);

        // Seed application in terminal state Offer directly via context
        var app = new JobApplication
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Company = company,
            Position = "Backend Developer",
            Status = ApplicationStatus.Offer,
            AppliedAt = DateTime.UtcNow.Date,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.JobApplications.Add(app);
        await context.SaveChangesAsync();

        var service = new JobApplicationService(context);

        // Act: try to transition from Offer via PATCH
        var result = await service.UpdateStatusAsync(app.Id, ApplicationStatus.Interview);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        Assert.Contains("terminal", result.Error.Message, StringComparison.OrdinalIgnoreCase);

        // Verify not changed in DB
        var persisted = await context.JobApplications.AsNoTracking().FirstAsync(x => x.Id == app.Id);
        Assert.Equal(ApplicationStatus.Offer, persisted.Status);

        // Also verify Rejected and Withdrawn are terminal
        var rejectedApp = new JobApplication
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Company = company,
            Position = "Frontend Developer",
            Status = ApplicationStatus.Rejected,
            AppliedAt = DateTime.UtcNow.Date
        };
        context.JobApplications.Add(rejectedApp);
        await context.SaveChangesAsync();
        var resultRejected = await service.UpdateStatusAsync(rejectedApp.Id, ApplicationStatus.Applied);
        Assert.True(resultRejected.IsFailure);
        Assert.Equal(ErrorType.Validation, resultRejected.Error!.Type);

        var withdrawnApp = new JobApplication
        {
            Id = Guid.NewGuid(),
            CompanyId = company.Id,
            Company = company,
            Position = "Fullstack Developer",
            Status = ApplicationStatus.Withdrawn,
            AppliedAt = DateTime.UtcNow.Date
        };
        context.JobApplications.Add(withdrawnApp);
        await context.SaveChangesAsync();
        var resultWithdrawn = await service.UpdateStatusAsync(withdrawnApp.Id, ApplicationStatus.Applied);
        Assert.True(resultWithdrawn.IsFailure);
        Assert.Equal(ErrorType.Validation, resultWithdrawn.Error!.Type);
    }
}
