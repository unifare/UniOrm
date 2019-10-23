using UniOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection; 

namespace UniOrm
{
    public enum DBType
    {

        Sqlite,
        SqlServer,
        Mysql,
        Postgre,
        InMemory
    }
    public enum OrmType
    {
        None,
        Sql,
        Type,
    }
    public interface IOrmAdaptor
    {
        DbConnectionConfig ConnectionConfig { get; set; }
        IResover Resover { get; set; }
        bool IsSelfDefineCreation { get; set; }
        Type DataGrounderType { get; set; }
        OrmType OrmType { get;   }
        OrmName OrmName { get; set; }
        List<Assembly> GetAssemblies();
        List<Assembly> RegistedModelAssemblies { get; set; }
        RegistedModelTypes RegistedModelTypes { get; }
        object GetCeateAction(params object[] objparameters);
        bool GetOpenSessionAction(object dbOperator, params object[] objparameters);
        object GetInsertAction(object dbOperator, params object[] objparameters);
        int GetSqlCommandAction(object dbOperator, string sql, params object[] objparameters);
        object GetSqlExecuteScalarAction(object dbOperator, string sql, params object[] objparameters);
        object GetDeleteAction(object dbOperator, params object[] objparameters);
        int GetUpdatection(object dbOperator, params object[] objparameters);
        QueryResult GetQueryPageAction(object dbOperator, string sql, int startpage, int pagesize, params object[] args);
        List<dynamic> GetQuertyAction (object dbOperator, string sql, params object[] args);
        IEnumerable<T> GetSqlQueryAction<T>(object dbOperator, string sql, params object[] args) where T : class, new();
        QueryResult GetSqlQueryPageAction<T>(object dbOperator, string sql, int startpage, int pagesize, params object[] args) where T : class, new();
        IEnumerable<TSource> QueryList<TSource>(object dbOperator, Expression<Func<TSource, bool>> predicate) where TSource : class, new();

        bool IsExcuteImmediately { get; set; }
        void GetCloseAction(object dbOperator);
        //void GetBeginTransicationAction();
        object[] GetCeateActionParamters();
        //void GetSubmitTransicationAction();
    }
}
