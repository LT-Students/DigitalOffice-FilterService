using System.Collections.Generic;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Models.Right;

namespace LT.DigitalOffice.FilterService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IUserInfoMapper
  {
    List<UserInfo> Map(
      List<RoleInfo> rolesInfo,
      List<OfficeInfo> officeInfo,
      List<UserData> usersData,
      List<PositionInfo> positionInfo,
      List<PositionFilteredData> positionFilteredData,
      List<PositionData> positionData,
      List<DepartmentData> departmentData,
      List<DepartmentFilteredData> departmentFilteredData,
      List<DepartmentInfo> departmentInfo,
      List<RoleFilteredData> rolesFilteredData,
      List<OfficeFilteredData> officeFilteredData,
      List<ProjectInfo> projectsInfo,
      List<ProjectData> projectsData);
  }
}
