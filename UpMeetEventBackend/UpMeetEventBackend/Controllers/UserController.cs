using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using UpMeetEventBackend.Models;
using UpMeetEventBackend.Models.DTOs;
using UpMeetEventBackend.Models.DTOs.EventUserDTOs;
using UpMeetEventBackend.Models.DTOs.UserDTOs;
using UpMeetEventBackend.Models.PublicClasses;

namespace UpMeetEventBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private UpMeetDbContext dbContext = new UpMeetDbContext();
        private UploadHandler uploader = new UploadHandler();

        static UserDTO convertUserDTO(User u)
        {
            return new UserDTO
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Bio = u.Bio,
                ImageId = u.ImageId,
                Active = u.Active,
                Image = convertImageDTO(u.Image),
                Events = u.Events.Select(e => convertEventDTO(e)).ToList()
            };
        }

        static ImageDTO convertImageDTO(Image? i = null)
        {
            if (i == null)
            {
                return null;
            }
            return new ImageDTO
            {
                ImageId = i.ImageId,
                Path = i.Path
            };
        }

        static BasicEventDTO convertEventDTO(Event e)
        {
            return new BasicEventDTO
            {
                EventId = e.EventId,
                Name = e.Name,
                Description = e.Description,
                ImageId = e.ImageId,
                Image = convertImageDTO(e.Image),
                Expired = e.Expired,
                Active = e.Active,
            };
        }

        [HttpGet]
        public IActionResult getAllUsers(string? username = null)
        {
            List<UserDTO> result = dbContext.Users.Include(e => e.Events.Where(e => e.Active == true)).ThenInclude(i => i.Image).Include(i => i.Image).Where(u => u.Active == true).Select(u => convertUserDTO(u)).ToList();

            if(username != null)
            {
                result = result.Where(u => u.UserName.ToLower().Contains(username.ToLower())).ToList();
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult getById(int id)
        {
            User result = dbContext.Users.Include(e => e.Events.Where(e => e.Active == true)).ThenInclude(i => i.Image).Include(i => i.Image).FirstOrDefault(u => u.UserId == id);

            if(result == null || result.Active == false)
            {
                return NotFound("User Not Found");
            }

            return Ok(convertUserDTO(result));
        }

        [HttpGet("Login")]
        public IActionResult login(string username, string password)
        {
            User result = dbContext.Users.Include(e => e.Events.Where(e => e.Active == true)).ThenInclude(i => i.Image).Include(i => i.Image).FirstOrDefault(u => u.UserName == username && u.Password == password);

            if(result == null || result.Active == false)
            {
                return NotFound("Username or Password was incorrect");
            }

            return Ok(convertUserDTO(result));
        }

        [HttpPost]
        public IActionResult addNewUser([FromForm] PostUserDTO newUser)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            User newUserDB = new User();

            newUserDB.UserId = 0;
            newUserDB.FirstName = newUser.FirstName;
            newUserDB.LastName = newUser.LastName;
            newUserDB.UserName = newUser.UserName;
            if(newUser.Bio != null)
            {
                newUserDB.Bio = newUser.Bio;
            }
            newUserDB.Active = true;
            newUserDB.Password = newUser.Password;
            if(newUser.Image != null)
            {
                Image newImage = uploader.Upload(newUser.Image);
                newUserDB.ImageId = newImage.ImageId;
                newUserDB.Image = dbContext.Images.Find(newUserDB.ImageId);
            }
            

            dbContext.Users.Add(newUserDB);
            dbContext.SaveChanges();

            

            return CreatedAtAction(nameof(getById), new {id = newUserDB.UserId}, convertUserDTO(newUserDB));
        }


        
        [HttpPut("{id}")]
        public IActionResult updateUserInfo([FromForm] PutUserDTO targetUser, int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(!dbContext.Users.Any(u => u.UserId == id) || dbContext.Users.Find(id).Active == false) 
            {
                return NotFound("User Not Found");
            }

            User updateUser = dbContext.Users.Include(e => e.Events.Where(e => e.Active == true)).ThenInclude(i => i.Image).Include(i => i.Image).FirstOrDefault(u => u.UserId == id);

            if (targetUser.FirstName != null)
            {
                updateUser.FirstName = targetUser.FirstName;
            }
            if(targetUser.LastName != null)
            {
                updateUser.LastName = targetUser.LastName;
            }
            if(targetUser.UserName != null)
            {
                updateUser.UserName = targetUser.UserName;
            }
            if(targetUser.Bio != null)
            {
                updateUser.Bio = targetUser.Bio;
            }
            if(targetUser.Password != null)
            {
                updateUser.Password = targetUser.Password;
            }
            if (targetUser.Image != null)
            {
                Image newImage = uploader.Upload(targetUser.Image);
                if(updateUser.Image != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), updateUser.Image.Path)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), updateUser.Image.Path));
                    dbContext.Images.Remove(updateUser.Image);
                }
                updateUser.ImageId = newImage.ImageId;
                updateUser.Image = dbContext.Images.Find(updateUser.ImageId);
            }
            

            dbContext.Users.Update(updateUser);
            dbContext.SaveChanges();

            return Ok(convertUserDTO(updateUser));
        }

        //Should Be GET??
        [HttpPut("AddToFavorites")]
        public IActionResult addFavoriteEvent([FromBody] UserFavorite userFav)
        {
            User Uresult = dbContext.Users.Include(i => i.Image).Include(e => e.Events.Where(e => e.Active == true)).ThenInclude(i => i.Image).FirstOrDefault(u => u.UserId == userFav.UserId);
            Event Eresult = dbContext.Events.Include(i => i.Image).Include(u => u.Users).ThenInclude(i => i.Image).FirstOrDefault(e => e.EventId == userFav.EventId);

            if (!Uresult.Events.Any(e => e.EventId == userFav.EventId))
            {
                dbContext.Users.Find(userFav.UserId).Events.Add(Eresult);
            }
            if(!Eresult.Users.Any(u => u.UserId  == userFav.UserId))
            {
                dbContext.Events.Find(userFav.EventId).Users.Add(Uresult);
            }
            
            dbContext.SaveChanges();

            return Ok(convertUserDTO(Uresult));
        }

        
        [HttpPut("RemoveFavorites")]
        public IActionResult deleteFavoriteEvent([FromBody] UserFavorite userFav)
        {
            User Uresult = dbContext.Users.Include(i => i.Image).Include(e => e.Events.Where(e => e.Active == true)).ThenInclude(i => i.Image).FirstOrDefault(u => u.UserId == userFav.UserId);
            Event Eresult = dbContext.Events.Include(i => i.Image).Include(u => u.Users).ThenInclude(i => i.Image).FirstOrDefault(e => e.EventId == userFav.EventId);

            if (Uresult.Events.Any(e => e.EventId == userFav.EventId))
            {
                dbContext.Users.Find(userFav.UserId).Events.Remove(Eresult);
            }
            if (Eresult.Users.Any(u => u.UserId == userFav.UserId))
            {
                dbContext.Events.Find(userFav.EventId).Users.Remove(Uresult);
            }


            dbContext.SaveChanges();

            return Ok(convertUserDTO(Uresult));
        }

        [HttpDelete("{id}")]
        public IActionResult removeUser(int id)
        {
            User result = dbContext.Users.Find(id);

            if(result == null || result.Active == false)
            {
                return NotFound("User Not Found");
            }

            result.Active = false;



            dbContext.Users.Update(result);
            dbContext.SaveChanges();

            return NoContent();
        }
    }
}
