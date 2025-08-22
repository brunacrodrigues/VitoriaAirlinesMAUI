using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services;

public class TicketService : ITicketService
{
    private readonly IApiService _apiService;

    public TicketService(IApiService apiService)
    {
        _apiService = apiService;
    }



    public async Task<ApiResponse<List<Ticket>?>> GetMyUpcomingAsync()
    {
        return await _apiService.GetAsync<List<Ticket>?>("api/flights/upcoming/me");
    }


    public async Task<ApiResponse<List<Ticket>?>> GetMyHistoryAsync()
    {
        return await _apiService.GetAsync<List<Ticket>?>("api/flights/history/me");
    }
}
