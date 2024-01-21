using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPICommon.model
{
    // OUModel.cs

    public class OUIdentifier
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
    }

    public class Attributes
    {
        public string CN { get; set; }
        public string GivenName { get; set; }
        public string SN { get; set; }
        public string UserPrincipalName { get; set; }
    }

    public class Identifier
    {
        public string Attribute { get; set; }
        public string Value { get; set; }
    }

    public class ADObject
    {
        public OUIdentifier OUIdentifier { get; set; }
        public Attributes Attributes { get; set; }
        public Identifier Identifier { get; set; }
    }

}
