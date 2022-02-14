using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Models.Right;

namespace LT.DigitalOffice.FilterService.Mappers.Models
{
  public class UserInfoMapper : IUserInfoMapper
  {
    public List<UserInfo> Map(
      List<ImageInfo> imagesInfo,
      List<RolesInfo> rolesInfo,
      List<OfficeInfo> officeInfo,
      List<UserData> usersData,
      List<PositionInfo> positionInfo,
      List<PositionFilteredData> positionFilteredData,
      List<PositionData> positionData,
      List<DepartmentData> departmentData,
      List<DepartmentFilteredData> departmentFilteredData,
      List<DepartmentInfo> departmentInfo,
      List<RoleFilteredData> rolesFilteredData,
      List<OfficeFilteredData> officeFilteredData)
    {

      return usersData.Select(x => new UserInfo
      {
        Id = x.Id,
        FirstName = x.FirstName,
        LastName = x.LastName,
        MiddleName = x?.MiddleName,
        Avatar = imagesInfo?.FirstOrDefault(image => image.Id == x.ImageId),
        Position = positionInfo?.FirstOrDefault(pi => pi.Id == positionFilteredData?.FirstOrDefault(pfd => pfd.UsersIds.Contains(x.Id))?.Id) ??
          positionInfo?.FirstOrDefault(pi => pi.Id == positionData?.FirstOrDefault(pd => pd.Users.FirstOrDefault(u => u.UserId == x.Id)?.UserId == x.Id)?.Id),  
        Department = departmentInfo?.FirstOrDefault(di => di.Id == departmentFilteredData?.FirstOrDefault(dfd => dfd.UsersIds.Contains(x.Id))?.Id) ??
          departmentInfo?.FirstOrDefault(di => di.Id == departmentData?.FirstOrDefault(dd => dd.UsersIds.Contains(x.Id))?.Id),
        Office = officeInfo?.FirstOrDefault(oi => oi.Id == officeFilteredData?.FirstOrDefault(ofd => ofd.UsersIds.Contains(x.Id))?.Id),
        Role = rolesInfo?.FirstOrDefault(ri => ri.Id == rolesFilteredData?.FirstOrDefault(rfd => rfd.UsersIds.Contains(x.Id))?.Id)
       }).ToList();
    }
  }
}
