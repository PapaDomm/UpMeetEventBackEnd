using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UpMeetEventBackend.Models;
using UpMeetEventBackend.Models.DTOs;
using UpMeetEventBackend.Models.DTOs.EventUserDTOs;
using UpMeetEventBackend.Models.DTOs.UserDTOs;
using UpMeetEventBackend.Models.PublicClasses;

namespace UpMeetEventBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private UpMeetDbContext dbContext = new UpMeetDbContext();
        private UploadHandler uploader = new UploadHandler();

        static BasicUserDTO convertBasicUserDTO(User u)
        {
            return new BasicUserDTO
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Bio = u.Bio,
                Image = convertImageDTO(u.Image)
            };
        }

        static ImageDTO convertImageDTO(Image? i = null)
        {
            if(i == null)
            {
                return null;
            }
            return new ImageDTO
            {
                ImageId = i.ImageId,
                Path = i.Path
            };
        }

        static EventDTO convertEventDTO(Event e)
        {
            return new EventDTO
            {
                EventId = e.EventId,
                Name = e.Name,
                Description = e.Description,
                ImageId = e.ImageId,
                Image = convertImageDTO(e.Image),
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Expired = e.Expired,
                Active = e.Active,
                City = e.City,
                State = e.State,
                Users = e.Users.Select(u => convertBasicUserDTO(u)).ToList()
            };
        }

        //Implement FromQuery?
        //Implement More Indepth Querying
        [HttpGet]
        public IActionResult getAllEvents(string? name = null, DateTime? startdate = null, DateTime? enddate = null, string? city = null, string? state = null, bool? expired = null)
        {
            List<EventDTO> result = dbContext.Events.Include(u => u.Users.Where(u => u.Active == true)).ThenInclude(i => i.Image).Include(i => i.Image).Where(e => e.Active == true).Select(e => convertEventDTO(e)).ToList();

            
            if (name != null)
            {
                result = result.Where(e => e.Name.ToLower().Contains(name.ToLower())).ToList();
            }
            //Fix dates logic should remove those not in bounds
            if(startdate != null)
            {
                result = result.Where(e => e.StartDate >  startdate).ToList();
            }
            if(enddate != null)
            {
                result = result.Where(e => e.EndDate < enddate).ToList();
            }
            if(city != null)
            {
                result = result.Where(e => e.City.ToLower().Contains(city.ToLower())).ToList();
            }
            if (state != null)
            {
                result = result.Where(e => e.State.ToLower() == state.ToLower()).ToList();
            }
            if(expired != null)
            {
                result = result.Where(e => e.Expired == expired).ToList();
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult getById(int id)
        {
            Event result = dbContext.Events.Include(u => u.Users.Where(u => u.Active == true)).ThenInclude(i => i.Image).Include(i => i.Image).FirstOrDefault(e => e.EventId == id);

            if(result == null || result.Active == false)
            {
                return NotFound("Event Not Found");
            }

            return Ok(convertEventDTO(result));
        }

        //Possible Validation on Dates
        [HttpPut("{id}")]
        public IActionResult updateEventInfo([FromForm] PutEventDTO targetEvent, int id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(!dbContext.Events.Any(e => e.EventId == id) || dbContext.Events.Find(id).Active == false) 
            {
                return NotFound("Event Not Found");
            }

            Event updateEvent = dbContext.Events.Include(u => u.Users.Where(u => u.Active == true)).ThenInclude(i => i.Image).Include(i => i.Image).FirstOrDefault(e => e.EventId == id);


            if (targetEvent.Name != null)
            {
                updateEvent.Name = targetEvent.Name;
            }
            if(targetEvent.Description != null)
            {
                updateEvent.Description = targetEvent.Description;
            }
            if(targetEvent.StartDate.HasValue)
            {
                updateEvent.StartDate = (DateTime)targetEvent.StartDate;
            }
            if(targetEvent.EndDate.HasValue)
            {
                updateEvent.EndDate = (DateTime)targetEvent.EndDate;
            }
            if(targetEvent.Expired.HasValue)
            {
                updateEvent.Expired = (bool)targetEvent.Expired;
            }
            if(targetEvent.City != null)
            {
                updateEvent.City = targetEvent.City;
            }
            if(targetEvent.State != null)
            {
                updateEvent.State = targetEvent.State;
            }
            if(targetEvent.Image != null)
            {
                Image newImage = uploader.Upload(targetEvent.Image);
                if(updateEvent.Image != null && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), updateEvent.Image.Path)))
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), updateEvent.Image.Path));
                    dbContext.Images.Remove(updateEvent.Image);
                }
                
                updateEvent.ImageId = newImage.ImageId;
                updateEvent.Image = dbContext.Images.Find(newImage.ImageId);
            }

            dbContext.Events.Update(updateEvent);
            dbContext.SaveChanges();

            return Ok(convertEventDTO(updateEvent));            
        }

        
        [HttpPost]
        public IActionResult addNewEvent([FromForm] PostEventDTO newEvent)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            Event newEventDB = new Event();

            
            //Possible Validation on Dates
            newEventDB.EventId = 0;
            newEventDB.Name = newEvent.Name;
            newEventDB.Description = newEvent.Description;
            newEventDB.Active = true;
            newEventDB.StartDate = newEvent.StartDate;
            newEventDB.EndDate = newEvent.EndDate;
            newEventDB.Expired = newEvent.Expired;
            newEventDB.City = newEvent.City;
            newEventDB.State = newEvent.State;
            if(newEvent.Image != null)
            {
                Image newImage = uploader.Upload(newEvent.Image);
                newEventDB.ImageId = newImage.ImageId;
                newEventDB.Image = dbContext.Images.Find(newImage.ImageId);
            }

            dbContext.Events.Add(newEventDB);
            dbContext.SaveChanges();

            Event returnEvent = dbContext.Events.Include(u => u.Users.Where(u => u.Active == true)).ThenInclude(i => i.Image).Include(i => i.Image).FirstOrDefault(e => e.EventId == newEventDB.EventId);

            return CreatedAtAction(nameof(getById), new {id = newEventDB.EventId}, convertEventDTO(returnEvent));
        }

        [HttpDelete("{id}")]
        public IActionResult removeEvent(int id)
        {
            Event result = dbContext.Events.Find(id);

            if(result == null || result.Active == false)
            {
                return NotFound("Event Not Found");
            }

            result.Active = false;

            dbContext.Events.Update(result); 
            dbContext.SaveChanges();

            return NoContent();
        }
    }
}
