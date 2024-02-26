using System.Globalization;
using RedisKit.Querying.Enums;

namespace BlazorAdminDashboard.Infrastructure.Stores;

// TODO: What elements of this class should actually exist within the RedisKit library?
// We could get the library to implement the index within a HostedService,
// but how could we pass in the custom schema?
internal sealed class RedisPagedTicketStore(
    IRedisConnectionProvider provider,
    IOptionsMonitor<RedisJsonOptions> options) : IPagedTicketStore
{
    private const string TicketIndex = "idx:tickets";

    // TODO: Constants
    private readonly IRedisConnection _redis = provider.GetRequiredConnection("persistent-db");
    private readonly RedisJsonOptions _jsonOptions = options.Get("persistent-db");

    private SearchCommands Search => _redis.Db.FT();

    public Task<IPagedList<AuthenticationTicket>> SearchTicketsAsync(SearchFilter filter, CancellationToken? ct = null)
    {
        ArgumentNullException.ThrowIfNull(filter);
        ct?.ThrowIfCancellationRequested();

        // If it is null, will default to "*" to return all Tickets.
        if (filter.Query is not null)
        {
            // Escapes characters such as '@' or '.' in email addresses for example.
            string escapedQuery = filter.Query.EscapeSpecialCharacters();

            // TODO: Should these actually be broken out into individual methods instead?
            // e.g. SearchBySubAsync(), SearchByIpAddressAsync() etc...
            // How do want to handle searching by the GEO field "location". Probably need to
            // implement something in the RedisKit library that can assist with creating a
            // GEO search by radius if it doesn't already exist.
            filter.Query = 
                $"(@sub:{{*{escapedQuery}*}}) | (@idp:{{*{escapedQuery}*}}) | (@name:{{*{escapedQuery}*}}) | " +
                $"(@session_id:{{*{escapedQuery}*}}) | (@ip_address:{{*{escapedQuery}*}})";
        }

        // TODO: from appsettings configuration or constant.
        return Search.SearchAsync<AuthenticationTicket>(TicketIndex, filter, _jsonOptions.Serializer);
    }

    public async Task<IPagedList<AuthenticationTicket>> GetByUserIdAsync(string userId, CancellationToken? ct = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ct?.ThrowIfCancellationRequested();

        // TODO: Should this be passed in or potentially a configuration option?
        // It seems very unlikely that a single user will ever have more than 20
        // logged in sessions at once as that would mean multiple devices / browsers.
        SearchFilter filter = new(page: 1, count: 20)
        {
            // TODO: This sort seems to not actually be working because
            // Redis doesn't know what to do with a date format like this...
            Query = $"@sub:{{{userId}}}",
            OrderBy = "last_activity",
            SortBy = SortDirection.Descending
        };

        IPagedList<AuthenticationTicket> tickets = await Search.SearchAsync<AuthenticationTicket>(TicketIndex, filter, _jsonOptions.Serializer);

        return tickets.OrderByDescending(t =>
        {
            // TODO: Need to move this into an extension!
            if (t.Properties.Items.TryGetValue(".last_activity", out string? value) && DateTimeOffset.TryParseExact(value!, "r",
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTimeOffset dto))
            {
                return dto;
            }

            return DateTimeOffset.UnixEpoch;

        }).ToPagedList(tickets.TotalResults, filter);
    }
}
