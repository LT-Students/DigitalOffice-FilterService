using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.FilterService.Models.Dto.Request.UserService
{
  public record UserFilter
  {
    [FromQuery(Name = "departmentsIds")]
    public List<Guid> DepartmentsIds { get; set; }

    [FromQuery(Name = "positionsIds")]
    public List<Guid> PositionsIds { get; set; }

    [FromQuery(Name = "rolesIds")]
    public List<Guid> RolesIds { get; set; }

    [FromQuery(Name = "officesIds")]
    public List<Guid> OfficesIds { get; set; }

    [FromQuery(Name = "isAscendingSort")]
    public bool? IsAscendingSort { get; set; }
  }
}
