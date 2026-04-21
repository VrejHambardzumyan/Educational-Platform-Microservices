using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Presentation.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(SignUpRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.RegisterUserAsync(request.UserName, request.Password, request.Email, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Registration failed." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(SignInRequestDto request)
        {
            try
            {
                var tokenResponse = await _authService.LoginUserAsync(request.UserName, request.Password);
                return Ok(tokenResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Login failed." });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto request)
        {
            try
            {
                var tokenResponse = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(tokenResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Token refresh failed." });
            }
        }

        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke(RefreshTokenRequestDto request)
        {
            await _authService.RevokeTokenAsync(request.RefreshToken);
            return NoContent();
        }

        // ── 2FA ──────────────────────────────────────────────────────────────

        [HttpPost("otp/request")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDto dto, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.RequestOtpAsync(dto.Email, dto.Purpose, cancellationToken);
                return Ok(new { message = "OTP sent to your email." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Failed to send OTP." });
            }
        }

        [HttpPost("otp/verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (dto.Purpose == "SignUp")
                {
                    var tokens = await _authService.VerifySignupOtpAsync(dto.Email, dto.Otp, cancellationToken);
                    return Ok(tokens);
                }

                if (dto.Purpose == "Payment")
                {
                    var verified = await _authService.VerifyPaymentOtpAsync(dto.Email, dto.Otp, cancellationToken);
                    return verified ? Ok(new { verified = true }) : BadRequest(new { message = "Invalid or expired OTP." });
                }

                return BadRequest(new { message = $"Unsupported OTP purpose: {dto.Purpose}." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "OTP verification failed." });
            }
        }

        [HttpPost("reset-password/request")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestDto dto, CancellationToken cancellationToken)
        {
            await _authService.RequestPasswordResetAsync(dto.Email, cancellationToken);
            // Always return OK to avoid email enumeration
            return Ok(new { message = "If this email is registered, an OTP has been sent." });
        }

        [HttpPost("reset-password/confirm")]
        public async Task<IActionResult> ResetPasswordConfirm([FromBody] ResetPasswordConfirmDto dto, CancellationToken cancellationToken)
        {
            try
            {
                await _authService.ConfirmPasswordResetAsync(dto.Email, dto.Otp, dto.NewPassword, cancellationToken);
                return Ok(new { message = "Password reset successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Password reset failed." });
            }
        }
    }
}
