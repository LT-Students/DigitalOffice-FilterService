using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Office;

namespace LT.DigitalOffice.FilterService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IOfficeService
  {
    Task<List<OfficeFilteredData>> GetOfficeFilterDataAsync(List<Guid> officesIds, List<string> errors);
  }
}
