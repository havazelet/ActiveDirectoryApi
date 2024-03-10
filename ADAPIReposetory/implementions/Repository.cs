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
        string ldapPath = _configuration["ActiveDirectory:LDAPPath"];

        string commonName = (adObjectType == objectType) ? _configuration["ActiveDirectory:OU"] : _configuration["ActiveDirectory:CN"];
        try { 
        using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{adObject.OUIdentifier?.Value},{ldapPath}"))
        using (DirectoryEntry newObjectEntry = ouEntry.Children.Add($"{commonName}={adObject.Attributes[commonName]}", adObjectType))
        {
            foreach (var attribute in adObject.Attributes)
            {
                newObjectEntry.Properties[attribute.Key].Value = attribute.Value;
            }
            newObjectEntry.CommitChanges();
        }
        } catch (Exception ex) { 
            Console.WriteLine($"An error occurred: {ex.Message}"); 
        }
    }

    public void ModifyADObject(ModifyModel newAdObject, string adObjectType)
    {
        try
        {
            string ldapPath = _configuration["ActiveDirectory:LDAPPath"];
            string entryPath = $"LDAP://{newAdObject.Identifier?.Value},{ldapPath}";

            using (DirectoryEntry ouEntry = new DirectoryEntry(entryPath))
            {
     
                if (ouEntry is not null)
                {
                    foreach (var attribute in newAdObject.WriteAttribute)
                    {
                        if (ouEntry.Properties.Contains(attribute.Key))
                        {
                            ouEntry.Properties[attribute.Key].Value = attribute.Value;
                        }
                    }
                    ouEntry.CommitChanges();
                }
              
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }


}

