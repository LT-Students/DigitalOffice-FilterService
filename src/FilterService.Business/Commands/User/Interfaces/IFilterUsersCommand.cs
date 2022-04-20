using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.FilterService.Models.Dto.Request.UserService;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;

namespace LT.DigitalOffice.FilterService.Business.Commands.User.Interfaces
{
  [AutoInject]
  public interface IFilterUsersCommand
  {
    Task<FindResultResponse<UserInfo>> ExecuteAsync(UserFilter filter, PaginationValues value);
  }
}
