using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Dto.Account;
using finTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using finTrack.Interfaces;
using finTrack.Dto.Transaction;

namespace finTrack.Controllers
{
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IAcountRepository _acountRepo;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITransactionRepository _transactionRepo;
        public AccountController(UserManager<AppUser> usermanager, ITokenService tokenservice, SignInManager<AppUser> signInManager, IAcountRepository acountRepo, ITransactionRepository transactionRepo)
        {
            _userManager = usermanager;
            _tokenService = tokenservice;
            _signInManager = signInManager;
            _acountRepo = acountRepo;
            _transactionRepo = transactionRepo;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid username!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

            var token = _tokenService.CreateToken(user);

            Response.Cookies.Append("TOKEN", token,
            new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(7)
            });

            return Ok(
                new NewUserDto
                {
                    Token = token
                }
            );
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var appUser = new AppUser
                {
                    FirstName = registerDto.FirstName,
                    LastName =registerDto.LastName,
                    Guid = Guid.NewGuid().ToString().ToUpper(),
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                };

                appUser.PasswordHash = _userManager.PasswordHasher.HashPassword(appUser, registerDto.Password);
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if(createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if(roleResult.Succeeded)
                    {
                        var token = _tokenService.CreateToken(appUser);
            
                        Response.Cookies.Append("TOKEN", token,
                        new CookieOptions()
                        {
                            Expires = DateTime.Now.AddDays(7)
                        });
                        return Ok(
                            new NewUserDto
                            {
                                Token = token
                            }
                        );
                    }else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("balance/{userId}")]
        public async Task<IActionResult> GetBalance()
        {
            if (!HttpContext.Items.ContainsKey("UserID"))return StatusCode(400);
  
            var UserID = HttpContext.Items["UserID"]?.ToString();
            if (string.IsNullOrEmpty(UserID)) return StatusCode(400);

            var user = await _acountRepo.GetUserByGuidAsync(UserID);
            if(user == null)
            {
                return StatusCode(400);
            }

            return Ok(new { Balance = user.Balance });
        }

    }
}