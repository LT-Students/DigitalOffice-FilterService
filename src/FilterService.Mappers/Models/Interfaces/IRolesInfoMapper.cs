using System.Collections.Generic;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Right;

namespace LT.DigitalOffice.FilterService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IRolesInfoMapper
  {
    List<RoleInfo> Map(List<RoleFilteredData> rolesFilteredData);
  }
}
