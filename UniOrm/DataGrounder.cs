using System;
using System.Collections.Generic; 
using System.Reflection; 
using SqlKata.Compilers;
using SqlKata;  
using System.Linq.Expressions;

namespace UniOrm
{
    public class DataGrounder : IDataGrounder
    {
        public string ConectionString { get; set; }
        public IOrmAdaptor OrmAdaptor { get; set; }
        public Compiler Compiler { get; set; }
        private DBType dbType;
        public DBType DBType
        {
            get
            {
                return dbType;
            }
            set
            {
                dbType = value;
                switch (DBType)
                {
                    case DBType.Sqlite:
                        Compiler = new SqliteCompiler();
                        break;
                    case DBType.InMemory:
                    case DBType.SqlServer:
                        Compiler = new SqlServerCompiler();
                        break;
                    case DBType.Mysql:
                        Compiler = new MySqlCompiler();
                        break;
                    case DBType.Postgre:
                        Compiler = new PostgresCompiler();
                        break;
                }
            }
        }

        public MethodInfo DeleteMethod { get; set; }
        public Func<object, string, object[], int> DeleteAction { get; set; }
        public object OrmObject { get; set; }
        public SqlResult ToSql(Query query)
        {
            return Compiler.Compile(query);
        }
        public int delete(params object[] deleobects)
        {
            if (deleobects != null && deleobects.Length > 0)
            {
                OrmAdaptor.GetDeleteAction(OrmObject, deleobects);
                return deleobects.Length;
            }
            return 0; 
        }

        //public int delete(object condition ) 
        //{
        //   var sss= ToSql(condition);
        //    return DeleteAction(OrmObject, sss.Sql, sss.Bindings.ToArray());
        //}

        public Func<object, object[], object> InsertAction { get; set; }
        public object Insert<T>(object[] data)
        {
            return InsertAction(OrmObject, data);
        }
        public Func<object, object[], bool> OpenAction { get; set; }
        public object[] OpenArgs { get; set; }
        public void Open()
        {
            OpenAction(OrmObject, OpenArgs);
        }
        public Func<object, string, int, int, object[], QueryResult> QueryPageAction { get; set; }
        public Func<object, string, object[], List<dynamic>> QueryAction { get; set; }
        public QueryResult QueryPage(string queryString, int strartPageIndex = 0, int PageSize = 30, params object[] paramters)
        {
            return QueryPageAction(OrmObject, queryString, strartPageIndex, PageSize, paramters);
        }
        public List<dynamic> Query(string sql, params object[] args)
        {
            if (args == null || (args.Length == 1 && args[0] == null))
            {
                return QueryAction(OrmObject, sql, null);
            }
            else
            {
                return QueryAction(OrmObject, sql, args);
            }
        }
        public IEnumerable<TSource> QueryList<TSource>(Expression<Func<TSource, bool>> predicate) where TSource : class, new()
        {
            return OrmAdaptor.QueryList(OrmObject, predicate);
        }
        public void SetType(Type type, Action<Type> mappingAction)
        {
            throw new NotImplementedException();
        }
        public Func<object, object[], int> UpdateAction { get; set; }
        public int Update(object[] data)
        {
            return UpdateAction(OrmObject, data);
        }
        public Func<object, string, object[], int> SqlCommandAction { get; set; }
        public int GetSqlCommandAction(string sql, params object[] objparameters)
        {
            return SqlCommandAction(OrmObject, sql, objparameters);
        }
        public Func<object, string, object[], object> SqlExecuteScalarAction { get; set; }
        public object GetSqlExecuteScalarAction(string sql, params object[] objparameters)
        {
            return SqlExecuteScalarAction(OrmObject, sql, objparameters);
        }
        public Action<object> Close { get; set; }
        public void Dispose()
        {
            Close(OrmObject);
        }
        public Query From<T>()
        { 
            var q = new Query();
            q.DataGrounder = this;
            var wholeTablename = q.DataGrounder.OrmAdaptor.ConnectionConfig.DefaultDbPrefixName + typeof(T).Name;
            return q.From(wholeTablename); 
        }
        public DataGrouderT<T> ToTyped<T>() where T : class, new()
        {
            var typedb = new DataGrouderT<T>();
            typedb.Parent = this;


            return typedb;
        }
        public Query From(string tablename)
        {
            var newquery = new Query();
            newquery.DataGrounder = this;
            var wholeTablename = newquery.DataGrounder.OrmAdaptor.ConnectionConfig.DefaultDbPrefixName + tablename;
            return newquery.From(wholeTablename); 
        }
        public virtual List<dynamic> Query(Query query)
        {
            var sss = ToSql(query);
            return Query(sss.Sql, sss.Bindings.ToArray());

        }
    }
}
