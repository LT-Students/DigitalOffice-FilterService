using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Broker.Requests.Interfaces;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.FilterService.Broker.Requests
{
  public class ImageService : IImageService
  {
    private readonly ILogger<ImageService> _logger;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;

    public ImageService(
      ILogger<ImageService> logger,
      IRequestClient<IGetImagesRequest> rcGetImages)
    {
      _logger = logger;
      _rcGetImages = rcGetImages;
    }

    public async Task<List<ImageData>> GetImagesDataAsync(List<Guid> usersImageIds, List<string> errors)
    {
      if (usersImageIds is null || !usersImageIds.Any())
      {
        return null;
      }

      List<ImageData> imagesData =
        (await RequestHandler.ProcessRequest<IGetImagesRequest, IGetImagesResponse>(
          _rcGetImages,
          IGetImagesRequest.CreateObj(usersImageIds, ImageSource.User),
          errors,
          _logger))
        ?.ImagesData;

      if (imagesData is null)
      {
        errors.Add("Cannot get Images");
      }

      return imagesData;
    }
  }
}
