using System.Collections.Generic;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models;

namespace LT.DigitalOffice.FilterService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IImageInfoMapper
  {
    List<ImageInfo> Map(List<ImageData> imagesData);
  }
}
