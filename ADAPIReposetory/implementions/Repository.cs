using ADAPICommon.model;
using ADAPIReposetory.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.DirectoryServices;
using System.Reflection;
using System.Text.Json;

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

    public static void Rename(DirectoryEntry objectEntry, string newName)
    {
        objectEntry.Rename("CN=" + newName);
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
        }
    }

    public void ModifyADObject(ModifyModel newAdObject, string adObjectType, DirectoryEntry objectEntry)
    {
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
        objectEntry.CommitChanges();
    }
}

