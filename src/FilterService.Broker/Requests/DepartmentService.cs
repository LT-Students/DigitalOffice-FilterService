using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Broker.Requests.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.FilterService.Broker.Requests
{
  public class DepartmentService : IDepartmentService
  {
    private readonly ILogger<DepartmentService> _logger;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IRequestClient<IFilterDepartmentsRequest> _rcGetDepartments;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartmentsData;

    public DepartmentService(
      ILogger<DepartmentService> logger,
      IGlobalCacheRepository globalCache,
      IRequestClient<IFilterDepartmentsRequest> rcGetDepartments,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartmentsData)
    {
      _logger = logger;
      _globalCache = globalCache;
      _rcGetDepartments = rcGetDepartments;
      _rcGetDepartmentsData = rcGetDepartmentsData;
    }

    public async Task<List<DepartmentFilteredData>> GetDepartmentFiltedDataAsync(List<Guid> departmentsIds, List<string> errors)
    {
      if (departmentsIds is null || !departmentsIds.Any())
      {
        return null;
      }

      List<DepartmentFilteredData> departmentsData = await _globalCache.GetAsync<List<DepartmentFilteredData>>(Cache.Departments, departmentsIds.GetRedisCacheHashCode());

      if (departmentsData is null)
      {
        departmentsData =
          (await RequestHandler.ProcessRequest<IFilterDepartmentsRequest, IFilterDepartmentsResponse>(
            _rcGetDepartments,
            IFilterDepartmentsRequest.CreateObj(departmentsIds),
            errors,
            _logger))
          ?.Departments;
      }
      if (departmentsData is null)
      {
        errors.Add("Can not filter by departments.");
      }

      return departmentsData;
    }

    public async Task<List<DepartmentData>> GetDepartmentsDataAsync(List<Guid> usersIds, List<string> errors)
    {
      if (!usersIds.Any())
      {
        return null;
      }

      List<DepartmentData> departmentsData = await _globalCache.GetAsync<List<DepartmentData>>(Cache.Departments, usersIds.GetRedisCacheHashCode());

      if (departmentsData is null)
      {
        departmentsData =
          (await RequestHandler.ProcessRequest<IGetDepartmentsRequest, IGetDepartmentsResponse>(
            _rcGetDepartmentsData,
            IGetDepartmentsRequest.CreateObj(usersIds),
            errors,
            _logger))
          ?.Departments;
      }
      
      return departmentsData;
    }
  }
}
