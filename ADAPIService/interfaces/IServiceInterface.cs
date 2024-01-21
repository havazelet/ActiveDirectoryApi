using ADAPICommon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPIService.interfaces;

public interface IServiceInterface
{
    public void CreateADObject(ADObject userModel, string adObjectType);
}
