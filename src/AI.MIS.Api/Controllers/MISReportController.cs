using Microsoft.AspNetCore.Mvc;
using AI.MIS.Application.Users;
using AI.MIS.Application.Copilot;
using AI.MIS.Application.Copilot.Models;
using AI.MIS.Domain.Entities;
using AI.MIS.Application.Copilot.Interface;
using System.Globalization;

namespace AI.MIS.Api.Controllers;

[ApiController]
[Route("api/misReports")]
public sealed class MISReportController : ControllerBase
{
    private readonly UserSession _userSession;
    private readonly IMISReportService _mISReportService;

    public MISReportController(UserSession userSession, IMISReportService mISReportService)
    {
        _userSession = userSession;
        _mISReportService = mISReportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData([FromQuery] DashboardQueryParameters query)
    {
        if (string.IsNullOrWhiteSpace(query.FromDate) ||
            string.IsNullOrWhiteSpace(query.ToDate) ||
            string.IsNullOrWhiteSpace(query.ProgramCode) ||
            string.IsNullOrWhiteSpace(query.ViewBy))
        {
            return BadRequest(new { message = "FromDate, ToDate, ProgramCode, and ViewBy are required query parameters." });
        }

        if (!DateTime.TryParseExact(query.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) ||
            !DateTime.TryParseExact(query.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            return BadRequest(new { message = "FromDate and ToDate must use the yyyy-MM-dd format." });
        }

        var viewModel = new ReportParameterViewModel
        {
            FromDate = query.FromDate,
            ToDate = query.ToDate,
            ProgramCode = query.ProgramCode,
            ViewBy = query.ViewBy,
            ZoneCode = query.ZoneCode ?? _userSession.ZoneCode,
            RegionCode = query.RegionCode ?? _userSession.RegionCode,
            AreaCode = query.AreaCode ?? _userSession.AreaCode,
            BranchCode = query.BranchCode ?? _userSession.BranchCode
        };

        if (viewModel.ProgramCode == null)
        {
            viewModel.ProgramCode = _userSession.ProgramCode;
        }

        var result = await _mISReportService.GetBranchPortfolioReport(viewModel);
        result.IsFromReportServer = OperationPolicies.WillReportLoadFromReportServerDB == 1;

        var dashboardViewModel = new DashboardViewModel();
        if (result.Data is BranchPortfolioViewModel portfolio)
        {
            dashboardViewModel.BranchPortfolio = portfolio;
        }

        return Ok(new BaseResponse
        {
            IsSuccessful = result.IsSuccessful,
            Message = result.Message,
            Data = dashboardViewModel,
            IsFromReportServer = result.IsFromReportServer
        });

    }

    public sealed record DashboardQueryParameters(
        string? FromDate,
        string? ToDate,
        string? ProgramCode,
        string? ViewBy,
        string? ZoneCode,
        string? RegionCode,
        string? AreaCode,
        string? BranchCode);

    public sealed record LoginRequest(string UserName, string Password);
}
