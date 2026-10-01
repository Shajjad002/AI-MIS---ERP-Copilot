using Microsoft.AspNetCore.Mvc;
using AI.MIS.Application.Users;
using AI.MIS.Application.Copilot;
using AI.MIS.Application.Copilot.Models;
using AI.MIS.Domain.Entities;

namespace AI.MIS.Api.Controllers;

[ApiController]
[Route("api/misReports")]
public sealed class MISReportController : ControllerBase
{
    private readonly UserSession _userSession;
    private readonly BaseResponse _response ;

    public MISReportController(UserSession userSession, BaseResponse response)
    {
        _userSession = userSession;
        _response = new BaseResponse();
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData(ReportParameterViewModel viewModel)
    {
        var branchDayStatusViewModel = new BranchDayStatusViewModel();

        if (viewModel.ProgramCode == null)
        {
            branchDayStatusViewModel.ProgramCode = _userSession.ProgramCode;
            viewModel.ProgramCode = _userSession.ProgramCode;
        }
        else
        {
            branchDayStatusViewModel.ProgramCode = viewModel.ProgramCode;
        }

        if (viewModel.ZoneCode == null)
        {
            branchDayStatusViewModel.ZoneCode = _userSession.ZoneCode;
            viewModel.ZoneCode = _userSession.ZoneCode;
        }
        if (viewModel.RegionCode == null)
        {
            branchDayStatusViewModel.RegionCode = _userSession.RegionCode;
            viewModel.RegionCode = _userSession.RegionCode;
        }
        if (viewModel.AreaCode == null)
        {
            branchDayStatusViewModel.AreaCode = _userSession.AreaCode;
            viewModel.AreaCode = _userSession.AreaCode;
        }
        if (viewModel.BranchCode == null)
        {
            branchDayStatusViewModel.BranchCode = _userSession.BranchCode;
            viewModel.BranchCode = _userSession.BranchCode;
        }

        branchDayStatusViewModel.SystemDay = Convert.ToDateTime(viewModel.ToDate);

        var result = new BaseResponse();
        result = await misReportService.GetBranchPortfolioReport(viewModel);
        result.IsFromReportServer = OperationPolicies.WillReportLoadFromReportServerDB == 1;

        var workStationStatusList = await _dayInformationService.GetWorkStationDayStatusList(branchDayStatusViewModel);
        var dashboardViewModel = new DashboardViewModel();
        if (result.IsSuccessful || workStationStatusList.Count > 0)
        {
            dashboardViewModel.BranchPortfolio = (BranchPortfolioViewModel)result.Data;
            if (!string.IsNullOrEmpty(viewModel.BranchCode))
            {
                dashboardViewModel.CurrentDateBranchOpenCount = workStationStatusList.Where(s => s.BranchCode == viewModel.BranchCode).ToList().Count;
            }
            else if (!string.IsNullOrEmpty(viewModel.AreaCode))
            {
                dashboardViewModel.CurrentDateBranchOpenCount = workStationStatusList.Where(s => s.AreaCode == viewModel.AreaCode).ToList().Count;
            }
            else if (!string.IsNullOrEmpty(viewModel.RegionCode))
            {
                dashboardViewModel.CurrentDateBranchOpenCount = workStationStatusList.Where(s => s.RegionCode == viewModel.RegionCode).ToList().Count;
            }
            else if (!string.IsNullOrEmpty(viewModel.ZoneCode))
            {
                dashboardViewModel.CurrentDateBranchOpenCount = workStationStatusList.Where(s => s.ZoneCode == viewModel.ZoneCode).ToList().Count;
            }
            else
            {
                dashboardViewModel.CurrentDateBranchOpenCount = workStationStatusList.Count;

            }

        }
        _response.IsSuccessful = result.IsSuccessful;
        _response.Message = result.Message;
        _response.Data = dashboardViewModel;
        _response.IsFromReportServer = result.IsFromReportServer;

        return Ok(_response);

    }

    public sealed record LoginRequest(string UserName, string Password);
}
