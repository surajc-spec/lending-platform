using LendingPlatform.Application.DTOs;
using LendingPlatform.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LendingPlatform.Api.Controllers;

[ApiController]
[Route("api/loans")]
public class LoansController : ControllerBase
{
    private readonly ILoanApplicationService _loanApplicationService;

    public LoansController(
        ILoanApplicationService loanApplicationService)
    {
        _loanApplicationService = loanApplicationService;
    }

    // POST: /api/loans
    [HttpPost]
    [ProducesResponseType(
        typeof(LoanApplicationResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoanApplicationResponse>> Create(
        [FromBody] CreateLoanApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _loanApplicationService.CreateAsync(
            request,
            cancellationToken);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    // GET: /api/loans
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<LoanApplicationResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<LoanApplicationResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _loanApplicationService.GetAllAsync(
            cancellationToken);

        return Ok(result);
    }

    // GET: /api/loans/metrics
    [HttpGet("metrics")]
    [ProducesResponseType(
        typeof(LoanMetricsResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoanMetricsResponse>> GetMetrics(
        CancellationToken cancellationToken)
    {
        var result = await _loanApplicationService.GetMetricsAsync(
            cancellationToken);

        return Ok(result);
    }
}