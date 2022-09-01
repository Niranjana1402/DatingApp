using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {

        private readonly DataContext _context; 
        
        public UsersController(DataContext context) {
            _context = context;
        }

        [HttpGet]           //niru
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() {
            var users = _context.Users.ToListAsync();
            return await users;
        }

        [HttpGet("{id}")]       //api/users/3
        public async Task<ActionResult<AppUser>> GetUserById(int id) {
            var user = _context.Users.FindAsync(id);
            return await user;

        }

    }
}