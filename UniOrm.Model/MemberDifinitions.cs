using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UniOrm.Model
{
    public class MemberDifinitions
    {
        public string VersionNum { get; set; }
        public int Id { get; set; }
        public int? BelongTypeId { get; set; }
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
        public DateTime? AddTime { get; set; }
    }
}
