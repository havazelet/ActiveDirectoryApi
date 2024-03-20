using ADAPICommon.model;
using ADAPIReposetory.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.DirectoryServices;
using System.Reflection;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace ADAPIReposetory.implementions;

public class Repository : IRepository
{
    private readonly ILogger<Repository> _logger;
    private readonly IConfiguration _configuration;

    public Repository(ILogger<Repository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }


    public void AddADObject(ADObject adObject, string adObjectType)
    {
        string objectType = _configuration["ActiveDirectory:ObjectType"];
        string commonName = (adObjectType == objectType) ? _configuration["ActiveDirectory:OU"] : _configuration["ActiveDirectory:CN"];

        using (DirectorySearcher searcher = new DirectorySearcher())
        {
            searcher.Filter = $"({adObject.OUIdentifier?.Attribute}={adObject.OUIdentifier?.Value})";
            SearchResult result = searcher.FindOne();

            if (result is not null)
            {
                DirectoryEntry objectEntry = result.GetDirectoryEntry();
                using (DirectoryEntry newObject = objectEntry.Children.Add($"{commonName}={adObject.Attributes[commonName]}", adObjectType))
                {
                    foreach (var attribute in adObject.Attributes)
                    {
                        if (attribute.Value is not null)
                            newObject.Properties[attribute.Key].Value = attribute.Value;
                    }
                    newObject.CommitChanges();
                }
            }
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
                else {
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

    public static void Rename(DirectoryEntry objectEntry, string newName)
    {
        try
        {
            objectEntry.Rename("CN=" + newName);
            Console.WriteLine("Entry renamed successfully.");
        }
        catch (DirectoryServicesCOMException ex)
        {
           // Console.WriteLine($"Error renaming entry: {ex.Message}");
           // _logger.LogError(ex, $"Error renaming entry: {ex.Message}");
        }
    }

    public static void ResetPassword(DirectoryEntry objectEntry, string newName)
    {
        objectEntry.Invoke("setPassword", new object[] { newName });
    }

    public static void MoveTo(DirectoryEntry objectEntry, Identifier destinationOu)
    {
        using (DirectorySearcher searcherDestination = new DirectorySearcher())
        {
            if (destinationOu is not null)
            {
                Identifier destOu = destinationOu;
                searcherDestination.Filter = $"({destOu.Attribute}={destOu.Value})";
            }
            SearchResult resultDestination = searcherDestination.FindOne();
            if (resultDestination != null)
            {
                DirectoryEntry objectEntryDestination = resultDestination.GetDirectoryEntry();
                objectEntry.MoveTo(objectEntryDestination);
            }
            else
            {
                Console.WriteLine("Destination OU not found.");
            }
        }
    }

    public void ModifyADObject(ModifyModel newAdObject, string adObjectType)
    {
        using DirectorySearcher searcher = new DirectorySearcher();
        searcher.Filter = $"({newAdObject.Identifier?.Attribute}={newAdObject.Identifier?.Value})";
        SearchResult result = searcher.FindOne();

        if (result is not null)
        {
            DirectoryEntry objectEntry = result.GetDirectoryEntry();

            foreach (var attribute in newAdObject.WriteAttribute)
            {
                if (objectEntry.Properties.Contains(attribute.Key))
                {
                    objectEntry.Properties[attribute.Key].Value = attribute.Value;
                }
                else
                {
                    objectEntry.Properties[attribute.Key].Add(attribute.Value);
                }
            }
            foreach (var action in newAdObject.Actions)
            {
                if (action.Value is not null)
                {
                    HandleAction(action.Key, action.Value, objectEntry);
                }
            }
            objectEntry.CommitChanges();
        }
    }

}

