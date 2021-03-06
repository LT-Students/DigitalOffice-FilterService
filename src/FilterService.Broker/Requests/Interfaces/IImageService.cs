using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Image;

namespace LT.DigitalOffice.FilterService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IImageService
  {
    Task<List<ImageData>> GetImagesDataAsync(List<Guid> usersImagesIds, List<string> errors);
  }
}
