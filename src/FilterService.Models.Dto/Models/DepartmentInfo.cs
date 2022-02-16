using System;

namespace LT.DigitalOffice.FilterService.Models.Dto.Models
{
  public record DepartmentInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
  }
}
