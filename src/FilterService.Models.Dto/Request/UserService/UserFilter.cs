﻿using System;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.FilterService.Models.Dto.Request.UserService
{
  public record UserFilter : BaseFindFilter
  {
    [FromQuery(Name = "departmentsIds")]
    public List<Guid> DepartmentsIds { get; set; }

    [FromQuery(Name = "positionsIds")]
    public List<Guid> PositionsIds { get; set; }

    [FromQuery(Name = "rightsIds")]
    public List<Guid> RightsIds { get; set; }

    [FromQuery(Name = "officesIds")]
    public List<Guid> OfficesIds { get; set; }
  }
}
