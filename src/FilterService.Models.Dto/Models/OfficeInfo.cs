using System;

namespace LT.DigitalOffice.FilterService.Models.Dto.Models
{
  public record OfficeInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
  }
}
