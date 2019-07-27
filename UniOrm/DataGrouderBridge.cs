using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UniOrm
{
    public class DataGrouderBridge
    {
        //public IDataGrounder DataGrounder { get; set; }
        public List<Assembly> OrmAssemlbies { get; set; }
        public DataGrouderBridge()
        {
            OrmAssemlbies = new List<Assembly>();
        }
        public IOrmAdaptor OrmAdaptor { get; set; }
        //public object DbType { get; set; }
        public Func<object[], object> CreateAction { get; set; }
        public Func<object[]> CeateActionParamters { get; set; }
        // public ConfigDeleteMapping(Type type,)
        public void ConfigDefaultInstance(Func<object[]> ceateActionParamters, Func<object[], object> createAction)
        {
            CeateActionParamters = ceateActionParamters;

            CreateAction = createAction;
        }

        public IDataGrounder CreateDefaultInstance()
        {

            var dataGrounder = new DataGrounder();
            dataGrounder.OrmObject = CreateAction(CeateActionParamters());
            CopyActions(dataGrounder);
            return dataGrounder;
        }
        public IDataGrounder CreateDefaultInstance(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return CreateDefaultInstance();
            }

            var dataGrounder = new DataGrounder();
            CopyActions(dataGrounder);
            dataGrounder.OrmObject = CreateAction(CeateActionParamters());
            dataGrounder.OrmAdaptor.ConnectionConfig.Connectionstring = connectionString;

            return dataGrounder;
        }
        public IDataGrounder CreateDefaultInstance(DBType dBType, string serverName, string dbname, string username, string pwd, string filePath = null, int? portNum = null)
        {
            var connectionString = string.Empty;
            var dataGrounder = new DataGrounder();
            CopyActions(dataGrounder);
            dataGrounder.OrmAdaptor.ConnectionConfig.DBType = (int)dBType;
            if (dBType != DBType.InMemory)
            {
                //var fuType = FlunentDBType.Sqlite;

                switch (dBType)
                {
                    case DBType.Sqlite:
                        connectionString = " Data Source = " + filePath;
                        break;
                    case DBType.SqlServer:
                        if (portNum != null)
                        {
                            connectionString = string.Concat("Server=", serverName, ";Database=", dbname, "; Port=", portNum, "; User=", username, "; Password=", pwd, "; ");

                        }
                        else
                        {
                            connectionString = string.Concat("Server=", serverName, ";Database=", dbname, "; Port=", portNum, "; User=", username, "; Password=", pwd, "; ");
                        }

                        break;
                    case DBType.Mysql:
                        if (portNum != null)
                        {
                            connectionString = string.Concat("Server=", serverName, ";Database=", dbname, "; Port=", portNum, "; User=", username, "; Password=", pwd, "; ");
                        }
                        else
                        {
                            connectionString = string.Concat("Server=", serverName, ";Database=", dbname, "; Port=", portNum, "; User=", username, "; Password=", pwd, "; ");
                        }
                        break;
                    case DBType.Postgre:
                        if (portNum != null)
                        {
                            connectionString = string.Concat("Server=", serverName, ";Database=", dbname, "; Port=", portNum, "; User=", username, "; Password=", pwd, "; ");
                        }
                        else
                        {
                            connectionString = string.Concat("Server=", serverName, ";Database=", dbname, "; Port=", portNum, "; User=", username, "; Password=", pwd, "; ");
                        }
                        break;
                }

            }

            dataGrounder.OrmAdaptor.ConnectionConfig.Connectionstring = connectionString;
            dataGrounder.OrmObject = CreateAction(CeateActionParamters());
            return dataGrounder;
        }
        private void CopyActions(DataGrounder dataGrounder)
        {
            dataGrounder.DBType = (DBType)OrmAdaptor.ConnectionConfig.DBType;
            dataGrounder.OrmAdaptor = OrmAdaptor;
            dataGrounder.InsertAction = InsertAction;
            dataGrounder.OpenAction = OpenAction;
            dataGrounder.DeleteAction = DeleteAction;
            dataGrounder.QueryPageAction = QueryPageAction;
            dataGrounder.QueryAction = QueryAction;
            dataGrounder.SqlCommandAction = ExcuteAction;
            dataGrounder.SqlExecuteScalarAction = SqlExecuteScalarAction;
            dataGrounder.UpdateAction = UpdateAction;
            dataGrounder.Close = Close;
        }
        public Action<object> Close { get; set; }
        public void ConfigCloseAction(Action<object> action)
        {
            Close = action;
        }
        public IDataGrounder CreateNewInstance(string conectionString, object dbType, Func<object[], object> createAction)
        {
            var dataGrounder = new DataGrounder();
            //CreateAction = createAction;
            dataGrounder.OrmObject = createAction(new object[] { conectionString, dbType });
            CopyActions(dataGrounder);
            return dataGrounder;
        }
        public object[] OpenArgs { get; set; }
        public Func<object, object[], bool> OpenAction { get; set; }
        public MethodInfo OpenMethod { get; set; }
        public void ConfigOpenAction(Func<object, object[], bool> action)
        {
            //var ormObjectType = DataGrounder.OrmObject.GetType();
            //OpenMethod = ormObjectType.GetMethod(openMethodName);
            //var dGrounder = (DataGrounder as DataGrounder);
            OpenAction = action;
            //OpenArgs = paramters;
        }
        public Func<object, object[], object> InsertAction { get; set; }
        public Func<object, string, object[], int> DeleteAction { get; set; }
        public MethodInfo InsertMethod { get; set; }
        public void ConfigInsertAction(Func<object, object[], object> action)
        {

            InsertAction = action;

        }

        public Func<object, object[], int> UpdateAction { get; set; }
        public MethodInfo UpdateMethod { get; set; }
        public void ConfigUpdateAction(Func<object, object[], int> action)
        {
            UpdateAction = action;
        }

        public Func<object, string, object[], int> ExcuteAction { get; set; }
        public void ConfigExcuteAction(Func<object, string, object[], int> action)
        {
            ExcuteAction = action;
        }
        public Func<object, string, object[], object> SqlExecuteScalarAction { get; set; }
        public void ConfigSqlExecuteScalarAction(Func<object, string, object[], object> action)
        {
            SqlExecuteScalarAction = action;
        }

        public Func<object, string, object[], List<dynamic>> QueryAction { get; set; }
        public Func<object, string, int, int, object[], QueryResult> QueryPageAction { get; set; }
        public void ConfigQueryPageAction(Func<object, string, int, int, object[], QueryResult> action)
        {

            QueryPageAction = action;
        }
        public void ConfigQueryAction(Func<object, string, object[], List<dynamic>> action)
        {

            QueryAction = action;
        }

        //public void Select( )
        //{

        //    QueryAction = action;
        //}
        //public IDataGrounder MapDelete(MethodInfo methodInfo, Func<IDataGrounder , string, List<object>,bool> deleteAction)
        //{
        //    DataGrounder dataGrounder = new DataGrounder();
        //    dataGrounder.DeleteAction = deleteAction;
        //    return (IDataGrounder)dataGrounder;
        //}
        //public Func<DataGrounder, string, List<object>, bool> DeleteAction;
    }
}
