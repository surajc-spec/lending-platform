using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LendingPlatform.Application.DTOs;
using Xunit;

namespace LendingPlatform.Tests.Integration;

public class LoansApiTests
    : IClassFixture<LendingPlatformWebApplicationFactory>
{
    private readonly LendingPlatformWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public LoansApiTests(
        LendingPlatformWebApplicationFactory factory)
    {
        _factory = factory;

        // Start every test with a clean integration-test database.
        _factory.ResetDatabase();

        _client = _factory.CreateClient();
    }

    // -------------------------------------------------------
    // POST /api/loans
    // -------------------------------------------------------

    [Fact]
    public async Task PostLoan_ShouldCreateSuccessfulApplication()
    {
        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 800);

        var response = await _client.PostAsJsonAsync(
            "/api/loans",
            request);

        response.StatusCode
            .Should().Be(HttpStatusCode.Created);

        var result = await response.Content
            .ReadFromJsonAsync<LoanApplicationResponse>();

        result.Should().NotBeNull();
        result!.LoanAmount.Should().Be(500_000m);
        result.AssetValue.Should().Be(1_000_000m);
        result.CreditScore.Should().Be(800);
        result.Ltv.Should().Be(50m);
        result.Decision.Should().Be("Successful");
    }

    [Fact]
    public async Task PostLoan_ShouldCreateDeclinedApplication()
    {
        var request = new CreateLoanApplicationRequest(
            LoanAmount: 900_000m,
            AssetValue: 1_000_000m,
            CreditScore: 999);

        var response = await _client.PostAsJsonAsync(
            "/api/loans",
            request);

        response.StatusCode
            .Should().Be(HttpStatusCode.Created);

        var result = await response.Content
            .ReadFromJsonAsync<LoanApplicationResponse>();

        result.Should().NotBeNull();
        result!.LoanAmount.Should().Be(900_000m);
        result.AssetValue.Should().Be(1_000_000m);
        result.CreditScore.Should().Be(999);
        result.Ltv.Should().Be(90m);
        result.Decision.Should().Be("Declined");

        result.Reason.Should()
            .Be("Applications with an LTV of 90% or more are declined.");
    }

    [Fact]
    public async Task PostLoan_ShouldReturnBadRequest_WhenLoanAmountIsInvalid()
    {
        var request = new CreateLoanApplicationRequest(
            LoanAmount: -500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 800);

        var response = await _client.PostAsJsonAsync(
            "/api/loans",
            request);

        response.StatusCode
            .Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostLoan_ShouldReturnBadRequest_WhenAssetValueIsInvalid()
    {
        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 0m,
            CreditScore: 800);

        var response = await _client.PostAsJsonAsync(
            "/api/loans",
            request);

        response.StatusCode
            .Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostLoan_ShouldReturnBadRequest_WhenCreditScoreIsInvalid()
    {
        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 1000);

        var response = await _client.PostAsJsonAsync(
            "/api/loans",
            request);

        response.StatusCode
            .Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostLoan_ShouldNotPersistApplication_WhenRequestIsInvalid()
    {
        var request = new CreateLoanApplicationRequest(
            LoanAmount: -500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 800);

        var postResponse = await _client.PostAsJsonAsync(
            "/api/loans",
            request);

        postResponse.StatusCode
            .Should().Be(HttpStatusCode.BadRequest);

        var getResponse = await _client.GetAsync(
            "/api/loans");

        getResponse.StatusCode
            .Should().Be(HttpStatusCode.OK);

        var applications = await getResponse.Content
            .ReadFromJsonAsync<List<LoanApplicationResponse>>();

        applications.Should().NotBeNull();
        applications.Should().BeEmpty();
    }

    // -------------------------------------------------------
    // GET /api/loans
    // -------------------------------------------------------

    [Fact]
    public async Task GetLoans_ShouldReturnApplications()
    {
        var request = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 800);

        var postResponse = await _client.PostAsJsonAsync(
            "/api/loans",
            request);

        postResponse.StatusCode
            .Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync(
            "/api/loans");

        response.StatusCode
            .Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<List<LoanApplicationResponse>>();

        result.Should().NotBeNull();
        result.Should().ContainSingle();

        result![0].LoanAmount.Should().Be(500_000m);
        result[0].AssetValue.Should().Be(1_000_000m);
        result[0].CreditScore.Should().Be(800);
        result[0].Ltv.Should().Be(50m);
        result[0].Decision.Should().Be("Successful");
    }

    // -------------------------------------------------------
    // GET /api/loans/metrics
    // -------------------------------------------------------

    [Fact]
    public async Task GetMetrics_ShouldReturnCorrectMetrics()
    {
        var successfulRequest = new CreateLoanApplicationRequest(
            LoanAmount: 500_000m,
            AssetValue: 1_000_000m,
            CreditScore: 800);

        var declinedRequest = new CreateLoanApplicationRequest(
            LoanAmount: 900_000m,
            AssetValue: 1_000_000m,
            CreditScore: 999);

        var successfulResponse = await _client.PostAsJsonAsync(
            "/api/loans",
            successfulRequest);

        successfulResponse.StatusCode
            .Should().Be(HttpStatusCode.Created);

        var declinedResponse = await _client.PostAsJsonAsync(
            "/api/loans",
            declinedRequest);

        declinedResponse.StatusCode
            .Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync(
            "/api/loans/metrics");

        response.StatusCode
            .Should().Be(HttpStatusCode.OK);

        var result = await response.Content
            .ReadFromJsonAsync<LoanMetricsResponse>();

        result.Should().NotBeNull();

        result!.SuccessfulApplications.Should().Be(1);
        result.DeclinedApplications.Should().Be(1);
        result.TotalLoanValueWritten.Should().Be(500_000m);
        result.MeanAverageLtv.Should().Be(70m);
    }
}