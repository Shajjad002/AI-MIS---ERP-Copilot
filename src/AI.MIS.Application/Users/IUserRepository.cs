using AI.MIS.Domain.Entities;

namespace AI.MIS.Application.Users;

public interface IUserRepository
{
    Task<User?> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default);
}
