using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using UniOrm.Common;
using UniOrm;
using UniOrm.Model;
using UniOrm.Loggers;

namespace UniOrm.Model.DataService
{
    public class CodeService : ICodeService
    {
        public const string LoggerName = "CodeService";
        IDbFactory dbFactory;
        IDataGrounder Db;
        IConfig Config;
        public bool IsOpenSessionEveryTime { get; set; }
        public OrmName ormname;
        public CodeService(IDbFactory dbfactory, IConfig config)
        {
            Config = config;
            IsOpenSessionEveryTime = false;
            dbFactory = dbfactory;

            var ormnamestring = Config.GetValue<string>("App", "UsingDBConfig", "OrmName");
            ormname = (OrmName)Enum.Parse(typeof(OrmName), ormnamestring);
        }

        public void OpenSession()
        {
            if (Db == null)
            {
                Db = dbFactory.Orms[ormname].CreateDefaultInstance();
            }
        }
        public SystemACon GetSystemACon()
        {
            OpenSession();
            //using (var db = dbFactory.Orms[ormname].CreateDefaultInstance())
            //{

            var q = KataDB.From(nameof(SystemACon));
            // var sss = db.ToSql(q);
            return Db.ToTyped<SystemACon>().Query(q).FirstOrDefault();  //$"select top 1 * from  {nameof(SystemACon)}=@FirstNameParam"' <SystemACon>().First();
            //}
        }
        public int InsertCode<T>(T objcode) where T : class, new()
        {
            OpenSession();
            //using (var db = dbFactory.Orms[ormname].CreateDefaultInstance())
            //{ 
            return Convert.ToInt32(Db.Insert<T>(objcode));
            //}
        }
        public TypeDefinition GetTypeDefinition(string typeName)
        {
            OpenSession();
            var q = KataDB.From(nameof(TypeDefinition)).Where("AliName", typeName);

            var typeds = Db.ToTyped<TypeDefinition>().Query(q);
            if (typeds.Count() == 0)
            {
                throw new Exception("TypeDefinition ali name " + typeName + " is shown more than twice. ");
            }
            else if (typeds.Count() > 1)
            {
                throw new Exception("TypeDefinition ali name " + typeName + " is shown more than twice. ");
            }
            else
            {
                return typeds.ToList()[0];
            }
        }

        public AdminUser GetAdminUser(string username, string password)
        {
            OpenSession();
            var typeds = Db.QueryList<AdminUser>(p => p.UserName == username && p.Password == password);
            if (typeds.Count() == 0)
            {
                Logger.LogError(LoggerName,"AdminUser   name " + username + " is not found " , new Exception("AdminUser   name " + username + " is shown more than twice. "));
                return null;
            }
            else if (typeds.Count() > 1)
            {
                Logger.LogError(LoggerName, "AdminUser   name " + username + " is shown more than twice. ", new Exception("AdminUser   name " + username + " is shown more than twice. "));
                return null;
            }
            else
            {
                return typeds.ToList()[0];
            }
        }
        public DefaultUser GetDefaultUser(string username, string password)
        {
            OpenSession();
            var typeds = Db.QueryList<DefaultUser>(p => p.UserName == username && p.Password == password);
            if (typeds.Count() == 0)
            {
                Logger.LogError(LoggerName, "DefaultUser   name " + username + " is not found ", new Exception("DefaultUser   name " + username + " is shown more than twice. "));
                return null;
            }
            else if (typeds.Count() > 1)
            {
                Logger.LogError(LoggerName, "DefaultUser   name " + username + " is shown more than twice. ", new Exception("DefaultUser   name " + username + " is shown more than twice. "));
                return null; 
            }
            else
            {
                return typeds.ToList()[0];
            }
        }
        public List<AConFlowStep> GetAConStateSteps(string stepflowid)
        {
            OpenSession();
            var oneobject = new List<AConFlowStep>(); ;
            var basequery = Db.From(nameof(AConFlowStep));

            if (stepflowid != null)
            {
                basequery.Where("AComposityId", stepflowid);

            }

            oneobject = basequery.ToList<AConFlowStep>();
            if (oneobject == null)
            {
                oneobject = new List<AConFlowStep>();
            }
            return oneobject;
        }

        public List<ComposeEntity> GetConposity(string id, string name = null)
        {
            OpenSession();
            var oneobject = new List<ComposeEntity>();
            var basequery = Db.From(nameof(ComposeEntity));
            if (id != null)
            {
                basequery.Where("Guid", id);

            }
            if (name != null)
            {
                basequery.Where(nameof(ComposeEntity.Name), name);

            }
            oneobject = basequery.ToList<ComposeEntity>();
            if (oneobject == null)
            {
                oneobject = new List<ComposeEntity>();
            }
            return oneobject;
        }
        public List<T> GetSimpleCode<T>(object simplequery) where T : class, new()
        {
            OpenSession();
            var oneobject = new List<T>();
            var basequery = Db.From<T>();
            if (simplequery != null)
            {
                basequery = basequery.Where(simplequery);
            }
            oneobject = basequery.ToList<T>();
            if (oneobject == null)
            {
                oneobject = new List<T>();
            }
            return oneobject;
        }

        public bool DeleteSimpleCode(object simplequery)
        {
            OpenSession();
            var reint = Db.delete(simplequery);
            return reint > 0;
        }
        public List<T> GetSimpleCodeTyped<T>(object simplequery) where T : class, new()
        {
            OpenSession();
            var oneobject = new List<T>();
            var basequery = Db.From<T>();
            if (simplequery != null)
            {
                basequery = basequery.Where(simplequery);
            }
            oneobject = basequery.ToList<T>();
            if (oneobject == null)
            {
                oneobject = new List<T>();
            }
            return oneobject;
        }

        public IEnumerable<T> GetSimpleCodeLinq<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            OpenSession();
            return Db.QueryList<T>(predicate);
        }
        public int UpdateSimpleCode(object obj)
        {
            OpenSession();
            return Db.Update(obj);
        }
        //public List<T> GetSimpleCode<T>(Expression<Func<T, bool>> expression)
        //{
        //    var oneobject = new List<T>();
        //    var basequery = Db.From<T>().Where(expression).ToList<T>();
        //    if (oneobject == null)
        //    {
        //        oneobject = new List<T>();
        //    }
        //    return oneobject;

        //}

        public void Dispose()
        {
            if (Db != null)
            {
                Db.Dispose();
            }
        }
    }

}
