using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPIService.interfaces;

public interface IUserInterface
{
    public void GetUser(object OUIdentifier, object userId, object groupName);
}
