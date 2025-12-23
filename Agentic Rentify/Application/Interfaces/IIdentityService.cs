
public interface IIdentityService
{
    Task<AuthResponseDto> RegisterAsync(string email, string password, string firstName, string lastName, string gender, string nationality, DateTime dateOfBirth, string? profileImage);
    Task<AuthResponseDto> LoginAsync(string email, string password);
    Task<bool> ConfirmEmailAsync(string email, string code);
    Task<bool> ConfirmEmailByTokenAsync(string email, string token);
    Task ResendOtpAsync(string email);
    Task<AuthResponseDto> LoginWithGoogleAsync(string idToken);
    Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
    
    Task ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(string email, string tokenOrOtp, string newPassword);
    Task ChangePasswordAsync(string email, string currentPassword, string newPassword);
    
    Task<bool> ValidateTokenAsync(string token);
}
