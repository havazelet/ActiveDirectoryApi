using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPICommon.model;

public class ModifyModel
{
    public Identifier Identifier { get; set; }
    public Dictionary<string, string> WriteAttribute { get; set; }
    public Identifier Actions { get; set; }
}
