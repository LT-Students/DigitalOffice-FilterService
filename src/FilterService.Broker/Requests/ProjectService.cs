using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Broker.Requests.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.FilterService.Broker.Requests
{
  public class ProjectService : IProjectService
  {
    private readonly ILogger<ProjectService> _logger;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IRequestClient<IGetProjectsRequest> _rcGetProjects;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProjectService(
      ILogger<ProjectService> logger,
      IGlobalCacheRepository globalCache,
      IRequestClient<IGetProjectsRequest> rcGetProjects,
      IHttpContextAccessor httpContextAccessor)
    {
      _logger = logger;
      _globalCache = globalCache;
      _rcGetProjects = rcGetProjects;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ProjectData>> GetProjectsDataAsync(List<Guid> usersProjectsIds, List<string> errors)
    {
      if (usersProjectsIds is null || !usersProjectsIds.Any())
      {
        return null;
      }

      object request = IGetProjectsRequest.CreateObj(projectsIds: usersProjectsIds, includeUsers: true);

      List<ProjectData> projectsData =
        await _globalCache.GetAsync<List<ProjectData>>(Cache.Projects, usersProjectsIds.GetRedisCacheKey(
          nameof(IGetProjectsRequest), request.GetBasicProperties()));

      if (projectsData is null)
      {
        projectsData =
          (await RequestHandler.ProcessRequest<IGetProjectsRequest, IGetProjectsResponse>(
            _rcGetProjects,
            request,
            errors,
            _logger))
          ?.Projects;
      }
      if (projectsData is null)
      {
        errors.Add("Can not filter by projects.");
      }

      return projectsData;
    }
  }
}
