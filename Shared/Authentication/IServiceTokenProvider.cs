namespace Shared.Authentication;

public interface IServiceTokenProvider
{
    string GenerateToken();
}
