using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Department;

namespace LT.DigitalOffice.FilterService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IDepartmentService
  {
    Task<List<DepartmentFilteredData>> GetDepartmentFilterDataAsync(List<Guid> departmentsIds, List<string> errors);

    Task<List<DepartmentData>> GetDepartmentsDataAsync(List<Guid> usersIds, List<string> errors);
  }
}
