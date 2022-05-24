using System;

namespace IdentityServerHost.Pages.Login;

public class LoginOptions
{
    public static bool AllowLocalLogin = true;
    public static bool AllowRememberLogin = true;
    public static TimeSpan RememberMeLoginDuration = TimeSpan.FromHours(8);
    public static string InvalidCredentialsErrorMessage = "Invalid username or password";
}