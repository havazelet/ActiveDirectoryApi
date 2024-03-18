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

    //static MethodInfo GetMethod(string actionKey)
    //{
    //    // Implement logic to map action key to method
    //    switch (actionKey)
    //    {
    //        case "SomeAction":
    //            return typeof(Repository).GetMethod("SomeActionMethod", BindingFlags.Static | BindingFlags.NonPublic);
    //        // Add more cases for other actions
    //        default:
    //            return null;
    //    }
    //}

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
                if (parameter.ParameterType == typeof(DirectoryEntry))
                {
                    args[i] = objectEntry;
                }
                //else if (actionValue.TryGetValue(parameter.Name, out object argValue))
                else if (true)
                {
                    try
                    {
                        //if (parameter.ParameterType == typeof(string))
                        //{
                        //    string stringValue = argValue.ToString();
                        //    args[i] = stringValue;
                        //}
                        if (true)
                        {
                            string typeString = actionValue.First().Value.ToString();
                            Type parameterType = Type.GetType(typeString);
                            // args[i] = JsonSerializer.Deserialize<parameter.ParameterType>(argValue.ToString());
                            object deserializedObject = JsonSerializer.Deserialize(actionValue.First().Value.ToString(), parameterType);
                            // Assign the deserialized object to the args array
                            args[i] = deserializedObject;
                        }
                    }
                    catch (InvalidCastException ex)
                    {
                        Console.WriteLine($"Failed to convert parameter '{parameter.Name}' to type '{parameter.ParameterType}'. {ex.Message}");
                    }
                }
            }
            method.Invoke(null, args);
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
            Console.WriteLine($"Error renaming entry: {ex.Message}");
            // Handle the exception as per your application's requirements
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
                //Identifier destOu = destinationOu["destinationOu"];
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
                    //HandleMoveAction(action.Key, action.Value, objectEntry);
                    HandleAction(action.Key, action.Value, objectEntry);
                }
            }
            objectEntry.CommitChanges();
        }
    }

}

