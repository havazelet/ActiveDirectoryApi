using ADAPICommon.model;
using ADAPIReposetory.interfaces;
using System;
using System.DirectoryServices;

namespace ADAPIReposetory.implementions;

public class Repository : IRepository
{
    public void AddADObject(ADObject adObject, string adObjectType)
    {
        try
        {
            using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://users/create/{adObject.OUIdentifier?.Value}"))
            {
                using (DirectoryEntry newObjectEntry = ouEntry.Children.Add($"CN={adObject.Attributes.CN}", adObjectType))
                {
                    newObjectEntry.Properties["OUIdentifier"].Value = adObject.OUIdentifier;
                    newObjectEntry.Properties["Attributes"].Value = adObject.Attributes;
                    newObjectEntry.Properties["Identifier"].Value = adObject.Identifier;

                    newObjectEntry.CommitChanges();                
                }
            }
        }
        catch (DirectoryServicesCOMException ex)
        {
            throw new InvalidOperationException("An error occurred while working with Active Directory.", ex);
        }
    }
}

