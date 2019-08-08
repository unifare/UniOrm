 
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Text;
using System.Text.RegularExpressions; 
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using Plugin.Common;
using System.Collections.Concurrent;

namespace BasicPlugin
{
 
    public class HttpUtility
    {
        static ConcurrentBag<RegexEn> regexEns = new ConcurrentBag<RegexEn>();
   
        public static string GetIsTriger(List<dynamic> assrules, string url)
        {
            //var assrules = assrulesPbj.AsDynamic();
            //var url = urlobj.AsDynamic() ;
            // dynamic assrules = new JObjectAccessor(assrulesPbj as string);

            //var assrules = (List<TrigerRuleInfo>)runtimeArgument.ResourceList.FirstOrDefault(p => p.KeyName == "allrules").RuntimeValue;
            //var url = runtimeArgument.ResourceList.FirstOrDefault(p => p.KeyName == "url").Value;
            //List<sdf> sdf = new List<sdf>();
            //sdf.Count
            //var allrows = (int)inparams1.GetPropertyValue("Count");

            for (var i = 0; i < assrules.Count; i++)
            {
                var TrigerRow = assrules[i];

                string ruleText = TrigerRow.Rule;
                if (string.IsNullOrEmpty(ruleText))
                {
                    continue;
                }
                else
                {
                    Regex reg = null;
                    var exsUrlrule = regexEns.FirstOrDefault(p => p.RuleText == TrigerRow.Rule);
                    if (exsUrlrule == null)
                    {
                        reg = new Regex(TrigerRow.Rule);
                        exsUrlrule = new RegexEn() { Regex = reg, RuleText = TrigerRow.Rule };
                        regexEns.Add(exsUrlrule);
                    }
                    else
                    {
                        reg = exsUrlrule.Regex;
                    }
                    var ismach = reg.IsMatch(url);
                    if (ismach)
                    {
                        return TrigerRow.ComposityId;
                    }
                }
            }


            return "";
        }
  
        public static object GetRequestUrl(ActionExecutingContext context)
        {
            return context.HttpContext.Request.Path.Value;

        }

   
        public static void ResponseText(ActionExecutingContext context, string returnObject)
        {
            //var 
            var resp = context.HttpContext.Response;
            resp.ContentType = "text/html";

            using (StreamWriter sw = new StreamWriter(resp.Body))
            {
                sw.Write(returnObject);
            }
        }
       
        public static void ResponseAjaxt(ActionExecutingContext context,object returnObject )
        {
            //var 
            var resp = context.HttpContext.Response;
            resp.ContentType = "application/json";

            using (StreamWriter sw = new StreamWriter(resp.Body))
            {
                sw.Write(returnObject.ToJson());
            }
        }
    }
}
