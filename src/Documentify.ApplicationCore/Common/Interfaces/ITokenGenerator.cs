namespace Documentify.ApplicationCore.Common.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(string userId, string userEmail);
    }
}
