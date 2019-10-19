using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UniOrm.Model;

namespace UniOrm
{
    public class DB
    {

        public static SqlSugarClient Inst
        {
            get
            {
                var configapp = APPCommon.AppConfig.UsingDBConfig;
                var config = new ConnectionConfig()
                {
                    ConnectionString = APPCommon.AppConfig.UsingDBConfig.Connectionstring,

                    IsAutoCloseConnection = true,
                    ConfigureExternalServices = new ConfigureExternalServices()
                    {
                        EntityService = (property, column) =>
                        {

                            var attributes = property.GetCustomAttributes(true);//get all attributes     
                            if (attributes.Any(it => it is KeyAttribute))//根据自定义属性    
                            {
                                column.IsPrimarykey = true;
                            }
                        },
                        EntityNameService = (type, entity) =>
                        {
                            var attributes = type.GetCustomAttributes(true);
                            if (attributes.Any(it => it is TableAttribute))
                            {
                                entity.DbTableName = (attributes.First(it => it is TableAttribute) as TableAttribute).Name;
                            }
                            else
                            {
                                entity.DbTableName = APPCommon.AppConfig.UsingDBConfig.DefaultDbPrefixName + entity.DbTableName;
                            }

                        }
                    }
                };

                switch (configapp.DBType)
                {
                    case 1:
                        config.DbType = DbType.SqlServer;
                        break;
                    case 0:
                        config.DbType = DbType.Sqlite;
                        break;
                    case 2:
                        config.DbType = DbType.MySql;
                        break;
                    case 3:
                        config.DbType = DbType.PostgreSQL;
                        break;
                }


                return new SqlSugarClient(config);
            }
        }

        public static SqlSugarClient New(string connectionstring, int dbtype)
        {

            var config = new ConnectionConfig()
            {
                ConnectionString = connectionstring,
                //DbType= DbType.
            };

            switch (dbtype)
            {
                case 1:
                    config.DbType = DbType.SqlServer;
                    break;
                case 0:
                    config.DbType = DbType.Sqlite;
                    break;
                case 2:
                    config.DbType = DbType.MySql;
                    break;
                case 3:
                    config.DbType = DbType.PostgreSQL;
                    break;
            }

            return new SqlSugarClient(config);

        }
    }

    public class RazorTool
    {
        public HttpContext HttpContext { get; set; }
        public RazorTool()
        {
            IHttpContextAccessor factory = APP.ApplicationServices.GetService<IHttpContextAccessor>();
            HttpContext = factory.HttpContext;
        }

        public AConFlowStep Step { get; set; }


        public List<dynamic> GetData(string sql, object args)
        {
            return DB.Inst.Ado.SqlQuery<dynamic>(sql, args);
        }

        //public dynamic GetData(string sql, object[] args)
        //{
        //    var aConFlowSteps = DB.Inst.Queryable<AConFlowStep>().ToList();
        //}

        public string Session(string key)
        {
            return HttpContext.Session.GetString(key);
        }

        public void Session(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }




        public string UrlQuery(string key)
        {
            return HttpContext.Request.Query[key];
        }

        public string GetForm(string key)
        {
            return HttpContext.Request.Form[key];
        }

        public HttpRequest Request
        {
            get
            {
                return HttpContext.Request;
            }
        }

        public HttpResponse Respone
        {
            get
            {
                return HttpContext.Response;
            }
        }
        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>  
        /// <param name="minutes">过期时长，单位：分钟</param>      
        public void SetCookies(string key, string value, int minutes = 30)
        {
            HttpContext.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }
        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="key">键</param>
        public void DeleteCookies(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        public string GetCookies(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }
    }
}
