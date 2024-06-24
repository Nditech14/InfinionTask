using Application.Contract;
using Application.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InfinionTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IEmailService _emailService;

        public UserController(IUser user, IEmailService emailService)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> LoginUser(LoginDto loginDto)
        {
            var result = await _user.LoginUserAsync(loginDto);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> Register(RegisterDto registerDto)
        {
            var result = await _user.RegisterUserAsync(registerDto);

            if (result != null)
            {
                if (result.IsSuccess)
                {
                   
                    try
                    {
                        var emailSubject = "Registration Confirmation";
                        var emailBody = $"Dear {registerDto.FirstName},<br/><br/>Thank you for registering with us.";

                        await _emailService.SendEmailAsync(registerDto.Email, emailSubject, emailBody);
                    }
                    catch (Exception ex)
                    {
                       
                        Console.WriteLine($"Failed to send confirmation email: {ex.Message}");
                    }
                }

                return Ok(result);
            }

            return BadRequest("Registration failed");
        }
    }
}
