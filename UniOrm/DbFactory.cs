using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using SQLBuiler;

namespace UniOrm
{
    public class DbFactory : IDbFactory
    {

        public DataGrouderBridge this[OrmName OrmName]
        {
            get
            {
                return Orms[OrmName];
            }
            set
            {
                Orms[OrmName] = value;
            }
        }

        public DataGrouderBridge this[string ormName]
        {
            get
            {
                var ormtype = (OrmName)Enum.Parse(typeof(OrmName), ormName);
                return Orms[ormtype];
            }
            set
            {
                var ormtype = (OrmName)Enum.Parse(typeof(OrmName), ormName);
                Orms[ormtype] = value;
            }
        }
        public Dictionary<OrmName, DataGrouderBridge> Orms { get; set; }

        public Dictionary<string, DataGrouderBridge> Others { get; set; }
        public DbFactory()
        {
            Orms = new Dictionary<OrmName, DataGrouderBridge>();
        }

        private static IDbFactory _Singleton = null;
        private readonly static object Singleton_Lock = new object(); //锁同步
        public static IDbFactory  Inst()
        {
            lock (Singleton_Lock)
            { 
                if (_Singleton == null)
                { 
                    _Singleton = new DbFactory();
                }
            }
            return _Singleton;
        }

        public void AddOrmDataGrounder(OrmName aliName, params Assembly[] ormAssemlbies)
        {
            if (!Orms.ContainsKey(aliName))
            {
                var dataGrounder = new DataGrouderBridge();
                dataGrounder.OrmAssemlbies = ormAssemlbies.ToList();
                Orms.Add(aliName, dataGrounder);
            }
        }
        public void AddOrmDataGrounder(OrmName aliName, List<Assembly> ormAssemlbies)
        {
            if (!Orms.ContainsKey(aliName))
            {
                var dataGrounder = new DataGrouderBridge();
                dataGrounder.OrmAssemlbies = ormAssemlbies;
                Orms.Add(aliName, dataGrounder);
            }
        }



        public object OrmObject { get; set; }
        public void SetType(object ormObject, Action<Type> mappingAction)
        {
            OrmObject = ormObject;
        }
     


    }
}
