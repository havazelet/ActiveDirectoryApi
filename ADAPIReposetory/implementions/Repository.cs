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
            using DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://users/create/{adObject.OUIdentifier?.Value}");
            using (DirectoryEntry newObjectEntry = ouEntry.Children.Add($"CN={adObject.Attributes?.Cn}", adObjectType))
            {
                newObjectEntry.Properties["Cn"].Value = adObject.Attributes.Cn;
                newObjectEntry.Properties["GivenName"].Value = adObject.Attributes.GivenName;
                newObjectEntry.Properties["Sn"].Value = adObject.Attributes.Sn;
                newObjectEntry.Properties["UserPrincipalName"].Value = adObject.Attributes.UserPrincipalName;

                newObjectEntry.CommitChanges();
            }
        }
        catch (DirectoryServicesCOMException ex)
        {
            throw new InvalidOperationException("An error occurred while working with Active Directory.", ex);
        }
    }
}

