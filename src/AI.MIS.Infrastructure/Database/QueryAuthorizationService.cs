using System.Security.Claims;
using AI.MIS.Application.Copilot;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace AI.MIS.Infrastructure.Database;

public sealed class QueryAuthorizationService : IQueryAuthorizationService
{
    public void Validate(string sql, ClaimsPrincipal user)
    {
        if (user.IsInRole("Administrator") || user.IsInRole("MIS Analyst"))
            return;

        if (!user.IsInRole("Branch User"))
            throw new UnauthorizedAccessException("The authenticated user has no query role.");

        var allowed = user.FindAll("branch_code").Select(claim => claim.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (allowed.Count == 0)
            throw new UnauthorizedAccessException("The branch user has no authorized branches.");

        var parser = new TSql160Parser(initialQuotedIdentifiers: true);
        using var reader = new StringReader(sql);
        var fragment = parser.Parse(reader, out var errors);
        if (errors.Count > 0) throw new UnauthorizedAccessException("The query could not be checked for branch authorization.");
        var visitor = new BranchPredicateVisitor();
        fragment.Accept(visitor);

        if (visitor.BranchCodes.Count == 0 || visitor.BranchCodes.Any(branch => !allowed.Contains(branch)))
            throw new UnauthorizedAccessException("Branch users must query only their authorized branch data.");
    }

    private sealed class BranchPredicateVisitor : TSqlFragmentVisitor
    {
        public HashSet<string> BranchCodes { get; } = new(StringComparer.OrdinalIgnoreCase);

        public override void Visit(BooleanComparisonExpression node)
        {
            if (node.ComparisonType == BooleanComparisonType.Equals &&
                node.FirstExpression is ColumnReferenceExpression firstColumn &&
                string.Equals(firstColumn.MultiPartIdentifier.Identifiers[^1].Value, "BranchCode", StringComparison.OrdinalIgnoreCase) &&
                node.SecondExpression is StringLiteral literal)
                BranchCodes.Add(literal.Value);
            base.Visit(node);
        }
    }
}
