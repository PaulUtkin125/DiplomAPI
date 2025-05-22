using Azure.Core;
using Diplom_Utkin.Model.Support;
using DiplomAPI.Data;
using DiplomAPI.Models.Support;
using Finansu.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private static readonly Imageporter _imageporter = new Imageporter();
        

        [HttpGet("NewBrokersList")]
        public async Task<ActionResult<List<Brokers>>> NewBrokersList()
        {
            using (var context = new dbContact())
            {
                var Broker = await context.Brokers.Include(x => x.Urisidikciiy).Where(x => x.TypeOfRequestId == 1 && x.isAdmitted == false && x.IsClosing == false).ToListAsync();
                foreach (var item in Broker)
                {
                    if (item.SourseFile != "")
                    {
                        item.SourseFile = _imageporter.porter(item.SourseFile);
                    }
                    
                }
                return Broker;
            }
        }

        [HttpGet("NotNewBrokersList")]
        public async Task<ActionResult<List<Brokers>>> NotNewBrokersList()
        {
            using (var context = new dbContact())
            {
                var Broker = await context.Brokers.Include(x => x.Urisidikciiy).Include(x => x.TypeOfRequest).Where(x => x.TypeOfRequestId != 1).ToListAsync();
                foreach (var item in Broker)
                {
                    if (item.SourseFile != "")
                    {
                        item.SourseFile = _imageporter.porter(item.SourseFile);
                    }

                }
                return Broker;
            }
        }

        [HttpPost("targetBroker")]
        public async Task<ActionResult> targetBroker([FromBody]ToolRequest Request)
        {
            using (var context = new dbContact())
            {
                var data = await context.Brokers.Include(x => x.Urisidikciiy).FirstAsync(x => x.Id == Request.id);
                if (data == null) return BadRequest();

                data.SourseFile = _imageporter.porter(data.SourseFile);
                return Ok(data);
            }
        }

        [HttpPost("ModefiteRequest")]
        public async Task<ActionResult> ModefiteRequest([FromBody]ModefiteRequestSupport modefite)
        {
            try
            {
                using (var context = new dbContact())
                {
                    var broker_Exist = await context.Brokers.FindAsync(modefite.brokerId);
                    if (modefite.mode == 0) // одобрить
                    {
                        broker_Exist.TypeOfRequestId = 2;
                    }
                    if (modefite.mode == 1) // отклонён
                    {
                        broker_Exist.TypeOfRequestId = 3;
                    }
                    context.Entry(broker_Exist).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("AllUserlist")]
        public async Task<ActionResult<List<User>>> AllUser([FromBody]int userId)
        {
            using (var context = new dbContact())
            {
                var Users = await context.User.Include(x => x.TypeOfUser).Where(x => x.Id != userId && x.TypeOfUserId != 4).ToListAsync();
                return Ok(Users);
            }
        }

        [HttpPost("targetUser")]
        public async Task<ActionResult> targetUser([FromBody] ToolRequest Request)
        {
            using (var context = new dbContact())
            {
                var data = await context.User.Include(x => x.TypeOfUser).FirstAsync(x => x.Id == Request.id);
                if (data == null) return BadRequest();

                return Ok(data);
            }
        }

        [HttpGet("AllUserType")]
        public async Task<ActionResult<List<TypeOfUser>>> AllUserType()
        {
            try
            {
                using (var context = new dbContact())
                {
                    var resalt = await context.typeOfUser.ToListAsync();
                    return Ok(resalt);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPatch("deleteUser")]
        public async Task<ActionResult> DeleteUser([FromBody]int id)
        {
            using (var context = new dbContact())
            {
                try
                {
                    var targetUser = await context.User.FindAsync(id);
                    targetUser.TypeOfUserId = 4;

                    context.Entry(targetUser).State = EntityState.Modified;
                    context.SaveChanges();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }

        [HttpPatch("updateUser")]
        public async Task<ActionResult> updateUser([FromBody]User request)
        {
            try
            {
                using (var context = new dbContact())
                {
                    var user_Exist = await context.User.FindAsync(request.Id);

                    if (request.TypeOfUserId != 0 &&
                        request.TypeOfUserId != user_Exist.TypeOfUserId) user_Exist.TypeOfUserId = request.TypeOfUserId;

                    if (request.Loggin != null && request.Loggin != user_Exist.Loggin) user_Exist.Loggin = request.Loggin;
                    if (request.Phone != null && request.Phone != user_Exist.Phone) user_Exist.Phone = request.Phone;

                    context.Entry(user_Exist).State = EntityState.Modified;
                    context.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
