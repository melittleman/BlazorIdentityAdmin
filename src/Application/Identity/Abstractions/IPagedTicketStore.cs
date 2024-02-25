namespace BlazorAdminDashboard.Application.Identity.Abstractions;

public interface IPagedTicketStore
{
    Task<IPagedList<AuthenticationTicket>> SearchTicketsAsync(SearchFilter filter, CancellationToken? ct = default);
}
