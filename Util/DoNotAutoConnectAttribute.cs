using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.Util
{
    [AttributeUsage(AttributeTargets.Property)]
    class DoNotAutoConnectAttribute : Attribute
    {
    }
}
