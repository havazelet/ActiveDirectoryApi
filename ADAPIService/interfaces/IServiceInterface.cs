﻿using ADAPICommon.model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPIService.interfaces;

public interface IServiceInterface
{
    public IActionResult CreateADObject(ADObject userModel, string adObjectType);
}
