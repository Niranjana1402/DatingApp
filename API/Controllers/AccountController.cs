using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context; 
        
        public AccountController(DataContext context) {
            _context = context;
        }

        [HttpPost("register")]

        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto) {
            
            if(await UserExists(registerDto.Username)) return BadRequest("Username already exists");

            using var hmac = new HMACSHA512();
            var user = new AppUser {
                UserName = registerDto.Username.ToLower(),
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                passwordSalt = hmac.Key
            };

            _context.Users.Add(user);           //Begins tracking the given entity that are not already being tracked
            await _context.SaveChangesAsync();  //Add the tracked data into the database

            return user;

        }

        private async Task<bool> UserExists(string username) {
            return await _context.Users.AnyAsync(anyuser => anyuser.UserName == username.ToLower());
        }       
        
    }
}