using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;

namespace LT.DigitalOffice.FilterService.Mappers.Models
{
  public class DepartmentInfoMapper : IDepartmentInfoMapper
  {
    public List<DepartmentInfo> Map(List<DepartmentData> departmentData)
    {
      if (!departmentData.Any())
      {
        return new();
      }

      return departmentData.Select(x => new DepartmentInfo
      {
        Id = x.Id,
        Name = x.Name
      }).ToList();
    }

    public List<DepartmentInfo> Map(List<DepartmentFilteredData> departmentFilteredData)
    {
      if (departmentFilteredData is null)
      {
        return null;
      }

      return departmentFilteredData.Select(x => new DepartmentInfo
      {
        Id = x.Id,
        Name = x.Name
      }).ToList();
    }
  }
}
