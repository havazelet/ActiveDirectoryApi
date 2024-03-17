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
    //}

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


    //public void HandleAction(string actionKey, Dictionary<string, object> actionValue, DirectoryEntry objectEntry)
    //{
    //    MethodInfo method = typeof(DirectoryEntry).GetMethod(actionKey);
    //    if (method != null)
    //    {
    //        object[] parameters = new object[actionValue.Count];
    //        int index = 0;
    //        foreach (var kvp in actionValue)
    //        {
    //            parameters[index++] = kvp.Key + "=" + kvp.Value;
    //        }
    //        method.Invoke(objectEntry, parameters);
    //    }
    //    else
    //    {
    //        Console.WriteLine($"Method {actionKey} not found.");
    //        // Handle the case where the method is not found
    //    }
    //}


    //public void HandleAction(string actionKey, Dictionary<string, string> actionValue, DirectoryEntry objectEntry)
    //{
    //   // Rename(objectEntry, actionValue.FirstOrDefault().Value);
    //    try
    //    {
    //        if (actionValue.FirstOrDefault().Value is not null)
    //        {
    //            // DirectoryEntry uEntry = new DirectoryEntry(objectEntry);
    //            objectEntry.Invoke(actionKey, new object[] { actionValue.FirstOrDefault().Value });
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Error performing action '{actionKey}': Value not found for 'CN' key in actionValue dictionary.");
    //        }       
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error performing action '{actionKey}': {ex.Message}");
    //    }
    //}


    public void HandleAction(string actionKey, Dictionary<string, string> actionValue, DirectoryEntry objectEntry)
    {
        try
        {
            if (actionValue.FirstOrDefault().Value is not null)
            { 
                var method = GetType().GetMethod(actionKey);
                method.Invoke(null, new object[] { objectEntry, actionValue.FirstOrDefault().Value });
            }
            else
            {
                Console.WriteLine($"Error performing action '{actionKey}': Value not found for 'CN' key in actionValue dictionary.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error performing action '{actionKey}': {ex.Message}");
        }
    }

    public static void Rename(DirectoryEntry objectEntry, string newName)
    {
        objectEntry.Rename(newName);
    }

    public static void ResetPassword(DirectoryEntry objectEntry, string newName)
    {
        objectEntry.Invoke("setPassword", new object[] { newName });
    }

    public static void MoveTo(DirectoryEntry objectEntry, Identifier newName)
    {
        using DirectorySearcher searcherDestination = new DirectorySearcher();
      //  Dictionary<string, string> dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(newName);

        //if (dictionary.ContainsKey("destinationOu"))
        //{
        //    Dictionary<string, string> destOu = dictionary["destinationOu"];

        //    if (destOu.ContainsKey("attribute") && destOu.ContainsKey("value"))
        //    {
        //        searcherDestination.Filter = $"({destOu["attribute"]}={destOu["value"]})";
        //    }
        //}
        //SearchResult resultDestination = searcherDestination.FindOne();
        //DirectoryEntry objectEntryDestination = resultDestination.GetDirectoryEntry();
        //objectEntry.MoveTo(objectEntryDestination);
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

