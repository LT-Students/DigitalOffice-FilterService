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
    private readonly IRequestClient<IFilteredUsersDataRequest> _rcGetUsers;

    private string CreateRedisKey(
      List<Guid> usersIds,
      int skipCount,
      int takeCount,
      bool? ascendingSort,
      string fullNameIncludeSubstring)
    {
      List<object> additionalArgs = new() { skipCount, takeCount };

      if (ascendingSort.HasValue)
      {
        additionalArgs.Add(ascendingSort.Value);
      }

      if (!string.IsNullOrEmpty(fullNameIncludeSubstring))
      {
        additionalArgs.Add(fullNameIncludeSubstring);
      }

      var key = usersIds.GetRedisCacheHashCode(additionalArgs.ToArray());

      return key;
    }

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
      PaginationValues values,
      List<string> errors)
    {
      List<UserData> usersData;
      int usersCount = 0;

      (usersData, usersCount) = await _globalCache.GetAsync<(List<UserData> usersData, int usersCount)>(
        Cache.Users,
        CreateRedisKey(usersIds, values.SkipCount, values.TakeCount, filter.IsAscendingSort, filter.FullNameIncludeSubstring));

      if (usersData is null)
      {
        IFilteredUsersDataResponse usersDataResponse =
          (await RequestHandler.ProcessRequest<IFilteredUsersDataRequest, IFilteredUsersDataResponse>(
            _rcGetUsers,
            IFilteredUsersDataRequest.CreateObj(
              usersIds: usersIds,
              skipCount: values.SkipCount,
              takeCount: values.TakeCount,
              ascendingSort: filter.IsAscendingSort,
              fullNameIncludeSubstring: filter.FullNameIncludeSubstring),
            errors,
            _logger));

        usersData = usersDataResponse is not null ? usersDataResponse.UsersData : usersData;
        usersCount = usersDataResponse is not null ? usersDataResponse.TotalCount : usersCount;
      }

      return (usersData, usersCount);
    }
  }
}
