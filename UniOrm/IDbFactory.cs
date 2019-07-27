using SqlKata;
using System.Collections.Generic;
using System.Reflection;

namespace UniOrm
{
    public interface IDbFactory
    {

        DataGrouderBridge this[OrmName OrmName] { get; set; }

        DataGrouderBridge this[string OrmName] { get; set; }
        Dictionary<OrmName, DataGrouderBridge> Orms { get; set; }
        Dictionary<string, DataGrouderBridge> Others { get; set; }

        void AddOrmDataGrounder(OrmName aliName, params Assembly[] ormAssemlbies);

        void AddOrmDataGrounder(OrmName aliName, List<Assembly> ormAssemlbies);

    }
}