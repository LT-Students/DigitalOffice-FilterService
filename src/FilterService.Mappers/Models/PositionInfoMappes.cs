using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Models.Broker.Models.Position;

namespace LT.DigitalOffice.FilterService.Mappers.Models
{
  public class PositionInfoMappes : IPositionInfoMapper
  {
    public List<PositionInfo> Map(List<PositionFilteredData> positionFilteredData)
    {
      return positionFilteredData.Select(x => new PositionInfo
      {
        Id = x.Id,
        Name = x.Name
      }).ToList();
    }

    public List<PositionInfo> Map(List<PositionData> positionData)
    {
      return positionData.Select(x => new PositionInfo
      {
        Id = x.Id,
        Name = x.Name
      }).ToList();
    }
  }
}
