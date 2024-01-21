using ADAPICommon.model;
using ADAPIReposetory.interfaces;
using System;
using System.DirectoryServices;

namespace ADAPIReposetory.implementions;

public class Repository : IRepository
{
    public void AddADObject(ADObject adObject)
    {
        try
        {
            using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{adObject.OUIdentifier.Value}"))
            {
                using (DirectorySearcher ouSearcher = new DirectorySearcher(ouEntry))
                {
                    string filter = $"(&(objectClass=user)({adObject.OUIdentifier.Attribute}={adObject.Attributes.CN}))";
                    ouSearcher.Filter = filter;

                    SearchResult objectResult = ouSearcher.FindOne();

                    if (objectResult != null)
                    {
                        using (DirectoryEntry objectEntry = objectResult.GetDirectoryEntry())
                        {
                            using (DirectoryEntry groupEntry = ouEntry.Children.Find($"CN={adObject.Identifier.Value}", "group"))
                            {
                                if (groupEntry != null)
                                {
                                    groupEntry.Invoke("Add", new object[] { objectEntry.Path });
                                    groupEntry.CommitChanges();
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Group with CN={adObject.Identifier.Value} not found.");
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Object with {adObject.OUIdentifier.Attribute}={adObject.Attributes.CN} not found.");
                    }
                }
            }
        }
        catch (DirectoryServicesCOMException ex)
        {
            throw new InvalidOperationException("An error occurred while working with Active Directory.", ex);
        }
    }
}

