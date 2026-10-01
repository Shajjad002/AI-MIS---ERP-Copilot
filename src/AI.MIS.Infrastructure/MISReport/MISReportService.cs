using System;
using AI.MIS.Application.Copilot;
using AI.MIS.Application.Copilot.Models;
using AI.MIS.Application.Copilot.SPModels;
using AI.MIS.Domain.Entities;

namespace AI.MIS.Infrastructure.MISReport;

public class MISReportService : BaseService<LoanTransactionReportSP>, IMISReportService
{
   private readonly IAsyncRepository<LoanTransactionReportSP> _loanDisbursementReportService;

   public MISReportService(IAsyncRepository<LoanTransactionReportSP> loanDisbursementReportService)
   {
       _loanDisbursementReportService = loanDisbursementReportService;
   }

       public async Task<BaseResponse> GetBranchPortfolioReport(ReportParameterViewModel viewModel)
    {
        BaseResponse response = new BaseResponse();

        //var reportDate = Convert.ToDateTime(viewModel.ToDate);
        var branchProtfoliosDataList = await _misSPRepository.GetBranchPortfolioReport(viewModel);
        List<BranchProtfolioSP> branchProtfoliosList = branchProtfoliosDataList.BranchProtfolioSP.ToList();

        if (branchProtfoliosList.Count > 0)
        {
            var branchPortfolioViewModel = new BranchPortfolioViewModel();

            branchPortfolioViewModel.BranchProtfolioList = branchProtfoliosList.OrderBy(s => s.BranchCode).OrderBy(s => s.BranchCode).ToList();

            branchPortfolioViewModel.AreaWiseBranchProtfolioList = branchProtfoliosList.GroupBy(g => g.AreaCode)
            .Select(s => new BranchProtfolioSP
            {
                //ProgramCode = s.FirstOrDefault().ProgramCode,
                ZoneCode = s.FirstOrDefault().ZoneCode,
                ZoneName = s.FirstOrDefault().ZoneName,
                RegionCode = s.FirstOrDefault().RegionCode,
                RegionName = s.FirstOrDefault().RegionName,
                AreaCode = s.Key,
                AreaName = s.FirstOrDefault().AreaName,
                NoOfCreditOfficer = s.Sum(s => s.NoOfCreditOfficer),
                NoOfSMEOfficer = s.Sum(s => s.NoOfSMEOfficer),
                NoOfCenter = s.Sum(s => s.NoOfCenter),
                Member = s.Sum(s => s.Member),
                Borrower = s.Sum(s => s.Borrower),
                OverdueBorrower = s.Sum(s => s.OverdueBorrower),
                Disbursement = s.Sum(s => s.Disbursement),
                OutstandingWithServiceCharge = s.Sum(s => s.OutstandingWithServiceCharge),
                OverdueWithServiceCharge = s.Sum(s => s.OverdueWithServiceCharge),
                PAR = s.Sum(s => s.PAR),
                SavingsBalance = s.Sum(s => s.SavingsBalance),
                PrincipalOutstanding = s.Sum(s => s.PrincipalOutstanding),
                ServiceChargeOutstanding = s.Sum(s => s.ServiceChargeOutstanding),
                OverduePrincipal = s.Sum(s => s.OverduePrincipal),
                Income = s.Sum(s => s.Income),
                Expense = s.Sum(s => s.Expense),
                RecoverableAmount = s.Sum(s => s.RecoverableAmount),
                RegulerRecovered = s.Sum(s => s.RegulerRecovered),
                BadLoanPrincipalOS = s.Sum(s => s.BadLoanPrincipalOS),
                PARRatio = s.Sum(s => s.PAR) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.PAR) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                OverdueOutstandingRatio = s.Sum(s => s.OverduePrincipal) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.OverduePrincipal) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                SavingsOutstandingRatio = s.Sum(s => s.SavingsBalance) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.SavingsBalance) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                OTR = s.Sum(s => s.RegulerRecovered) > 0 && s.Sum(s => s.RecoverableAmount) > 0 ? (s.Sum(s => s.RegulerRecovered) * 100) / s.Sum(s => s.RecoverableAmount) : 0,

                //    GoodLoanPrincipalOSRatio 

            }).OrderBy(s => s.AreaCode).ToList();

            branchPortfolioViewModel.RegionWiseBranchProtfolioList = branchProtfoliosList.GroupBy(g => g.RegionCode)
              .Select(s => new BranchProtfolioSP
              {
                  //ProgramCode = s.FirstOrDefault().ProgramCode,
                  ZoneCode = s.FirstOrDefault().ZoneCode,
                  ZoneName = s.FirstOrDefault().ZoneName,
                  RegionCode = s.Key,
                  RegionName = s.FirstOrDefault().RegionName,

                  NoOfCreditOfficer = s.Sum(s => s.NoOfCreditOfficer),
                  NoOfSMEOfficer = s.Sum(s => s.NoOfSMEOfficer),
                  NoOfCenter = s.Sum(s => s.NoOfCenter),
                  Member = s.Sum(s => s.Member),
                  Borrower = s.Sum(s => s.Borrower),
                  OverdueBorrower = s.Sum(s => s.OverdueBorrower),
                  Disbursement = s.Sum(s => s.Disbursement),
                  OutstandingWithServiceCharge = s.Sum(s => s.OutstandingWithServiceCharge),
                  OverdueWithServiceCharge = s.Sum(s => s.OverdueWithServiceCharge),
                  PAR = s.Sum(s => s.PAR),
                  SavingsBalance = s.Sum(s => s.SavingsBalance),
                  PrincipalOutstanding = s.Sum(s => s.PrincipalOutstanding),
                  ServiceChargeOutstanding = s.Sum(s => s.ServiceChargeOutstanding),
                  OverduePrincipal = s.Sum(s => s.OverduePrincipal),
                  Income = s.Sum(s => s.Income),
                  Expense = s.Sum(s => s.Expense),
                  RecoverableAmount = s.Sum(s => s.RecoverableAmount),
                  RegulerRecovered = s.Sum(s => s.RegulerRecovered),
                  BadLoanPrincipalOS = s.Sum(s => s.BadLoanPrincipalOS),
                  PARRatio = s.Sum(s => s.PAR) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.PAR) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                  OverdueOutstandingRatio = s.Sum(s => s.OverduePrincipal) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.OverduePrincipal) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                  SavingsOutstandingRatio = s.Sum(s => s.SavingsBalance) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.SavingsBalance) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                  OTR = s.Sum(s => s.RegulerRecovered) > 0 && s.Sum(s => s.RecoverableAmount) > 0 ? (s.Sum(s => s.RegulerRecovered) * 100) / s.Sum(s => s.RecoverableAmount) : 0,

                  //    GoodLoanPrincipalOSRatio 
              }).OrderBy(s => s.RegionCode).ToList();

            branchPortfolioViewModel.ZoneWiseBranchProtfolioList = branchProtfoliosList.GroupBy(g => g.ZoneCode)
              .Select(s => new BranchProtfolioSP
              {
                  //ProgramCode = s.FirstOrDefault().ProgramCode,
                  ZoneName = s.FirstOrDefault().ZoneName,
                  ZoneCode = s.Key,

                  NoOfCreditOfficer = s.Sum(s => s.NoOfCreditOfficer),
                  NoOfSMEOfficer = s.Sum(s => s.NoOfSMEOfficer),
                  NoOfCenter = s.Sum(s => s.NoOfCenter),
                  Member = s.Sum(s => s.Member),
                  Borrower = s.Sum(s => s.Borrower),
                  OverdueBorrower = s.Sum(s => s.OverdueBorrower),
                  Disbursement = s.Sum(s => s.Disbursement),
                  OutstandingWithServiceCharge = s.Sum(s => s.OutstandingWithServiceCharge),
                  OverdueWithServiceCharge = s.Sum(s => s.OverdueWithServiceCharge),
                  PAR = s.Sum(s => s.PAR),
                  SavingsBalance = s.Sum(s => s.SavingsBalance),
                  PrincipalOutstanding = s.Sum(s => s.PrincipalOutstanding),
                  ServiceChargeOutstanding = s.Sum(s => s.ServiceChargeOutstanding),
                  OverduePrincipal = s.Sum(s => s.OverduePrincipal),
                  Income = s.Sum(s => s.Income),
                  Expense = s.Sum(s => s.Expense),
                  RecoverableAmount = s.Sum(s => s.RecoverableAmount),
                  RegulerRecovered = s.Sum(s => s.RegulerRecovered),
                  BadLoanPrincipalOS = s.Sum(s => s.BadLoanPrincipalOS),
                  PARRatio = s.Sum(s => s.PAR) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.PAR) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                  OverdueOutstandingRatio = s.Sum(s => s.OverduePrincipal) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.OverduePrincipal) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                  SavingsOutstandingRatio = s.Sum(s => s.SavingsBalance) > 0 && s.Sum(s => s.PrincipalOutstanding) > 0 ? (s.Sum(s => s.SavingsBalance) * 100) / s.Sum(s => s.PrincipalOutstanding) : 0,
                  OTR = s.Sum(s => s.RegulerRecovered) > 0 && s.Sum(s => s.RecoverableAmount) > 0 ? (s.Sum(s => s.RegulerRecovered) * 100) / s.Sum(s => s.RecoverableAmount) : 0,

                  //    GoodLoanPrincipalOSRatio 

              }).OrderBy(s => s.ZoneCode).ToList();

            if (branchPortfolioViewModel.LendingType == MemberLendingType.Group)
            {
                branchPortfolioViewModel.TotalNoOfOfficer = branchProtfoliosList.Sum(s => s.NoOfCreditOfficer);
            }
            else if (branchPortfolioViewModel.LendingType == MemberLendingType.Individual)
            {
                branchPortfolioViewModel.TotalNoOfOfficer = branchProtfoliosList.Sum(s => s.NoOfSMEOfficer);
            }
            else
            {
                branchPortfolioViewModel.TotalNoOfOfficer = branchProtfoliosList.Sum(s => s.NoOfCreditOfficer + s.NoOfSMEOfficer);
            }

            branchPortfolioViewModel.TotalNoOfCenter = branchProtfoliosList.Sum(s => s.NoOfCenter);
            branchPortfolioViewModel.TotalNoOfCreditOfficer = branchProtfoliosList.Sum(s => s.NoOfCreditOfficer);
            branchPortfolioViewModel.TotalNoOfSMEOfficer = branchProtfoliosList.Sum(s => s.NoOfSMEOfficer);
            branchPortfolioViewModel.TotalMember = branchProtfoliosList.Sum(s => s.Member);
            branchPortfolioViewModel.TotalBorrower = branchProtfoliosList.Sum(s => s.Borrower);
            branchPortfolioViewModel.TotalOverdueBorrower = branchProtfoliosList.Sum(s => s.OverdueBorrower);
            branchPortfolioViewModel.TotalDisbursement = branchProtfoliosList.Sum(s => s.Disbursement);
            branchPortfolioViewModel.TotalOutstandingWithServiceCharge = branchProtfoliosList.Sum(s => s.OutstandingWithServiceCharge);
            branchPortfolioViewModel.TotalOverdueWithServiceCharge = branchProtfoliosList.Sum(s => s.OverdueWithServiceCharge);
            branchPortfolioViewModel.TotalPAR = branchProtfoliosList.Sum(s => s.PAR);
            branchPortfolioViewModel.TotalBadLoanPrincipalOS = branchProtfoliosList.Sum(s => s.BadLoanPrincipalOS);
            branchPortfolioViewModel.TotalSavingsBalance = branchProtfoliosList.Sum(s => s.SavingsBalance);
            branchPortfolioViewModel.TotalPrincipalOutstanding = branchProtfoliosList.Sum(s => s.PrincipalOutstanding);
            branchPortfolioViewModel.TotalServiceChargeOutstanding = branchProtfoliosList.Sum(s => s.ServiceChargeOutstanding);
            branchPortfolioViewModel.TotalOverduePrincipal = branchProtfoliosList.Sum(s => s.OverduePrincipal);
            branchPortfolioViewModel.TotalIncome = branchProtfoliosList.Sum(s => s.Income);
            branchPortfolioViewModel.TotalExpense = branchProtfoliosList.Sum(s => s.Expense);
            branchPortfolioViewModel.TotalRegulerRecovered = branchProtfoliosList.Sum(s => s.RegulerRecovered);
            branchPortfolioViewModel.TotalRecoverableAmount = branchProtfoliosList.Sum(s => s.RecoverableAmount);
            branchPortfolioViewModel.AvaragePARRatio = branchPortfolioViewModel.TotalPAR > 0 && branchPortfolioViewModel.TotalPrincipalOutstanding > 0 ? (branchPortfolioViewModel.TotalPAR * 100) / branchPortfolioViewModel.TotalPrincipalOutstanding : 0;
            branchPortfolioViewModel.AvarageOverdueOutstandingRatio = branchPortfolioViewModel.TotalOverduePrincipal > 0 && branchPortfolioViewModel.TotalPrincipalOutstanding > 0 ? (branchPortfolioViewModel.TotalOverduePrincipal * 100) / branchPortfolioViewModel.TotalPrincipalOutstanding : 0;
            branchPortfolioViewModel.AvarageSavingsOutstandingRatio = branchPortfolioViewModel.TotalSavingsBalance > 0 && branchPortfolioViewModel.TotalPrincipalOutstanding > 0 ? (branchPortfolioViewModel.TotalSavingsBalance * 100) / branchPortfolioViewModel.TotalPrincipalOutstanding : 0;
            branchPortfolioViewModel.AvarageOTR = branchPortfolioViewModel.TotalRegulerRecovered > 0 && branchPortfolioViewModel.TotalRecoverableAmount > 0 ? (branchPortfolioViewModel.TotalRegulerRecovered * 100) / branchPortfolioViewModel.TotalRecoverableAmount : 0;
            branchPortfolioViewModel.TotalZone = branchProtfoliosList.DistinctBy(s => s.ZoneCode).Count();
            branchPortfolioViewModel.TotalRegion = branchProtfoliosList.DistinctBy(s => s.RegionCode).Count();
            branchPortfolioViewModel.TotalArea = branchProtfoliosList.DistinctBy(s => s.AreaCode).Count();
            branchPortfolioViewModel.TotalBranch = branchProtfoliosList.DistinctBy(s => s.BranchCode).Count();
            branchPortfolioViewModel.LendingType = viewModel.LendingType;
            branchPortfolioViewModel.ViewBy = viewModel.ViewBy;
            branchPortfolioViewModel.DataGenerationTime = branchProtfoliosDataList.DataGenerationTime;

            response.IsSuccessful = true;
            response.Message = "Ok";
            response.Data = branchPortfolioViewModel;
        }
        else
        {
            response.IsSuccessful = false;
            response.Message = "No Data Found";
        }
        return response;
    }
}
