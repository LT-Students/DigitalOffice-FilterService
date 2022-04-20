using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Broker.Requests.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.FilterService.Broker.Requests
{
  public class PositionService : IPositionService
  {
    private readonly ILogger<PositionService> _logger;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IRequestClient<IFilterPositionsRequest> _rcGetPosotions;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositionsData;

    public PositionService(
      ILogger<PositionService> logger,
      IGlobalCacheRepository globalCache,
      IRequestClient<IFilterPositionsRequest> rcGetPosotions,
      IRequestClient<IGetPositionsRequest> rcGetPositionsData)
    {
      _logger = logger;
      _globalCache = globalCache;
      _rcGetPosotions = rcGetPosotions;
      _rcGetPositionsData = rcGetPositionsData;
    }

    public async Task<List<PositionFilteredData>> GetPositionFilterDataAsync(List<Guid> positionsIds, List<string> errors)
    {
      if (positionsIds is null || !positionsIds.Any())
      {
        return null;
      }

      List<PositionFilteredData> positionsData = await _globalCache.GetAsync<List<PositionFilteredData>>(Cache.Positions, positionsIds.GetRedisCacheHashCode());

      if (positionsData is null)
      {
        positionsData =
          (await RequestHandler.ProcessRequest<IFilterPositionsRequest, IFilterPositionsResponse>(
            _rcGetPosotions,
            IFilterPositionsRequest.CreateObj(positionsIds),
            errors,
            _logger))
          ?.Positions;
      }
      if (positionsData is null)
      {
        errors.Add("Can not filter by positions.");
      }

      return positionsData;
    }

    public async Task<List<PositionData>> GetPositionsDataAsync(List<Guid> usersIds, List<string> errors)
    {
      if (!usersIds.Any())
      {
        return null;
      }

      List<PositionData> positionsData = await _globalCache.GetAsync<List<PositionData>>(Cache.Positions, usersIds.GetRedisCacheHashCode());

      if (positionsData is null)
      {
        positionsData =
          (await RequestHandler.ProcessRequest<IGetPositionsRequest, IGetPositionsResponse>(
            _rcGetPositionsData,
            IGetPositionsRequest.CreateObj(usersIds),
            errors,
            _logger))
          ?.Positions;
      }

      return positionsData;
    }
  }
}
