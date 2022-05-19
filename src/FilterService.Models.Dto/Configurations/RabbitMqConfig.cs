using LT.DigitalOffice.Kernel.BrokerSupport.Attributes;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Requests.User;

namespace LT.DigitalOffice.FilterService.Models.Dto.Configurations
{
  public class RabbitMqConfig : BaseRabbitMqConfig
  {
    [AutoInjectRequest(typeof(IFilterOfficesRequest))]
    public string FilterOfficesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IFilterDepartmentsRequest))]
    public string FilterDepartmentsEndpoint { get; set; }

    [AutoInjectRequest(typeof(IFilterPositionsRequest))]
    public string FilterPositionsEndpoint { get; set; }

    [AutoInjectRequest(typeof(IFilterRolesRequest))]
    public string FilterRolesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IFilteredUsersDataRequest))]
    public string FilterUsersDataEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetImagesRequest))]
    public string GetImagesEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetDepartmentsRequest))]
    public string GetDepartmentsEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetPositionsRequest))]
    public string GetPositionsEndpoint { get; set; }

    [AutoInjectRequest(typeof(IGetProjectsRequest))]
    public string GetProjectsEndpoint { get; set; }
  }
}
