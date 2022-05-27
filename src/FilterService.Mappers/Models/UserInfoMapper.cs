using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Models.Right;

namespace LT.DigitalOffice.FilterService.Mappers.Models
{
  public class UserInfoMapper : IUserInfoMapper
  {
    public List<UserInfo> Map(
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
      List<ProjectData> projectsData)
    {
      if (usersData is null)
      {
        return null;
      }

      return usersData.Select(x => new UserInfo
      {
        Id = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        MiddleName = x?.MiddleName,
        Status = x.Status,
        Position = positionInfo?.FirstOrDefault(pi => pi.Id == positionFilteredData?.FirstOrDefault(pfd => pfd.UsersIds.Contains(x.Id))?.Id) ??
          positionInfo?.FirstOrDefault(pi => pi.Id == positionData?.FirstOrDefault(pd => pd.Users.FirstOrDefault(u => u.UserId == x.Id)?.UserId == x.Id)?.Id),
        Department = departmentInfo?.FirstOrDefault(di => di.Id == departmentFilteredData?.FirstOrDefault(dfd => dfd.UsersIds.Contains(x.Id))?.Id) ??
          departmentInfo?.FirstOrDefault(di => di.Id == departmentData?.FirstOrDefault(dd => dd.UsersIds.Contains(x.Id))?.Id),
        Office = officeInfo?.FirstOrDefault(oi => oi.Id == officeFilteredData?.FirstOrDefault(ofd => ofd.UsersIds.Contains(x.Id))?.Id),
        Role = rolesInfo?.FirstOrDefault(ri => ri.Id == rolesFilteredData?.FirstOrDefault(rfd => rfd.UsersIds.Contains(x.Id))?.Id),
        Projects = projectsInfo?.Where(project => (bool)(projectsData?.Where(projectData => projectData.Users.FirstOrDefault(user => user.UserId == x.Id) is not null)
          .Select(result => result.Id).Contains(project.Id))).ToList()
      }).ToList();
    }

    public List<UserInfo> Map(
      List<UserInfo> usersInfos,
      List<UserData> usersData,
      List<ImageInfo> imagesInfos)
    {
      if (!usersInfos.Any() || usersInfos is null)
      {
        return new();
      }

      usersInfos.ForEach(
        ui => ui.Avatar = imagesInfos?.FirstOrDefault(
          i => i.Id == usersData.FirstOrDefault(ud => ud.Id == ui.Id).ImageId));

      return usersInfos;
    }
  }
}
