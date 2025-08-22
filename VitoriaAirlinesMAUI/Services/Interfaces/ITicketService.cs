using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces;

public interface ITicketService
{
    Task<ApiResponse<List<Ticket>?>> GetMyUpcomingAsync();
    Task<ApiResponse<List<Ticket>?>> GetMyHistoryAsync();

}
