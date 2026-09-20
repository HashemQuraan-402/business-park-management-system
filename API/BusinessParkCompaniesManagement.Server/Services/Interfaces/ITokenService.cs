namespace BusinessParkCompaniesManagement.Server.Services.Interfaces
{
    public interface ITokenService
    {
        (string token, DateTime expiresAt) GenerateToken(string userId, string username, string role, int? companyId = null);
    }

}
