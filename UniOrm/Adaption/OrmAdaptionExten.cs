
using System;
using System.Linq;
using System.Reflection; 

namespace UniOrm.Adaption
{
    public static class OrmAdaptionExten
    {
        public static MethodInfo GetGenericMethod(Type targetType, string name, BindingFlags flags, params Type[] parameterTypes)
        {
            var methods = targetType.GetMethods(flags).Where(m => m.Name == name && m.IsGenericMethod);
            foreach (MethodInfo method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != parameterTypes.Length)
                    continue;

                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        break;
                }
                return method;
            }
            return null;
        }

        public static void AddOrm(this IDbFactory dataGrounders, IOrmAdaptor ormAdaptor)
        { 
            dataGrounders.AddOrmDataGrounder(ormAdaptor.OrmName , ormAdaptor.GetAssemblies());
            //dataGrounders.Orms[ormAdaptor.Name].ConectionString = ormAdaptor.DbType;
            dataGrounders.Orms[ormAdaptor.OrmName ].OrmAdaptor = ormAdaptor;
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigDefaultInstance(ormAdaptor.GetCeateActionParamters, ormAdaptor.GetCeateAction);
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigOpenAction(ormAdaptor.GetOpenSessionAction);
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigInsertAction(ormAdaptor.GetInsertAction);
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigUpdateAction(ormAdaptor.GetUpdatection);
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigExcuteAction(ormAdaptor.GetSqlCommandAction);
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigSqlExecuteScalarAction(ormAdaptor.GetSqlExecuteScalarAction);
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigQueryPageAction(ormAdaptor.GetQueryPageAction);
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigQueryAction(ormAdaptor.GetQuertyAction);
            dataGrounders.Orms[ormAdaptor.OrmName ].ConfigCloseAction(ormAdaptor.GetCloseAction);
            //dataGrounders.Orms["sqlsugar"].ConfigInsertAction((_args) =>
            //{
            //    Type insrtobjType;
            //    if (_args != null && _args.Length > 0)
            //    {
            //        insrtobjType = _args[0].GetType();
            //    }
            //    else
            //    {
            //        return 0;
            //    }
            //    var dGrounder = dataGrounders.Orms["sqlsugar"].DataGrounder;
            //    var db = dGrounder.OrmObject as SqlSugarClient;
            //    var ortype = typeof(SqlSugarClient);
            //    var method = GetGenericMethod(ortype, "Insertable", BindingFlags.Public | BindingFlags.Instance, new Type[] { insrtobjType });
            //    var tempobj = new object();
            //    if (method.ContainsGenericParameters)
            //    {
            //        var m2 = method.MakeGenericMethod(new Type[] { insrtobjType });

            //        tempobj = m2.Invoke(db, new object[] { _args });
            //    }
            //    else
            //    {
            //        tempobj = method.Invoke(db, _args);
            //    }
            //    var method2 = tempobj.GetType().GetMethod("ExecuteCommand");
            //    var reobj = method2.Invoke(tempobj, null);
            //    return reobj;
            //    //typeof(SqlSugarClient).GetMethod("Insertable").ContainsGenericParameters
            //    // return db.Insertable<T>(_args).ExecuteCommand();
            //});
            //dataGrounders.Orms["sqlsugar"].ConfigUpdateAction((_args) =>
            // {
            //     var dGrounder = dataGrounders.Orms["sqlsugar"].DataGrounder;
            //     var db = dGrounder.OrmObject as SqlSugarClient;
            //     return db.Updateable(_args).ExecuteCommand();

            // });

            //dataGrounders.Orms["sqlsugar"].ConfigQueryAction((queryString, objectparameter, strartPageIndex, pageSize) =>
            //{
            //    var dGrounder = dataGrounders.Orms["sqlsugar"].DataGrounder;
            //    var db = dGrounder.OrmObject as SqlSugarClient;
            //    var total = 0;
            //    var data = db.SqlQueryable<dynamic>(queryString).AddParameters(objectparameter).ToPageList(strartPageIndex, pageSize, ref total);
            //    return new QueryResult { currentIndex = strartPageIndex + 1, DataList = data, PageSize = pageSize, TotalPage = total };

            //});

        }
    }
}
