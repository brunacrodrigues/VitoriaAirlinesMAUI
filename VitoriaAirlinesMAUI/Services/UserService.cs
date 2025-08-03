using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    public class UserService : ApiService, IUserService
    {
        public UserService(HttpClient httpClient) : base(httpClient) { }



    }
}
