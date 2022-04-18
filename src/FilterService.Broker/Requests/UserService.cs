using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Broker.Requests.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Request.UserService;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.User;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.FilterService.Broker.Requests
{
  public class UserService : IUserService
  {
    private readonly ILogger<UserService> _logger;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IRequestClient<IFilteredUsersDataRequest> _rcGetUsers;

    public UserService(
      ILogger<UserService> logger,
      IGlobalCacheRepository globalCache,
      IRequestClient<IFilteredUsersDataRequest> rcGetUsers)
    {
      _logger = logger;
      _globalCache = globalCache;
      _rcGetUsers = rcGetUsers;
    }

    public async Task<(List<UserData> usersData, int usersCount)> GetFilteredUsersDataAsync(
      List<Guid> usersIds,
      PaginationValues value,
      List<string> errors)
    {
      (List<UserData> usersData, int usersCount) =
        await _globalCache.GetAsync<(List<UserData> usersData, int usersCount)>
          (Cache.Users, usersIds.GetRedisCacheHashCode());

      if (usersData is null || !usersIds.Any())
      {
        IFilteredUsersDataResponse usersDataResponse =
          (await RequestHandler.ProcessRequest<IFilteredUsersDataRequest, IFilteredUsersDataResponse>(
            _rcGetUsers,
            IFilteredUsersDataRequest.CreateObj(usersIds, value.SkipCount, value.TakeCount),
            errors,
            _logger));

        usersData = usersDataResponse is not null ? usersDataResponse.UsersData : new();
        usersCount = usersDataResponse is not null ? usersDataResponse.TotalCount : 0;
      }

      return (usersData, usersCount);
    }
  }
}
