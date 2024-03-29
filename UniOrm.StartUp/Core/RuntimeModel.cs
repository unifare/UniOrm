﻿using Fasterflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UniOrm.Common;
using UniOrm.Model;

namespace UniOrm.Core
{
    public class RuntimeModel : IDisposable
    {
        public static Dictionary<string, object> StaticResouceInfos = new Dictionary<string, object>();
        public RuntimeModel ParentRuntimeModel { get; set; }
        static readonly string logName = "AConState.Application.TypeMaker";
        public string NextRunTimeKey = "_NextRunTimeKey";
        public Dictionary<string, object> ResouceInfos { get; set; }
        public IConfig config { get; set; }
        public ComposeTemplate ComposeTemplate { get; set; }
        public object DBground { get; set; }
        public object OpenDBSession(object dbFactory, string ormname, string connectionstring)
        {
            if (DBground == null)
            {
                var adp = dbFactory.GetIndexer(ormname);
                  //  .GetIndexer(ormname);

                if (!string.IsNullOrEmpty(connectionstring))
                {
                    DBground = adp.CallMethod("CreateDefaultInstance", connectionstring);
                }
                else
                {
                    DBground = adp.CallMethod("CreateDefaultInstance");
                }
                return DBground;
            }
            else
            {
                return DBground;
            }
        }

        public RuntimeModel(IConfig _config)
        {
            config = _config;
            ResouceInfos = new Dictionary<string, object>();
        }



        public void Dispose()
        {
            ParentRuntimeModel.Dispose();
        }

        public int HashCode { get; set; }
        public ComposeEntity ComposeEntity { get; set; }

        public bool SetComposityResourceValue(string key, object value)
        {

            if (!ResouceInfos.ContainsKey(key))
            {
                ResouceInfos.Add(key, value);
            }
            else
            {
                ResouceInfos[key] = value;
            }
            return true;
        }
        public object Resuce(string argNames)
        {
            if (!ResouceInfos.ContainsKey(argNames))
            {
                if (!StaticResouceInfos.ContainsKey(argNames))
                {
                    return null;
                }
                else
                {
                    return StaticResouceInfos[argNames];
                }
            }
            else
            {
                return ResouceInfos[argNames];
            }
        }
        public List<object> GetPoolResuce(IEnumerable<string> argNames)
        {
            var objParams = new List<object>();
            //if( string.IsNullOrEmpty(spec.InPoolResKeys))
            // {
            //     return objParams;
            // }
            // var inpoolkeys = spec.InPoolResKeys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) ;

            foreach (var paraskey in argNames)
            {
                var foundobject = Resuce(paraskey);
                //var objd = ResouceInfos.FirstOrDefault(p => p.KeyName == paraskey);
                if (foundobject == null)
                {
                    Logger.LogError(logName, "GetPoolResuce -> {0} is null", paraskey);
                    objParams.Add(null);
                }
                else
                {
                    objParams.Add(foundobject);
                }
            }
            return objParams;
        }

    }
}
