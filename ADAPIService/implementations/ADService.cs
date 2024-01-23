using ADAPICommon.model;
using ADAPIReposetory;
using ADAPIReposetory.implementions;
using ADAPIReposetory.interfaces;
using ADAPIService.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPIService.implementations;

public class ADService : IServiceInterface
{
    protected IRepository _repository;

    public ADService(IServiceInterface repository)
    {

    }

    public bool IsValidADObject(ADObject adObject)
    {
        if (adObject == null || adObject.Attributes == null || adObject.Identifier == null || adObject.OUIdentifier == null)
        {
            return false;
        }
        return true;
    }

    public void CreateADObject(ADObject adObject, string adObjectType)
    {
        _repository.AddADObject(adObject, adObjectType);      
    }
}
