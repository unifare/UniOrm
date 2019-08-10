using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text; 
namespace UniOrm.Common
{
    public class MagicExtension
    {
        public static object BackToInst(object obj)
        {
            if (obj == null)
                return null;
            var ww = obj as PrivateReflectionDynamicObjectBase;
            if( ww==null)
            { 
                return obj;
            }
            return ww.Instance;
        }
    }
}
