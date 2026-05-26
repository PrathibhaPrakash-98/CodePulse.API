using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Implementation;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            //Creating a user
            var user = new IdentityUser
            {
                UserName = request.Email.Trim(),
                Email = request.Email.Trim()
            };

            var identityResult = await userManager.CreateAsync(user, request.Password);
            if (identityResult.Succeeded)
            {
                //User created successfully
                identityResult = await userManager.AddToRoleAsync(user, "Reader"); //Assigning default role
                if (identityResult.Succeeded)
                {
                    return Ok(new { message = "User registered successfully" });
                }
                else
                {
                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError(error.Code, error.Description);
                        }
                    }
                }
            }
            else
            {
                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }
            return ValidationProblem(ModelState);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO req)
        {
            var identityUser = await userManager.FindByEmailAsync(req.Email);
            if (identityUser != null)
            {
                var checkPasswordResult = await userManager.CheckPasswordAsync(identityUser, req.Password);
                if (checkPasswordResult)
                {
                    var roles = await userManager.GetRolesAsync(identityUser);
                    var token = tokenRepository.CreateJwtToken(identityUser, roles.ToList());
                    var response = new ResponseDTO()
                    {
                        Email = req.Email,
                        Token = token,
                        Roles = roles.ToList()

                    };
                    return Ok(response);
                }

                else
                {
                    ModelState.AddModelError("Login", "Invalid email or password");
                }

            }
            return ValidationProblem(ModelState);
        }
    }
}
