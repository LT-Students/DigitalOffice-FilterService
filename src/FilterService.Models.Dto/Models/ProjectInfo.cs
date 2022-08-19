using System;

namespace LT.DigitalOffice.FilterService.Models.Dto.Models
{
  public class ProjectInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string ShortName { get; set; }
    public string ShortDescription { get; set; }
  }
}
