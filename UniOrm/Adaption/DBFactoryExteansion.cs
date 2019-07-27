using System;
using System.Collections.Generic;
using UniOrm.Adaption;

namespace UniOrm
{
    public static class DBFactoryExteansion
    {
        public static DataGrouderBridge EFCore(this IDbFactory dbFactory, IEnumerable<Type> types = null)
        {
            if (!dbFactory.Orms.ContainsKey(OrmName.EFCore))
            {
                var sdsf = new EFCoreDataAdaper();

                dbFactory.AddOrm(sdsf);
                return GetAndReturnDBFactory(dbFactory, types);
            }
            else
            {
                if (types != null)
                {
                    dbFactory.Orms[OrmName.EFCore].OrmAdaptor.RegistedModelTypes.AddTypes(types);
                }
                return dbFactory.Orms[OrmName.EFCore];
            }
        }
        public static DataGrouderBridge EFCore<T>(this IDbFactory dbFactory)
        {
            var types = new List<Type>();
            types.Add(typeof(T));
            return EFCore(dbFactory, types);
        }
        public static DataGrouderBridge EFCore<T>(this IDbFactory dbFactory, params Type[] types)
        { 
            return EFCore(dbFactory, types);
        }
        private static DataGrouderBridge GetAndReturnDBFactory(IDbFactory dbFactory, IEnumerable<Type> types)
        {
            if (types != null)
            {
                dbFactory.Orms[OrmName.EFCore].OrmAdaptor.RegistedModelTypes.AddTypes(types);
            }
            return dbFactory.Orms[OrmName.EFCore];
        }

        public static DataGrouderBridge PatePoco(this IDbFactory dbFactory)
        {
            if (dbFactory.Orms[OrmName.PatePoco] == null)
            {
                var sdsf = new PatePocoOrmAdaptor();

                dbFactory.AddOrm(sdsf);
                return dbFactory.Orms[OrmName.PatePoco];
            }
            else
            {
                return dbFactory.Orms[OrmName.PatePoco];
            }
        }
    }
}
