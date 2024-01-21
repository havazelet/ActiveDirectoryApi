using ADAPICommon.model;
using ADAPIReposetory;
using ADAPIReposetory.implementions;
using ADAPIService.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPIService.implementations;

public class Service : IServiceInterface
{
    public void CreateADObject(ADObject userModel)
    {
        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                Repository ADrequest = new Repository();
                ADrequest.AddADObject(userModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
