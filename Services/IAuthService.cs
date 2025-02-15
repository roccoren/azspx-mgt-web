using azspx_mgt_web.Models;

namespace azspx_mgt_web.Services;

public interface IAuthService
{
    Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
    string GenerateJwtToken(UserInfo user);
    bool ValidateUser(LoginRequest request, out UserInfo? user);
}