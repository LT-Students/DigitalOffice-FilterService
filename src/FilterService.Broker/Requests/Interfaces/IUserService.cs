using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Models.Dto.Request.UserService;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;

namespace LT.DigitalOffice.FilterService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IUserService
  {
    Task<(List<UserData> usersData, int usersCount)> GetFilteredUsersDataAsync(
      List<Guid> usersIds,
      UserFilter filter,
      PaginationValues value,
      List<string> errors);
  }
}
