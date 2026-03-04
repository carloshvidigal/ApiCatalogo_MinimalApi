using ApiCatalogo_MinimalApi.Models;

namespace ApiCatalogo_MinimalApi.Services
{
    public interface ITokenService
    {
        string GerarToken(string key, string issuer, string audience, UserModel user);
    }
}
