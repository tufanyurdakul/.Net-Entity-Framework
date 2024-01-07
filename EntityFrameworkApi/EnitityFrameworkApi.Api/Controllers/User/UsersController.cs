using EntityFrameworkApi.Database;
using EntityFrameworkApi.Model.Api.User;
using EntityFrameworkApi.Model.Database.User;
using EntityFrameworkApi.Model.Shared.Api;
using EntityFrameworkApi.Utilities;
using EntityFrameworkApi.Utilities.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EntityFrameworkApi.Api.Controllers.User
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly AppSettings _appSettings;
        public UsersController(DatabaseContext context, AppSettings appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.Where(x => !x.IsDeleted).ToListAsync();
            var respModel = users.Select(x => new UserModel()
            {
                Id = x.Id,
                Age = x.Age,
                BirthDate = x.BirthDate,
                Gender = x.Gender.ToString().ToLower().Equals("m") ? "man" : "woman",
                Name = x.Name,
                IsMarried = x.IsMarried
            });

            return Ok(respModel);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(long userId)
        {
            TokenModel tokenModel = Functions.ParseTokenWithoutAdmin(HttpContext.User.Claims, userId);

            var user = await _context.Users.Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException("user not found");

            var respModel = new UserModel()
            {
                Age = user.Age,
                BirthDate = user.BirthDate,
                Gender = user.Gender.ToString().ToLower().Equals("m") ? "man" : "woman",
                Id = user.Id,
                Name = user.Name,
                IsMarried = user.IsMarried,
            };

            return Ok(respModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(UserSignModel model)
        {
            var user = await _context.Users.Where(x => x.Name.Equals(model.Name) && !x.IsDeleted).FirstOrDefaultAsync();
            if (user != null)
                throw new InvalidDataException("user is defined on system");

            UserDto userDto = new UserDto()
            {
                Age = (short)model.Age,
                BirthDate = model.BirthDate,
                CreateDate = DateTime.UtcNow,
                CreatorUserId = 1,
                Gender = model.Gender.ToLower().Equals("man") ? 'M' : 'W',
                IsActive = true,
                IsDeleted = false,
                IsMarried = model.IsMarried,
                Name = model.Name,
                Password = model.Password,
            };

            await _context.Users.AddAsync(userDto);
            await _context.SaveChangesAsync();

            return Created("users", userDto);
        }

        [HttpPut("userId")]
        public async Task<IActionResult> Update(long userId, UserUpdateModel model)
        {
            TokenModel tokenModel = Functions.ParseToken(HttpContext.User.Claims);

            var user = await _context.Users.Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException("user not found");

            bool sameNameControl = await _context.Users.AnyAsync(x => x.Id != userId && x.Name.ToLower().Equals(model.Name.ToLower()));
            if (sameNameControl)
                throw new InvalidDataException("username is defined on system");

            user.Age = (short)model.Age;
            user.BirthDate = model.BirthDate;
            user.IsMarried = model.IsMarried;
            user.Name = model.Name;
            user.Gender = model.Gender.ToLower().Equals("man") ? 'M' : 'W';
            user.ModifiedDate = DateTime.UtcNow;
            user.ModifierUserId = tokenModel.Id;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("userId")]
        public async Task<IActionResult> Delete(long userId)
        {
            TokenModel tokenModel = Functions.ParseTokenWithoutAdmin(HttpContext.User.Claims, userId);

            var user = await _context.Users.Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException("user not found");

            user.IsDeleted = true;
            user.ModifiedDate = DateTime.UtcNow;
            user.ModifierUserId = tokenModel.Id;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("authentication")]
        public async Task<IActionResult> Authentication(UserLoginModel model)
        {
            var user = await _context.Users.Where(x => x.Name.Equals(model.Name) && !x.IsDeleted).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException("User or password not match");

            if (!model.Password.Equals(user.Password))
                throw new KeyNotFoundException("User or password not match");

            if (!user.IsActive)
                throw new KeyNotFoundException("User is not active");

            #region Token Handler

            List<Claim> claims = new List<Claim>();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);


            claims.Add(new Claim("id", user.Id.ToString()));
            claims.Add(new Claim("name", user.Name));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "your_issuer",
                Audience = "your_audience"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            #endregion

            return Ok(token);
        }
    }
}
