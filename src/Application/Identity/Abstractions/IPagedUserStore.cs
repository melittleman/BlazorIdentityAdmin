namespace BlazorIdentityAdmin.Application.Identity.Abstractions;

public interface IPagedUserStore : IUserStore<User>
{
    Task<IPagedList<User>> SearchUsersAsync(SearchFilter filter, CancellationToken? ct = default);
}
