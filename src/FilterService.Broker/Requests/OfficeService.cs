using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Broker.Requests.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Responses.Office;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.FilterService.Broker.Requests
{
  public class OfficeService : IOfficeService
  {
    private readonly ILogger<OfficeService> _logger;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IRequestClient<IFilterOfficesRequest> _rcGetOffices;

    public OfficeService(
      ILogger<OfficeService> logger,
      IGlobalCacheRepository globalCache,
      IRequestClient<IFilterOfficesRequest> rcGetOffices)
    {
      _logger = logger;
      _globalCache = globalCache;
      _rcGetOffices = rcGetOffices;
    }

    public async Task<List<OfficeFilteredData>> GetOfficeFilteredDataAsync(List<Guid> officesIds, List<string> errors)
    {
      if (officesIds is null || !officesIds.Any())
      {
        return null;
      }

      List<OfficeFilteredData> officesData = await _globalCache.GetAsync<List<OfficeFilteredData>>(Cache.Offices, officesIds.GetRedisCacheHashCode());

      if (officesData is null)
      {
        officesData =
          (await RequestHandler.ProcessRequest<IFilterOfficesRequest, IFilterOfficesResponse>(
            _rcGetOffices,
            IFilterOfficesRequest.CreateObj(officesIds),
            errors,
            _logger))
          ?.Offices;
      }
      if (officesData is null)
      {
        errors.Add("Can not filter by offices.");
      }

      return officesData;
    }
  }
}
