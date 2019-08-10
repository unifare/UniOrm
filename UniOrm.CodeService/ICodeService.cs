 
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace UniOrm.Model.DataService
{
    public interface ICodeService:IDisposable
    {
        bool IsOpenSessionEveryTime { get; set; }
        SystemACon GetSystemACon();
        List<ComposeEntity> GetConposity(string  id, string name=null);
        TypeDefinition GetTypeDefinition(string typeName);
        int InsertCode<T>(T objcode) where T : class, new();
        bool DeleteSimpleCode(object simplequery);
        List<AConFlowStep> GetAConStateSteps(string id);
         List<T> GetSimpleCode<T>(object simplequery) where T : class, new();
        IEnumerable<T> GetSimpleCodeLinq<T>(Expression<Func<T, bool>> predicate) where T : class, new();
   
        int UpdateSimpleCode (object obj);

        AdminUser GetAdminUser(string username, string password);
    }
}
