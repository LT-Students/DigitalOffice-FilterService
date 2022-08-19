using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.Models.Broker.Models.Image;

namespace LT.DigitalOffice.FilterService.Mappers.Models
{
  public class ImageInfoMapper : IImageInfoMapper
  {
    public List<ImageInfo> Map(List<ImageData> imagesData)
    {
      if (imagesData is null)
      { 
        return null;
      }

      return imagesData.Select(x => new ImageInfo
      {
        Id = x.ImageId,
        ParentId = x.ParentId,
        Name = x.Name,
        Content = x.Content,
        Extension = x.Extension
      }).ToList();
    }
  }
}
