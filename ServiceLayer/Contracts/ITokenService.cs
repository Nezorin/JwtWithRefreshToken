using System.Security.Claims;

namespace ServiceLayer.Contracts
{
    public interface ITokenService
    {
        string GenerateJwtToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
