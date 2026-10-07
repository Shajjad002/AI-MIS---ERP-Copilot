using System;
using System.Linq.Expressions;
using AI.MIS.Application.Copilot;
using AI.MIS.Application.Copilot.Interface;
using AI.MIS.Application.Copilot.Models;
using AI.MIS.Application.Copilot.SPModels;
using AI.MIS.Domain.Entities;

namespace AI.MIS.Infrastructure.MISReport;

public class MISReportService : IMISReportService
{
    public Task<BaseResponse> GetBranchPortfolioReport(ReportParameterViewModel viewModel)
    {
        return Task.FromResult(new BaseResponse
        {
            IsSuccessful = true,
            Message = "Branch portfolio report is placeholder-only for the current local build.",
            Data = new BranchPortfolioViewModel()
        });
    }

    public Task<LoanTransactionReportSP> GetById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IList<LoanTransactionReportSP>> GetList()
    {
        return Task.FromResult<IList<LoanTransactionReportSP>>(new List<LoanTransactionReportSP>());
    }

    public Task<IList<LoanTransactionReportSP>> GetList(ISpecification<LoanTransactionReportSP> spec)
    {
        return Task.FromResult<IList<LoanTransactionReportSP>>(new List<LoanTransactionReportSP>());
    }

    public Task<int> Count(ISpecification<LoanTransactionReportSP> spec)
    {
        return Task.FromResult(0);
    }

    public Task<LoanTransactionReportSP> Add(LoanTransactionReportSP entity)
    {
        throw new NotImplementedException();
    }

    public Task Update(LoanTransactionReportSP entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(long id)
    {
        throw new NotImplementedException();
    }

    public Task PermanentDelete(LoanTransactionReportSP entity)
    {
        throw new NotImplementedException();
    }

    public Task<IList<LoanTransactionReportSP>> ListAsync()
    {
        return Task.FromResult<IList<LoanTransactionReportSP>>(new List<LoanTransactionReportSP>());
    }

    public Task<IList<LoanTransactionReportSP>> ListAsync(Expression<Func<LoanTransactionReportSP, bool>> criteria, params string[] navigations)
    {
        return Task.FromResult<IList<LoanTransactionReportSP>>(new List<LoanTransactionReportSP>());
    }

    public Task<IList<LoanTransactionReportSP>> ListAsync(ISpecification<LoanTransactionReportSP> spec)
    {
        return Task.FromResult<IList<LoanTransactionReportSP>>(new List<LoanTransactionReportSP>());
    }

    public Task UpdateAll(List<LoanTransactionReportSP> entity)
    {
        throw new NotImplementedException();
    }

    public Task AddAll(List<LoanTransactionReportSP> entity)
    {
        throw new NotImplementedException();
    }

    public Task<LoanTransactionReportSP> FirstOrDefaultAsync(Expression<Func<LoanTransactionReportSP, bool>> criteria, params string[] navigations)
    {
        throw new NotImplementedException();
    }

    public Task<LoanTransactionReportSP> FirstOrDefaultAsync(ISpecification<LoanTransactionReportSP> spec)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(LoanTransactionReportSP entity, params Expression<Func<LoanTransactionReportSP, object>>[] properties)
    {
        throw new NotImplementedException();
    }
}
