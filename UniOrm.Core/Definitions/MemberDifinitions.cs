using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AConState.Core.Definitions
{
    public class MemberDifinitions
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public virtual MemberTypes MemberType { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the field is private.
        //
        // Returns:
        //     true if the field is private; otherwise; false.
        public bool IsPrivate { get; set; }

        //
        // Summary:
        //     Gets a value indicating whether the field is static.
        //
        // Returns:
        //     true if this field is static; otherwise, false.
        public bool IsStatic { get; set; }
    }
}
