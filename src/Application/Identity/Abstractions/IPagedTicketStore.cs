namespace BlazorAdminDashboard.Application.Identity.Abstractions;

public interface IPagedTicketStore
{
    Task<IPagedList<AuthenticationTicket>> SearchTicketsAsync(SearchFilter filter, CancellationToken? ct = null);

    // TODO: Would be nicer to have as 'Ulid userId' but can we be certain this will be available everywhere?
    Task<IPagedList<AuthenticationTicket>> GetByUserIdAsync(string userId, CancellationToken? ct = null);
}
