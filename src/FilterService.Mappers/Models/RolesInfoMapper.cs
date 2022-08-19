using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Models.Broker.Models.Right;

namespace LT.DigitalOffice.FilterService.Mappers.Models
{
  public class RolesInfoMapper : IRolesInfoMapper
  {
    public List<RoleInfo> Map(List<RoleFilteredData> rolesFilteredData)
    {
      if (rolesFilteredData is null)
      {
        return null;
      }

      return rolesFilteredData.Select(x => new RoleInfo
      {
        Id = x.Id,
        Name = x.Name
      }).ToList();
    }
  }
}
