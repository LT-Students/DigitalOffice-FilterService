using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Models.Broker.Models.Project;

namespace LT.DigitalOffice.FilterService.Mappers.Models
{
  public class ProjectInfoMapper : IProjectInfoMapper
  {
    public List<ProjectInfo> Map(List<ProjectData> projectsData)
    {
      if (projectsData is null)
      {
        return null;
      }

      return projectsData.Select(x => new ProjectInfo
      {
        Id = x.Id,
        Name = x.Name,
        Status = x.Status,
        ShortName = x.ShortName,
        ShortDescription = x.ShortDescription
      }).ToList();
    }
  }
}
