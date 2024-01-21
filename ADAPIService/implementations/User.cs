using ADAPIReposetory;
using ADAPIReposetory.implementions;
using ADAPIService.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPIService.implementations;

public class User : IUserInterface
{
    public void GetUser(object OUIdentifier, object userId, object groupName)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                Repository ADrequest = new Repository();
                ADrequest.AddUserToGroupInOU(OUIdentifier, userId, groupName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
