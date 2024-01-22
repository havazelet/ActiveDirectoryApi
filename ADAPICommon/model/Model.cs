using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADAPICommon.model;

public class ADObject
{
    public OUIdentifierModel OUIdentifier { get; set; }
    public AttributesModel Attributes { get; set; }
    public IdentifierModel Identifier { get; set; }
}
