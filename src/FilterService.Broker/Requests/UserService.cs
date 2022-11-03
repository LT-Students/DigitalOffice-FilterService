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

      object request = IFilteredUsersDataRequest.CreateObj(
        usersIds: usersIds,
        skipCount: values.SkipCount,
        takeCount: values.TakeCount,
        ascendingSort: filter.IsAscendingSort,
        fullNameIncludeSubstring: filter.FullNameIncludeSubstring);

      (usersData, usersCount) =
        await _globalCache.GetAsync<(List<UserData> usersData, int usersCount)>(Cache.Users, usersIds.GetRedisCacheKey(
          nameof(IFilteredUsersDataRequest), request.GetBasicProperties()));

      if (usersData is null)
      {
        IFilteredUsersDataResponse usersDataResponse =
          (await RequestHandler.ProcessRequest<IFilteredUsersDataRequest, IFilteredUsersDataResponse>(
            _rcGetUsers,
            request,
            errors,
            _logger));

        usersData = usersDataResponse?.UsersData ?? usersData;
        usersCount = usersDataResponse?.TotalCount ?? usersCount;
      }

      return (usersData, usersCount);
    }
  }
}
