using EntityFrameworkApi.Database;
using EntityFrameworkApi.Model.Api.User;
using EntityFrameworkApi.Model.Database.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkApi.Api.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseContext _context;
        public UsersController(DatabaseContext context)
        {
            _context = context;
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
            var user = await _context.Users.Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException("user not found");

            return Ok(user);
        }

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
            user.ModifierUserId = user.Id;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("userId")]
        public async Task<IActionResult> Delete(long userId)
        {
            var user = await _context.Users.Where(x => x.Id == userId && !x.IsDeleted).FirstOrDefaultAsync();
            if (user == null)
                throw new KeyNotFoundException("user not found");

            user.IsDeleted = true;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
