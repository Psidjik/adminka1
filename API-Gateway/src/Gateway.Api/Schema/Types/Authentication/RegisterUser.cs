namespace Gateway.Api.Schema.Types.Authentication;

/// <summary>
/// Запрос на регистрацию
/// </summary>
public class RegisterUser
{
    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public required string Password { get; init; }
    

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public required FullName FullName { get; init; }

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string PersonalNumber { get; init; }
}
