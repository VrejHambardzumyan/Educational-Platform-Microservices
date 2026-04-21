using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Application.Interfaces;
using UserManagementService.Application.Models.DTOs;

namespace UserManagementService.Presentation.Controllers
{
    /// <summary>Authentication and account-access endpoints.</summary>
    [ApiController]
    [Route("auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>Register a new user account.</summary>
        /// <remarks>
        /// Creates the account and sends a 6-digit OTP to the supplied email.
        /// The account is not activated until the OTP is verified via <c>POST /auth/otp/verify</c> with Purpose = <c>SignUp</c>.
        /// </remarks>
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

        /// <summary>Authenticate with username and password.</summary>
        /// <remarks>Returns a short-lived access token and a long-lived refresh token on success.</remarks>
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

        /// <summary>Exchange a refresh token for a new access token.</summary>
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

        /// <summary>Revoke a refresh token (logout).</summary>
        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke(RefreshTokenRequestDto request)
        {
            await _authService.RevokeTokenAsync(request.RefreshToken);
            return NoContent();
        }

        // ── OTP ──────────────────────────────────────────────────────────────

        /// <summary>Send a 6-digit OTP to the user's email.</summary>
        /// <remarks>
        /// <b>Purpose</b> controls what the OTP is issued for:
        /// <list type="bullet">
        ///   <item><term>SignUp</term><description>Email verification after registration. Verify with <c>POST /auth/otp/verify</c>.</description></item>
        ///   <item><term>Payment</term><description>Pre-payment 2-factor challenge. Verify with <c>POST /auth/otp/verify</c>.</description></item>
        /// </list>
        /// For password-reset OTPs use <c>POST /auth/reset-password/request</c> instead.
        /// </remarks>
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

        /// <summary>Verify an OTP and complete the associated flow.</summary>
        /// <remarks>
        /// <b>Purpose</b> determines the response:
        /// <list type="bullet">
        ///   <item><term>SignUp</term><description>Activates the account and returns access + refresh tokens.</description></item>
        ///   <item><term>Payment</term><description>Returns <c>{ "verified": true }</c> on success.</description></item>
        /// </list>
        /// For password-reset OTP verification use <c>POST /auth/reset-password/confirm</c>.
        /// </remarks>
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

        // ── Password reset ────────────────────────────────────────────────────

        /// <summary>Send a password-reset OTP to the user's email.</summary>
        /// <remarks>
        /// Always returns 200 OK regardless of whether the email is registered, to prevent email enumeration.
        /// Complete the reset with <c>POST /auth/reset-password/confirm</c>.
        /// </remarks>
        [HttpPost("reset-password/request")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestDto dto, CancellationToken cancellationToken)
        {
            await _authService.RequestPasswordResetAsync(dto.Email, cancellationToken);
            return Ok(new { message = "If this email is registered, an OTP has been sent." });
        }

        /// <summary>Verify the password-reset OTP and set a new password.</summary>
        /// <remarks>
        /// OTP verification and password update happen atomically.
        /// Request a reset OTP first via <c>POST /auth/reset-password/request</c>.
        /// </remarks>
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
