using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services;

public class DashboardService : IDashboardService
{
    private readonly IApiService _apiService;


    public DashboardService(IApiService apiService)
    {
        _apiService = apiService;
    }


    public async Task<ApiResponse<CustomerDashboardDto?>> GetDashboardDataAsync()
    {
        return await _apiService.GetAsync<CustomerDashboardDto>("api/dashboard/me");
    }
}