using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.FilterService.Broker.Requests.Interfaces;
using LT.DigitalOffice.FilterService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.FilterService.Mappers.Models.Interfaces;
using LT.DigitalOffice.FilterService.Models.Dto.Models;
using LT.DigitalOffice.FilterService.Models.Dto.Request.UserService;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Models.Right;

namespace LT.DigitalOffice.FilterService.Business.Commands.User
{
  public class FilterUsersCommand : IFilterUsersCommand
  {
    private readonly IUserInfoMapper _userInfoMapper;
    private readonly IImageInfoMapper _imageInfoMapper;
    private readonly IPositionInfoMapper _positionInfoMapper;
    private readonly IDepartmentInfoMapper _departmentInfoMapper;
    private readonly IOfficeInfoMapper _officeInfoMapper;
    private readonly IRolesInfoMapper _rolesInfoMapper;
    private readonly IProjectInfoMapper _projectsInfoMapper;
    private readonly IOfficeService _officeService;
    private readonly IDepartmentService _departmentService;
    private readonly IImageService _imageService;
    private readonly IPositionService _positionService;
    private readonly IRoleService _roleService;
    private readonly IUserService _userService;
    private readonly IProjectService _projectService;

    #region private methods
    private List<Guid> FilteredUserIds(params List<Guid>[] userIds)
    {
      List<Guid> filteredUserIds = new();

      for (int i = 0; i < userIds.Length; i++)
      {
        if (userIds[i] is null)
        {
          continue;
        }
        if (!filteredUserIds.Any())
        {
          filteredUserIds.AddRange(userIds[i]);
        }
        if (!userIds[i].Any(id => filteredUserIds.Contains(id)))
        {
          return null;
        }

        filteredUserIds.AddRange(userIds[i]);

        filteredUserIds = filteredUserIds
          .GroupBy(g => g)
          .Where(id => id.Count() > 1)
          .Select(g => g.Key).ToList();
      }

      return filteredUserIds;
    }
    #endregion

    public FilterUsersCommand(
    IUserInfoMapper userInfoMapper,
    IImageInfoMapper imageInfoMapper,
    IPositionInfoMapper positionInfoMapper,
    IDepartmentInfoMapper departmentInfoMapper,
    IOfficeInfoMapper officeInfoMapper,
    IRolesInfoMapper rolesInfoMapper,
    IProjectInfoMapper projectsInfoMapper,
    IOfficeService officeService,
    IDepartmentService departmentService,
    IImageService imageService,
    IPositionService positionService,
    IRoleService roleService,
    IUserService userService,
    IProjectService projectService)
    {
      _rolesInfoMapper = rolesInfoMapper;
      _userInfoMapper = userInfoMapper;
      _imageInfoMapper = imageInfoMapper;
      _positionInfoMapper = positionInfoMapper;
      _departmentInfoMapper = departmentInfoMapper;
      _officeInfoMapper = officeInfoMapper;
      _projectsInfoMapper = projectsInfoMapper;
      _officeService = officeService;
      _departmentService = departmentService;
      _imageService = imageService;
      _positionService = positionService;
      _roleService = roleService;
      _userService = userService;
      _projectService = projectService;
    }

    public async Task<FindResultResponse<UserInfo>> ExecuteAsync(
      UserFilter filter,
      PaginationValues value)
    {
      FindResultResponse<UserInfo> response = new();

      List<UserInfo> userInfo = new();

      Task<List<DepartmentFilteredData>> departmentsUsersTask = _departmentService.GetDepartmentFilteredDataAsync(filter.DepartmentsIds, response.Errors);
      Task<List<OfficeFilteredData>> officesUsersTask = _officeService.GetOfficeFilteredDataAsync(filter.OfficesIds, response.Errors);
      Task<List<PositionFilteredData>> positionsUsersTask = _positionService.GetPositionFilteredDataAsync(filter.PositionsIds, response.Errors);
      Task<List<RoleFilteredData>> rolesUsersTask = _roleService.GetRolesFilteredDataAsync(filter.RolesIds, response.Errors);
      Task<List<ProjectData>> projectsUsersTask = _projectService.GetProjectsDataAsync(filter.ProjectsIds, response.Errors);

      await Task.WhenAll(departmentsUsersTask, officesUsersTask, positionsUsersTask, rolesUsersTask, projectsUsersTask);

      List<OfficeFilteredData> officeFilteredData = await officesUsersTask;
      List<DepartmentFilteredData> departmentsFilteredUsers = await departmentsUsersTask;
      List<PositionFilteredData> positionsFilteredData = await positionsUsersTask;
      List<RoleFilteredData> rolesFilteredData = await rolesUsersTask;
      List<ProjectData> projectsData = await projectsUsersTask;

      List<Guid> filteredUsers = FilteredUserIds(
        officeFilteredData?.SelectMany(x => x.UsersIds).ToList(),
        departmentsFilteredUsers?.SelectMany(x => x.UsersIds).ToList(),
        positionsFilteredData?.SelectMany(x => x.UsersIds).ToList(),
        rolesFilteredData?.SelectMany(x => x.UsersIds).ToList(),
        projectsData?.SelectMany(x => x.Users).Select(x => x.UserId).ToList());

      if (filteredUsers is not null)
      {
        (List<UserData> usersData, int? totalCount) = await _userService.GetFilteredUsersDataAsync(filteredUsers, filter, value, response.Errors);

        List<PositionInfo> positionInfo = new();
        List<DepartmentInfo> departmentInfo = new();
        List<PositionData> positionData = new();
        List<DepartmentData> departmentData = new();

        if (filter.PositionsIds is null)
        {
          positionData.AddRange(await _positionService.GetPositionsDataAsync(
              usersData.Select(u => u.Id).ToList(),
              response.Errors) ?? new());

          positionInfo.AddRange(_positionInfoMapper.Map(positionData));
        }
        else
        {
          positionInfo.AddRange(_positionInfoMapper.Map(positionsFilteredData) ?? new());
        }

        if (filter.DepartmentsIds is null)
        {
          departmentData.AddRange(await _departmentService.GetDepartmentsDataAsync(
              usersData.Select(u => u.Id).ToList(),
              response.Errors) ?? new());

          departmentInfo.AddRange(_departmentInfoMapper.Map(departmentData));
        }
        else
        {
          departmentInfo.AddRange(_departmentInfoMapper.Map(departmentsFilteredUsers) ?? new());
        }

        userInfo = _userInfoMapper.Map(
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
          officeFilteredData,
          _projectsInfoMapper.Map(projectsData),
          projectsData);

        List<ImageData> usersImages = await _imageService.GetImagesDataAsync(
           usersData.Where(u => u.ImageId.HasValue).
           Select(u => u.ImageId.Value).ToList(),
           response.Errors);

        userInfo = _userInfoMapper.Map(userInfo, usersData, _imageInfoMapper.Map(usersImages));
        response.TotalCount = totalCount.Value;
      }

      response.Body = userInfo;

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
