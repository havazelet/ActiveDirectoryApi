using ADAPICommon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ADAPIReposetory.interfaces;

public interface IRepository
{
    public void AddADObject(ADObject adObject, string adObjectType);
    public void ModifyADObject(ModifyModel newAdObject, string adObjectType);
}
