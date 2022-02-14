using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.FilterService.Models.Dto.Request.UserService;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.FilterService.Business.Commands.User.Interfaces
{
  [AutoInject]
  public interface IUserServiceFilterCommand
  {
    Task<FindResultResponse<UserInfo>> ExecuteAsync(FilterUserService filter);
  }
}
