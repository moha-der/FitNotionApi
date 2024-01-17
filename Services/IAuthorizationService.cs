using FitNotionApi.Models.Custom;

namespace FitNotionApi.Services
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResponse> DevolverToken(AuthorizationRequest authorization);
    }
}
