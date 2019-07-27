using UniOrm.Adaption;
using SqlKata;
using SqlKata.Compilers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace UniOrm
{
    public interface IDataGrounder : IDisposable
    {
        string ConectionString { get; set; }
        IOrmAdaptor OrmAdaptor { get; set; }
        DBType DBType { get; set; }
        SqlResult ToSql(Query query);
        //MethodInfo DeleteMethod { get; set; }
        object OrmObject { get; set; }
        //void SetType(Type type, Action<Type> mappingAction);
        //Action<object[]> OpenAction { get; set; }
        //object[] OpenArgs { get; set; }
        void Open();
        //Func<object[], object> InsertAction { get; set; }
        object Insert<T>(params object[] data);

        // Func<object[], int> UpdateAction { get; set; }
        int Update(params object[] data);

        //Func<string  , object[], int> DeleteAction { get; set; }
        int delete(params object[] deleobects);
        //int delete(object simplequery );
        // Func<string, object[] ,int  , int  , dynamic> QueryAction { get; set; }
        QueryResult QueryPage(string queryString, int strartPageIndex = 0, int PageSize = 30, params object[] paramters);
        DataGrouderT<T> ToTyped<T>() where T: class, new(); 
        List<dynamic> Query(string sql, params object[] args);
        int GetSqlCommandAction(string sql, params object[] objparameters);
        object GetSqlExecuteScalarAction(string sql, params object[] objparameters);
        Query From<T>();
        Query From(string tablename);
        //IEnumerable QueryList<T>(Expression<Func<T, bool>> expression, object paramters, int strartPageIndex = 0, int PageSize = 30);
        IEnumerable<TSource> QueryList<TSource>( Expression<Func<TSource, bool>> predicate) where TSource : class, new();

    }
}
