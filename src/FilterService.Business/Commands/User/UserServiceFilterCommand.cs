using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.FilterService.Models.Dto.Request.UserService;
using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.Models.Broker.Requests.Department;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Requests.Office;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Requests.Rights;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Department;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.Models.Broker.Responses.Office;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.Models.Broker.Responses.Rights;
using LT.DigitalOffice.Models.Broker.Responses.User;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.FilterService.Business.Commands.User
{
  public class UserServiceFilterCommand : IUserServiceFilterCommand
  {
    private readonly IRequestClient<IFilterOfficesRequest> _rcGetOffices;
    private readonly IRequestClient<IFilterDepartmentsRequest> _rcGetDepartments;
    private readonly IRequestClient<IFilterPositionsRequest> _rcGetPositions;
    private readonly IRequestClient<IFilterRolesRequest> _rcGetRoles;
    private readonly IRequestClient<IGetUsersDataRequest> _rcGetUsers;
    private readonly IRequestClient<IGetDepartmentsRequest> _rcGetDepartmentsData;
    private readonly IRequestClient<IGetPositionsRequest> _rcGetPositionsData;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IUserInfoMapper _userInfoMapper;
    private readonly IImageInfoMapper _imageInfoMapper;
    private readonly IPositionInfoMapper _positionInfoMapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IOfficeInfoMapper _officeInfoMapper;
    private readonly IRolesInfoMapper _rolesInfoMapper;
    private readonly ILogger<UserServiceFilterCommand> _logger;

    #region private methods

    private async Task<List<OfficeFilteredData>> GetOfficeFilteredDataAsync(List<Guid> officesIds, List<string> errors)
    {
      if (officesIds == null || !officesIds.Any())
      {
        return null;
      }

      return (await RequestHandler.ProcessRequest<IFilterOfficesRequest, IFilterOfficesResponse>(
       _rcGetOffices,
       IFilterOfficesRequest.CreateObj(officesIds),
       errors, _logger)).Offices.ToList();
    }

    private async Task<List<DepartmentFilteredData>> GetDepartmentFilteredDataAsync(List<Guid> departmentsIds, List<string> errors)
    {
      if (departmentsIds == null || !departmentsIds.Any())
      {
        return null;
      }

      return (await RequestHandler.ProcessRequest<IFilterDepartmentsRequest, IFilterDepartmentsResponse>(
        _rcGetDepartments,
        IFilterDepartmentsRequest.CreateObj(departmentsIds),
        errors, _logger)).Departments.ToList();
    }

    private async Task<List<PositionFilteredData>> GetPositionFilteredDataAsync(List<Guid> positionsIds, List<string> errors)
    {
      if (positionsIds == null || !positionsIds.Any())
      {
        return null;
      }

      return (await RequestHandler.ProcessRequest<IFilterPositionsRequest, IFilterPositionsResponse>(
        _rcGetPositions,
        IFilterPositionsRequest.CreateObj(positionsIds),
        errors, _logger)).Positions.ToList();
    }

    private async Task<List<RoleFilteredData>> GetRolesFilteredDataAsync(List<Guid> roleIds, List<string> errors)
    {
      if (roleIds == null || !roleIds.Any())
      {
        return null;
      }

      return (await RequestHandler.ProcessRequest<IFilterRolesRequest, IFilterRolesResponse>(
        _rcGetRoles,
        IFilterRolesRequest.CreateObj(roleIds),
        errors, _logger)).Roles.ToList();
    }

    private async Task<List<UserData>> GetUsersDataAsync(List<Guid> usersIds, FilterUserService filter, List<string> errors)
    {
      if (filter.DepartmentsIds is null && 
          filter.PositionsIds is null &&
          filter.RightsIds is null && 
          filter.OfficesIds is null)
      {
        return (await RequestHandler.ProcessRequest<IGetUsersDataRequest, IGetUsersDataResponse>(
          _rcGetUsers,
          IGetUsersDataRequest.CreateObj(new List<Guid>(), filter.SkipCount, filter.TakeCount),
          errors, _logger)).UsersData.ToList();
      }

      return (await RequestHandler.ProcessRequest<IGetUsersDataRequest, IGetUsersDataResponse>(
          _rcGetUsers,
          IGetUsersDataRequest.CreateObj(usersIds),
          errors, _logger)).UsersData.ToList();
    }

    private async Task<List<PositionData>> GetPositionsDataAsync(List<Guid> usersIds, List<string> errors)
    {
      return (await RequestHandler.ProcessRequest<IGetPositionsRequest, IGetPositionsResponse>(
          _rcGetPositionsData,
          IGetPositionsRequest.CreateObj(usersIds),
          errors, _logger)).Positions.ToList();
    }

    private async Task<List<DepartmentData>> GetDepartmentsDataAsync(
      List<Guid> usersIds,
      List<string> errors)
    {
      return (await RequestHandler.ProcessRequest<IGetDepartmentsRequest, IGetDepartmentsResponse>(
          _rcGetDepartmentsData,
          IGetPositionsRequest.CreateObj(usersIds),
          errors, _logger)).Departments.ToList();
    }

    private async Task<List<ImageData>> GetImagesDataAsync(List<Guid> usersImageIds, List<string> errors)
    {
      if (usersImageIds == null || !usersImageIds.Any())
      {
        return null;
      }

      return (await RequestHandler.ProcessRequest<IGetImagesRequest, IGetImagesResponse>(
        _rcGetImages,
        IGetImagesRequest.CreateObj(usersImageIds, ImageSource.User),
        errors, _logger)).ImagesData.ToList();

    }

    private List<Guid> FilteredUserIds(List<Guid> officesUserIds, 
      List<Guid> departmentsUserIds, 
      List<Guid> positionsUserIds,
      List<Guid> rolesUserIds)
    {
      List<Guid> filteredUserIds = new();

      if (departmentsUserIds.Any())
      {
        filteredUserIds.AddRange(departmentsUserIds);

        if (officesUserIds.Any())
        {
          if (!officesUserIds.Any(o => filteredUserIds.Contains(o)))
          {
            return null;
          }

          filteredUserIds.AddRange(officesUserIds);

          filteredUserIds = filteredUserIds
            .GroupBy(g => g)
            .Where(id => id.Count() > 1)
            .Select(g => g.Key).ToList();
        }
        if (positionsUserIds.Any())
        {
          if (!positionsUserIds.Any(p => filteredUserIds.Contains(p)))
          {
            return null;
          }

          filteredUserIds.AddRange(positionsUserIds);

          filteredUserIds = filteredUserIds
            .GroupBy(g => g)
            .Where(id => id.Count() > 1)
            .Select(g => g.Key).ToList();
        }
        if (rolesUserIds.Any())
        {
          if (!rolesUserIds.Any(r => filteredUserIds.Contains(r)))
          {
            return null;
          }

          filteredUserIds.AddRange(rolesUserIds);

          filteredUserIds = filteredUserIds
            .GroupBy(g => g)
            .Where(id => id.Count() > 1)
            .Select(g => g.Key).ToList();
        }

        return filteredUserIds;
      }

      if (officesUserIds.Any())
      {
        filteredUserIds.AddRange(officesUserIds);

        if (positionsUserIds.Any())
        {
          if (!positionsUserIds.Any(p => filteredUserIds.Contains(p)))
          {
            return null;
          }

          filteredUserIds.AddRange(positionsUserIds);

          filteredUserIds = filteredUserIds
            .GroupBy(g => g)
            .Where(id => id.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        }
        if (rolesUserIds.Any())
        {
          if (!rolesUserIds.Any(r => filteredUserIds.Contains(r)))
          {
            return null;
          }

          filteredUserIds.AddRange(rolesUserIds);

          filteredUserIds = filteredUserIds
            .GroupBy(g => g)
            .Where(id => id.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        }

        return filteredUserIds;
      }

      if (positionsUserIds.Any())
      {
        filteredUserIds.AddRange(positionsUserIds);

        if (rolesUserIds.Any())
        {
          if (!rolesUserIds.Any(r => filteredUserIds.Contains(r)))
          {
            return null;
          }

          filteredUserIds.AddRange(rolesUserIds);

          filteredUserIds = filteredUserIds
            .GroupBy(g => g)
            .Where(id => id.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        }

        return filteredUserIds;
      }

      filteredUserIds.AddRange(rolesUserIds);

      return filteredUserIds;
    }

    #endregion

    public UserServiceFilterCommand(
      IRequestClient<IFilterOfficesRequest> rcGetOffices,
      IRequestClient<IFilterDepartmentsRequest> rcGetDepartments,
      IRequestClient<IFilterPositionsRequest> rcGetPositions,
      IRequestClient<IFilterRolesRequest> rcGetRoles,
      IRequestClient<IGetUsersDataRequest> rcGetUsers,
      IRequestClient<IGetPositionsRequest> rcGetPositionsData,
      IRequestClient<IGetDepartmentsRequest> rcGetDepartmentsData,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IUserInfoMapper userInfoMapper,
      IImageInfoMapper imageInfoMapper,
      IPositionInfoMapper positionInfoMapper,
      IDepartmentInfoMapper departmentInfoMapper,
      IOfficeInfoMapper officeInfoMapper,
      IRolesInfoMapper rolesInfoMapper,
      ILogger<UserServiceFilterCommand> logger)
    {
      _rcGetOffices = rcGetOffices;
      _rcGetDepartments = rcGetDepartments;
      _rcGetPositions = rcGetPositions;
      _rcGetRoles = rcGetRoles;
      _rcGetUsers = rcGetUsers;
      _rcGetPositionsData = rcGetPositionsData;
      _rcGetDepartmentsData = rcGetDepartmentsData;
      _rolesInfoMapper = rolesInfoMapper;
      _rcGetImages = rcGetImages;
      _userInfoMapper = userInfoMapper;
      _imageInfoMapper = imageInfoMapper;
      _positionInfoMapper = positionInfoMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _officeInfoMapper = officeInfoMapper;
      _logger = logger;
    }

    public async Task<FindResultResponse<UserInfo>> ExecuteAsync(FilterUserService filter)
    {
      FindResultResponse<UserInfo> response = new();

      List<UserInfo> userInfo = new();
      List<string> errors = new();

      Task<List<DepartmentFilteredData>> departmentsUsersTask = GetDepartmentFilteredDataAsync(filter.DepartmentsIds, response.Errors);
      Task<List<OfficeFilteredData>> officesUsersTask = GetOfficeFilteredDataAsync(filter.OfficesIds, response.Errors);
      Task<List<PositionFilteredData>> positionsUsersTask = GetPositionFilteredDataAsync(filter.PositionsIds, response.Errors);
      Task<List<RoleFilteredData>> rolesUsersTask = GetRolesFilteredDataAsync(filter.RightsIds, response.Errors);

      await Task.WhenAll(departmentsUsersTask, officesUsersTask, positionsUsersTask, rolesUsersTask);

      List<OfficeFilteredData> officeFilteredData = await officesUsersTask;
      List<DepartmentFilteredData> departmentsFilteredUsers = await departmentsUsersTask;
      List<PositionFilteredData> positionsFilteredData = await positionsUsersTask;
      List<RoleFilteredData> rolesFilteredData = await rolesUsersTask;

      List<Guid> filteredUsers = FilteredUserIds(
        officeFilteredData?.SelectMany(x => x.UsersIds).ToList() ?? new List<Guid>(),
        departmentsFilteredUsers?.SelectMany(x => x.UsersIds).ToList() ?? new List<Guid>(),
        positionsFilteredData?.SelectMany(x => x.UsersIds).ToList() ?? new List<Guid>(),
        rolesFilteredData?.SelectMany(x => x.UsersIds).ToList() ?? new List<Guid>());

      //ToDo add emplementation SkipCount TakeCount
      if ((filter.RightsIds is null &&
           filter.OfficesIds is null &&
           filter.DepartmentsIds is null &&
           filter.PositionsIds is null) ||
           filteredUsers is not null)
      {
        List<UserData> usersData = await GetUsersDataAsync(filteredUsers, filter, errors);

        List<ImageData> usersImages = await GetImagesDataAsync(
          usersData.Where(u => u.ImageId.HasValue).
          Select(u => u.ImageId.Value).ToList(),
          errors);

        List<PositionInfo> positionInfo = new();
        List<DepartmentInfo> departmentInfo = new();
        List<PositionData> positionData = new();
        List<DepartmentData> departmentData = new();

        if (filter.PositionsIds is null)
        {
          positionData.AddRange(await GetPositionsDataAsync(
              usersData.Select(u => u.Id).ToList(), errors));

          positionInfo.AddRange(_positionInfoMapper.Map(positionData));
        }
        else
        {
          positionInfo.AddRange(_positionInfoMapper.Map(positionsFilteredData));
        }

        if (filter.DepartmentsIds is null)
        {
          departmentData.AddRange(await GetDepartmentsDataAsync(
                usersData.Select(u => u.Id).ToList(),
                errors));

          departmentInfo.AddRange(_departmentInfoMapper.Map(departmentData));
        }
        else
        {
          departmentInfo.AddRange(_departmentInfoMapper.Map(departmentsFilteredUsers));
        }

        userInfo = _userInfoMapper.Map(
          _imageInfoMapper.Map(usersImages),
          _rolesInfoMapper.Map(rolesFilteredData),
          _officeInfoMapper.Map(officeFilteredData),
          usersData,
          positionInfo,
          positionsFilteredData,
          positionData,
          departmentData,
          departmentsFilteredUsers,
          departmentInfo,
          rolesFilteredData,
          officeFilteredData);
      }

      response.Body = userInfo;

      response.TotalCount = response.Body.Count();
      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
