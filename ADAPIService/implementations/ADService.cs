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
using Microsoft.Graph;
using Microsoft.AspNetCore.Mvc;


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

    public IActionResult CreateADObject(ADObject adObject, string adObjectType)
    {
        try
        {
            if (!IsValidADObject(adObject))
            {
                var errorResponse = new { IsSuccess = false, ErrorMessage = "Invalid ADObject. Please provide valid attributes." };
                return new BadRequestObjectResult(errorResponse);
            }
            _repository.AddADObject(adObject, adObjectType);
            return new OkObjectResult("ADObject added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while working with Active Directory: {ex.Message}");
                return new StatusCodeResult(500);
        }
    }

    public bool IsValidADObject(ADObject adObject)
    {
        return adObject != null &&
         adObject.Attributes != null &&
         adObject.Identifier != null &&
         adObject.OUIdentifier != null;
    }
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }


}
