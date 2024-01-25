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
using Microsoft.Extensions.Logging;

namespace ADAPIService.implementations;

public class ADService : IServiceInterface
{
    private readonly ILogger<ADService> _logger;
    protected IRepository _repository;

    public ADService(ILogger<ADService> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public void CreateADObject(ADObject adObject, string adObjectType)
    {
        _repository.AddADObject(adObject, adObjectType);
    }

    public bool IsValidADObject(ADObject adObject)
    {
        if (adObject == null || adObject.Attributes == null || adObject.Identifier == null || adObject.OUIdentifier == null)
        {
            return false;
        }
        return true;
    }

   
}
