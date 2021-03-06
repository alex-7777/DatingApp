using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data {
    public class Seed 
    {        
        // Not nencessary to use own Verification method if Identity is in place
        // private readonly DataContext _context;
        // public Seed (DataContext context) 
        // {
        //     _context = context;
        // }

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public Seed (UserManager<User> userManager, RoleManager<Role> roleManager) 
        {
            _userManager = userManager;            
            _roleManager = roleManager;   
        }

        public void SeedUsers()
        {             
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                var roles = new List<Role>()
                {
                    new Role { Name = "Member" },
                    new Role { Name = "Admin" },
                    new Role { Name = "Moderator" },
                    new Role { Name = "VIP" }
                };

                foreach (var role in roles) 
                {
                    // Using Identity Method to create a new role
                    _roleManager.CreateAsync(role).Wait(); // Use Wait(), because SeedUsers method returns void and it is not a async method
                }

                foreach (var user in users)
                {
                    // Not nencessary to use own Verification method if Identity is in place
                    // byte[] passwordHash, passwordSalt;
                    // CreatePasswordHash("password", out passwordHash, out passwordSalt);                    
                    // user.PasswordHash = passwordHash;
                    // user.PasswordSalt = passwordSalt;
                    // user.UserName = user.UserName.ToLower();

                    // Not nencessary to use own Verification method if Identity is in place
                    //_context.Users.Add(user);

                    // Using Identity Method to create a new user
                    _userManager.CreateAsync(user, "password").Wait(); // Use Wait(), because SeedUsers method returns void and it is not a async method
                    _userManager.AddToRoleAsync(user, "Member").Wait();
                }

                var adminUser = new User
                {
                    UserName = "Admin"
                };

                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator"}).Wait();
                }
                
                // Not nencessary to use own Verification method if Identity is in place
                // _context.SaveChanges();
            }            
        }

        
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));               
            }            
        }

    }
}