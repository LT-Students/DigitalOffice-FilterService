using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Models.Broker.Models.Office;

namespace LT.DigitalOffice.FilterService.Mappers.Models
{
  public class OfficeInfoMapper : IOfficeInfoMapper
  {
    public List<OfficeInfo> Map(List<OfficeFilteredData> officeFilteredData)
    {
      if (officeFilteredData is null)
      {
        return null;
      }

      return officeFilteredData.Select(x => new OfficeInfo
      {
        Id = x.Id,
        Name = x.Name
      }).ToList();
    }
  }
}
