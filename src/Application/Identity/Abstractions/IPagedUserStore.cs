using RedisKit.Querying;
using RedisKit.Querying.Abstractions;

namespace BlazorAdminDashboard.Application.Identity.Abstractions;

public interface IPagedUserStore<TUser>
{
    Task<IPagedList<TUser>> SearchUsersAsync(SearchFilter filter, string? query = null);
}
