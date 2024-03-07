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
    public void AddADObject(ADObject userModel, string adObjectType);
}
