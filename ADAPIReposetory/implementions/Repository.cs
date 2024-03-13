using ADAPICommon.model;
using ADAPIReposetory.interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.DirectoryServices;

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

    public void HandleMoveAction(ModifyModel newAdObject)
    {
        using DirectorySearcher searcherDestination = new DirectorySearcher();
        searcherDestination.Filter = $"({newAdObject.Actions.Move?.DestinationOu?.Attribute}={newAdObject.Actions.Move?.DestinationOu?.Value})";
        SearchResult resultDestination = searcherDestination.FindOne();
        DirectoryEntry objectEntryDestination = resultDestination.GetDirectoryEntry();
        objectEntry.MoveTo(objectEntryDestination);
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

            if (newAdObject.Actions.Move is not null)
            {
                HandleMoveAction(newAdObject);  
            }
            objectEntry.CommitChanges();
        }
    }

}

