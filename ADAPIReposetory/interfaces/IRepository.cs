using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ADAPIReposetory.interfaces
{
    internal interface IRepository
    {
        public void AddUserToGroupInOU(object OUIdentifier, object userId, object groupName);
    }
}
