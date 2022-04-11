using System;
using System.Collections.Generic;
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
    private readonly IRequestClient<IGetUsersDataRequest> _rcGetUsers;

    public UserService(
      ILogger<UserService> logger,
      IGlobalCacheRepository globalCache,
      IRequestClient<IGetUsersDataRequest> rcGetUsers)
    {
      _logger = logger;
      _globalCache = globalCache;
      _rcGetUsers = rcGetUsers;
    }

    public async Task<List<UserData>> GetFilteredUsersDataAsync(
      List<Guid> usersIds,
      PaginationValues value,
      List<string> errors)
    {
      if (usersIds is null)
      {
        return null;
      }

      List<UserData> usersData = await _globalCache.GetAsync<List<UserData>>(Cache.Users, usersIds.GetRedisCacheHashCode());

      if (usersData is null)
      {
        usersData =
          (await RequestHandler.ProcessRequest<IGetUsersDataRequest, IGetUsersDataResponse>(
            _rcGetUsers,
            IGetUsersDataRequest.CreateObj(usersIds, value.SkipCount, value.TakeCount),
            errors,
            _logger))
          ?.UsersData;
      }
      if (usersData is null)
      {
        errors.Add("Cannot get Users");
      }

      return usersData;
    }
  }
}
