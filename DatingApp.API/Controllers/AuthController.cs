using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers 
{
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController (IAuthRepository repo, IConfiguration config) 
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto) // [FromBody] is not required, because its done already with [ApiController]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            // validate request not required, because it is already done with [ApiController] attribute  
            // if (!ModelState.IsValid) return BadRequest(ModelState);  // Checking validation whether passed data with UserForRegisterDto is valid - means checking [Required] or [StringLength] fields / given values.

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username)) return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201); // fix later to CreatedAtRoute
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // 1. Check if the given user (name+password) exists in the database
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            // if not, return Unauthorized without given a Hint about the reason (no info regarding what is wrong - name or password)
            if (userFromRepo == null) return Unauthorized();

            // 2. Build Token

            // 2.1 Create claims (id, name) - type of the claim along with the value from the database
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // 2.2 In order to make sure that a token is valid, when it comes back, the server needs to SIGN this token.
            // 2.2(a) for this, firt of all a SECURITY KEY need to be created
            // Remark: Use random generated very long string of characters in the AppSettings (see appsettings.json file)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            // 2.2(b) use created KEY as part of the Signing. The Key need to be encrypted with the Hashing Algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // 2.3 Start to create a TOKEN
            
            // 2.3(a) Create a TOKEN Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // passing the claims created above
                Expires = DateTime.Now.AddDays(1), // Give a Date of Expiration for this token
                SigningCredentials = creds // passing the Signing Credentials that have been created above
            };
            
            // 2.3(b) Create Token handler which allows to create a Token object
            var tokenHandler = new JwtSecurityTokenHandler();
            // 2.3(c) Create TOKEN object by using the Token handler and passing the tokenDescriptor object
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            // 3. Return JWT token to the client, so that it can be used or the authentication for any further requests
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });

        }
    }
}