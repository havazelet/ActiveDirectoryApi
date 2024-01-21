using ADAPIReposetory.interfaces;
using System.Data;
using System.DirectoryServices;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ADAPIReposetory.implementions
{
    public class Repository : IRepository
    {
      
        public void AddUserToGroupInOU(object OUIdentifier, object userId, object groupName)
        {
            try
            {   
                using (DirectoryEntry ouEntry = new DirectoryEntry($"LDAP://{OUIdentifier}"))
                {
                    using (DirectorySearcher ouSearcher = new DirectorySearcher(ouEntry))
                    {
                        ouSearcher.Filter = $"(&(objectClass=user)(sAMAccountName={userId}))";
                        SearchResult userResult = ouSearcher.FindOne();

                        if (userResult != null)
                        {
                            using (DirectoryEntry userEntry = userResult.GetDirectoryEntry())
                            {
                                using (DirectoryEntry groupEntry = ouEntry.Children.Find($"CN={groupName}", "group"))
                                {
                                    if (groupEntry != null)
                                    {
                                        groupEntry.Invoke("Add", new object[] { userEntry.Path });
                                        groupEntry.CommitChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (DirectoryServicesCOMException ex)
            {
                throw new ("error", ex);
            }
        }


        public void AddAdObject(object OUIdentifier, object userId, object groupName)
        {
            String strPath = "LDAP://DC=onecity,DC=corp,DC=fabrikam,DC=com";
        }

    }


}




