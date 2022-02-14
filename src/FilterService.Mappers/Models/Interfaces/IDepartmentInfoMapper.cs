using System.Collections.Generic;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Department;

namespace LT.DigitalOffice.FilterService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IDepartmentInfoMapper
  {
    List<DepartmentInfo> Map(List<DepartmentData> departmentData);
    List<DepartmentInfo> Map(List<DepartmentFilteredData> departmentFilteredData);
  }
}
