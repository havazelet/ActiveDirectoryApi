﻿using ADAPICommon.model;
using ADAPIReposetory.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.DirectoryServices;

namespace ADAPIReposetory.implementions;

public class Repository : IRepository
{
    private readonly ILogger<Repository> _logger;

    public Repository(ILogger<Repository> logger)
    {
        _logger = logger;
    }


    public void AddADObject(ADObject adObject, string adObjectType)
    {
        string commonName = (adObjectType == "OrganizationalUnit") ? "Ou" : "Cn";

        using DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{adObject.OUIdentifier?.Value},DC=osher,DC=lab");
        using (DirectoryEntry newObjectEntry = ouEntry.Children.Add($"{commonName}={adObject.Attributes[commonName]}", adObjectType))
        {
            foreach (var attribute in adObject.Attributes)
            {
                newObjectEntry.Properties[attribute.Key].Value = attribute.Value;
            }
            newObjectEntry.CommitChanges();
        }
    }
}

