using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){

        if (await UserExists(registerDTO.Username)) return BadRequest("Username is taken");

        using var hmac = new HMACSHA512();
        var user = new AppUser {
            UserName = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.UserName,
            token = tokenService.CreateToken(user)
        };
        
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
        var isUser = await context.Users.FirstOrDefaultAsync(x => 
            x.UserName == loginDTO.Username.ToLower()
        );

        if (isUser == null) return Unauthorized("Invalid user or password");

        using var hmac = new HMACSHA512(isUser.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

        for (int i = 0; i < computedHash.Length; i++){
            if (computedHash[i] != isUser.PasswordHash[i]) return Unauthorized("Ivalid user or password");            
        }

        return new UserDTO
        {
            Username = isUser.UserName,
            token = tokenService.CreateToken(isUser)
        };
    }

    private async Task<bool> UserExists(string username){
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
