using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AConState.Core.Definitions
{
    public class MethodDefinition:MemberDifinitions
    {
        public override MemberTypes MemberType { get; set; }
       
        public virtual string ReturnType { get; set; }
        public string[] ParameterInfo { get; set; }
    }
}
