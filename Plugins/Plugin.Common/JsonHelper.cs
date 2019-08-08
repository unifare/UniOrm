﻿using Newtonsoft.Json;
using System;

namespace Plugin.Common
{
    public static class JsonHelper
    {
        public static T ToObject<T>(this string inputString)
        {
            return JsonConvert.DeserializeObject<T>(inputString);
        }
        public static string ToJson(this object inpar)
        {
            if (inpar == null)
            {
                return "{}";
            }
            else
            {
                return JsonConvert.SerializeObject(inpar);
            }
        }
    }
}
