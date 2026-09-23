namespace AI.MIS.Application.Users;

public interface IUserManagementService
{
    Task<IReadOnlyList<ManagedUser>> ListAsync(CancellationToken cancellationToken = default);
    Task<ManagedUser> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken = default);
    Task UpdateAccessAsync(Guid userId, UpdateUserAccessCommand command, CancellationToken cancellationToken = default);
}

public sealed record ManagedUser(Guid Id, string UserName, string DisplayName, string Email, bool IsActive, IReadOnlyList<string> Roles, IReadOnlyList<string> BranchCodes);
public sealed record CreateUserCommand(string UserName, string DisplayName, string Email, string Password, string Role, IReadOnlyList<string> BranchCodes);
public sealed record UpdateUserAccessCommand(string Role, IReadOnlyList<string> BranchCodes, bool IsActive);
