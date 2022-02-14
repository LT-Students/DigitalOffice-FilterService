using System;

namespace LT.DigitalOffice.FilterService.Models.Dto.Models
{
  public record UserInfo
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }

    public ImageInfo Avatar { get; set; }
    public PositionInfo Position { get; set; }
    public RolesInfo Role { get; set; }
    public OfficeInfo Office { get; set; }
    public DepartmentInfo Department { get; set; }
  }
}
