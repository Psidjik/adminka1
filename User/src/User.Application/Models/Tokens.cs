namespace User.Application.Models;

public class Tokens
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
