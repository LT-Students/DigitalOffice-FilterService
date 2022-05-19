using System.Collections.Generic;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Project;

namespace LT.DigitalOffice.FilterService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IProjectInfoMapper
  {
    List<ProjectInfo> Map(List<ProjectData> projectsData);
  }
}
