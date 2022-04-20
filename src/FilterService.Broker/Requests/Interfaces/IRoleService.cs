using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Right;

namespace LT.DigitalOffice.FilterService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IRoleService
  {
    Task<List<RoleFilteredData>> GetRolesFilterDataAsync(List<Guid> roleIds, List<string> errors);
  }
}
