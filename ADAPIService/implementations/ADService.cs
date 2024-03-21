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
using System.Reflection;
using System.Text.Json;
using System.DirectoryServices;


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
            return new OkObjectResult($"{adObject} added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while adding {adObject}, error message: {ex.Message}");
            return new StatusCodeResult(500);
        }
    }

    public bool IsValidADObject(ADObject adObject)
    {
        return adObject is not null &&
         adObject.Attributes is not null &&
         adObject.Identifier is not null &&
         adObject.OUIdentifier is not null;
    }
    public class OperationResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }


    public IActionResult ModifyADObject(ModifyModel newAdObject, string adObjectType)
    {
        using DirectorySearcher searcher = new DirectorySearcher();
        searcher.Filter = $"({newAdObject.Identifier?.Attribute}={newAdObject.Identifier?.Value})";
        SearchResult result = searcher.FindOne();

        try
        {
            DirectoryEntry objectEntry = result.GetDirectoryEntry();
            _repository.ModifyADObject(newAdObject, adObjectType, objectEntry);

            foreach (var action in newAdObject.Actions)
            {
                if (action.Value is not null)
                {
                    HandleAction(action.Key, action.Value, objectEntry);
                }
            }
            return new OkObjectResult($"{newAdObject} modify successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while modify {newAdObject}, error message:", ex.Message);
            return new StatusCodeResult(500);
        }
        
    }

    public void HandleAction(string actionKey, Dictionary<string, object> actionValue, DirectoryEntry objectEntry)
    {
        MethodInfo method = typeof(Repository).GetMethod(actionKey);
        if (method != null)
        {
            ParameterInfo[] parameters = method.GetParameters();
            object[] args = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                Type parameterType = parameter.ParameterType;

                if (parameterType == typeof(DirectoryEntry))
                {
                    args[i] = objectEntry;
                }
                else
                {
                    try
                    {
                        var json = JsonSerializer.Serialize(actionValue.FirstOrDefault().Value);

                        MethodInfo methodInfo = typeof(JsonSerializer).GetMethod("Deserialize", new[] { typeof(JsonDocument), typeof(JsonSerializerOptions) });
                        MethodInfo generic = methodInfo.MakeGenericMethod(parameterType);
                        args[i] = generic.Invoke(null, new object[] { JsonDocument.Parse(json), new JsonSerializerOptions() });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to deserialize parameter '{parameter.Name}' to type '{parameterType}'. {ex.Message}");
                        return;
                    }
                }
            }

            try
            {
                method.Invoke(null, args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error invoking method '{actionKey}': {ex.Message}");
            }
        }
    }


}
