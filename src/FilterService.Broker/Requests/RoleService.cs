using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Broker.Requests.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.FilterService.Broker.Requests
{
  public class RoleService : IRoleService
  {
    private readonly ILogger<RoleService> _logger;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IRequestClient<IFilterRolesRequest> _rcGetRoles;

    public RoleService(
      ILogger<RoleService> logger,
      IGlobalCacheRepository globalCache,
      IRequestClient<IFilterRolesRequest> rcGetRoles)
    {
      _logger = logger;
      _globalCache = globalCache;
      _rcGetRoles = rcGetRoles;
    }

    public async Task<List<RoleFilteredData>> GetRolesFilterDataAsync(List<Guid> roleIds, List<string> errors)
    {
      if (roleIds is null || !roleIds.Any())
      {
        return null;
      }

      List<RoleFilteredData> rolesData = await _globalCache.GetAsync<List<RoleFilteredData>>(Cache.Rights, roleIds.GetRedisCacheHashCode());
      if (rolesData is null)
      {
        rolesData =
          (await RequestHandler.ProcessRequest<IFilterRolesRequest, IFilterRolesResponse>(
            _rcGetRoles,
            IFilterRolesRequest.CreateObj(roleIds),
            errors,
            _logger))
          .Roles;
      }

      return rolesData;
    }
  }
}
