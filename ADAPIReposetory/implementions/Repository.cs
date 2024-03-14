using ADAPICommon.model;
using ADAPIReposetory.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.DirectoryServices;
using System.Reflection;
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

    //public void HandleMoveAction(Dictionary<string, object> actionValue, DirectoryEntry objectEntry)
    //{
    //    using DirectorySearcher searcherDestination = new DirectorySearcher();

    //    if (actionValue.ContainsKey("destinationOu"))
    //    {
    //        Dictionary<string, object> destOu = (Dictionary<string, object>)actionValue["destinationOu"];

    //        if (destOu.ContainsKey("attribute") && destOu.ContainsKey("value"))
    //        {
    //            searcherDestination.Filter = $"({destOu["attribute"]}={destOu["value"]})";
    //        }
    //    }
    //    SearchResult resultDestination = searcherDestination.FindOne();
    //    DirectoryEntry objectEntryDestination = resultDestination.GetDirectoryEntry();
    //    objectEntry.MoveTo(objectEntryDestination);
    // }

    public void HandleMoveAction(string actionKey, Dictionary<string, object> actionValue, DirectoryEntry objectEntry)
    {
        using DirectorySearcher searcherDestination = new DirectorySearcher();

        if (actionValue.ContainsKey("destinationOu"))
        {
            Dictionary<string, object> destOu = (Dictionary<string, object>)actionValue["destinationOu"];

            if (destOu.ContainsKey("attribute") && destOu.ContainsKey("value"))
            {
                searcherDestination.Filter = $"({destOu["attribute"]}={destOu["value"]})";
            }
        }
        SearchResult resultDestination = searcherDestination.FindOne();
        DirectoryEntry objectEntryDestination = resultDestination.GetDirectoryEntry();

        // Get the MoveTo method using reflection
        var moveToMethod = typeof(DirectoryEntry).GetMethod(actionKey, new[] { typeof(DirectoryEntry) });

        // Invoke the MoveTo method dynamically
        moveToMethod.Invoke(objectEntry, new object[] { objectEntryDestination });
    }


    //public void HandleAction(string actionKey, Dictionary<string, object> actionValue, DirectoryEntry objectEntry)
    //{
    //   // var moveToMethod = typeof(DirectoryEntry).GetMethod(actionKey, new[] { typeof(DirectoryEntry) });
    //    MethodInfo renameMethod = typeof(DirectoryEntry).GetMethod("Rename");
    //    object[] parameters = new object[] { actionValue["Key"] + "=" + actionValue["Value"] };
    //    renameMethod.Invoke(objectEntry, parameters);
    //    // moveToMethod.Invoke(objectEntry, new object[] { actionValue });
    //}


    public void HandleAction(string actionKey, Dictionary<string, object> actionValue, DirectoryEntry objectEntry)
    {
        MethodInfo method = typeof(DirectoryEntry).GetMethod(actionKey);
        if (method != null)
        {
            object[] parameters = new object[actionValue.Count];
            int index = 0;
            foreach (var kvp in actionValue)
            {
                parameters[index++] = kvp.Key + "=" + kvp.Value;
            }
            method.Invoke(objectEntry, parameters);
        }
        else
        {
            Console.WriteLine($"Method {actionKey} not found.");
            // Handle the case where the method is not found
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

