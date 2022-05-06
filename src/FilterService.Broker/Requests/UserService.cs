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
      UserFilter filter,
      PaginationValues value,
      List<string> errors)
    {
      List<UserData> usersData = null;
      int usersCount = 0;

      if (filter.FullNameIncludeSubstring is null)
      {
        if (filter.IsAscendingSort.HasValue)
        {
          (usersData, usersCount) =
            await _globalCache.GetAsync<(List<UserData> usersData, int usersCount)>
              (Cache.Users, usersIds.GetRedisCacheHashCode(
                value.SkipCount,
                value.TakeCount,
                filter.IsAscendingSort));
        }
        else
        {
          (usersData, usersCount) =
              await _globalCache.GetAsync<(List<UserData> usersData, int usersCount)>
                (Cache.Users, usersIds.GetRedisCacheHashCode(
                  value.SkipCount,
                  value.TakeCount));
        }
      }

      if (usersData is null)
      {
        IFilteredUsersDataResponse usersDataResponse =
          (await RequestHandler.ProcessRequest<IFilteredUsersDataRequest, IFilteredUsersDataResponse>(
            _rcGetUsers,
            IFilteredUsersDataRequest.CreateObj(
              usersIds,
              value.SkipCount,
              value.TakeCount,
              filter.IsAscendingSort,
              filter.FullNameIncludeSubstring),
            errors,
            _logger));

        usersData = usersDataResponse is not null ? usersDataResponse.UsersData : usersData;
        usersCount = usersDataResponse is not null ? usersDataResponse.TotalCount : usersCount;
      }

      return (usersData, usersCount);
    }
  }
}
