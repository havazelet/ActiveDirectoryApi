using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPICommon.model;

public class ADObject
{
    public OUIdentifier OUIdentifier { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
    public Identifier Identifier { get; set; }
}
