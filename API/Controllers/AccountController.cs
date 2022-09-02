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

            using var hmac = new HMACSHA512();      //first time when a new HMAc bla bla is called, a new random key is set and is been assigned to password Salt
            var user = new AppUser {
                UserName = registerDto.Username.ToLower(),
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                passwordSalt = hmac.Key
            };

            _context.Users.Add(user);           //Begins tracking the given entity that are not already being tracked
            await _context.SaveChangesAsync();  //Add the tracked data into the database

            return user;

        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto) {

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

            if(user == null) return Unauthorized("Invalid user");

            using var hmac = new HMACSHA512(user.passwordSalt);     // Random key is set during registration but during login we can use already stored key of that particular user
                                                                    //This time we need to calculate the computed hash of their password using the password salt so we have something to compare it against
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i< computedHash.Length; i++) {
                if(computedHash[i] != user.passwordHash[i]) return Unauthorized("Invalid Password");
            }

            return user;

        }

        private async Task<bool> UserExists(string username) {
            return await _context.Users.AnyAsync(anyuser => anyuser.UserName == username.ToLower());
        }       
        
    }
}
