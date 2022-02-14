using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.FilterService.Models.Dto.Request.UserService;
using LT.DigitalOffice.Kernel.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.FilterService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UserServiceController : ControllerBase
  {
    [HttpGet("filter")]
    public async Task<FindResultResponse<UserInfo>> GetAsync(
      [FromServices] IFilterUsersCommand command,
      [FromQuery] FilterUserService filter)
    {
      return await command.ExecuteAsync(filter);
    }
  }
}
