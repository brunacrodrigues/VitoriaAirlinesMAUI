using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<CustomerDashboardDto?>> GetDashboardDataAsync();
    }
}
