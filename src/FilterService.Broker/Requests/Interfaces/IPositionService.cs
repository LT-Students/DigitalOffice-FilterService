using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Position;

namespace LT.DigitalOffice.FilterService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IPositionService
  {
    Task<List<PositionFilteredData>> GetPositionFilterDataAsync(List<Guid> positionsIds, List<string> errors);

    Task<List<PositionData>> GetPositionsDataAsync(List<Guid> usersIds, List<string> errors);
  }
}
