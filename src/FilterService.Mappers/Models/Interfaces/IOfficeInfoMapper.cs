using System.Collections.Generic;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Office;

namespace LT.DigitalOffice.FilterService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IOfficeInfoMapper
  {
    List<OfficeInfo> Map(List<OfficeFilteredData> officeFilteredData);
  }
}
