using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace RockwellBlog.Enums
{
    public enum PublishState
    {
        //indexed from 0 by default. Can change starting index by saying propertyName = num. Ex ProductionReady = 5
        [Description("Production Ready")]
        ProductionReady,
        PreviewReady,
        NotReady
    }
}
