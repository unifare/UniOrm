using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace AConState.Core.Definitions
{
    public class TypeDefinition
    {
        public long Id { get; set; }
        public string ClassName { get; set; } 
        public virtual GenericParameterAttributes GenericParameterAttributes { get; set; } 
        public bool IsVisible { get; set; } 
        public bool IsNotPublic { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is declared public.
        //
        // Returns:
        //     true if the System.Type is declared public and is not a nested type; otherwise,
        //     false.
        public bool IsPublic { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether a class is nested and declared public.
        //
        // Returns:
        //     true if the class is nested and declared public; otherwise, false.

        //
        // Summary:
        //     Gets the attributes associated with the System.Type.
        //
        // Returns:
        //     A System.Reflection.TypeAttributes object representing the attribute set of the
        //     System.Type, unless the System.Type represents a generic type parameter, in which
        //     case the value is unspecified.
        public TypeAttributes Attributes { get; set; }

        //
        // Summary:
        //     Gets a value indicating whether the System.Type is nested and visible only to
        //     classes that belong to either its own family or to its own assembly.
        //
        // Returns:
        //     true if the System.Type is nested and visible only to classes that belong to
        //     its own family or to its own assembly; otherwise, false.
        public bool IsNestedFamORAssem { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the fields of the current type are laid out automatically
        //     by the common language runtime.
        //
        // Returns:
        //     true if the System.Type.Attributes property of the current type includes System.Reflection.TypeAttributes.AutoLayout;
        //     otherwise, false.
        public bool IsAutoLayout { get; set; }

        //
        // Summary:
        //     Gets the namespace of the System.Type.
        //
        // Returns:
        //     The namespace of the System.Type; null if the current instance has no namespace
        //     or represents a generic parameter.
        public string Namespace { get; set; }

        //
        // Summary:
        //     Gets the assembly-qualified name of the type, which includes the name of the
        //     assembly from which this System.Type object was loaded.
        //
        // Returns:
        //     The assembly-qualified name of the System.Type, which includes the name of the
        //     assembly from which the System.Type was loaded, or null if the current instance
        //     represents a generic type parameter.
        public string AssemblyQualifiedName { get; set; }

        public string FullName { get; set; }


        public Guid GUID { get; set; }

        public bool IsValueType { get; set; }

        public bool IsInterface { get; set; }


        public virtual bool IsGenericType { get; set; }

        public bool IsArray { get; set; }



        public bool IsSpecialName { get; set; }

        public virtual bool IsEnum { get; set; }

        public bool IsSealed { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsClass { get; set; }

    }
}
