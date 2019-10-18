using System;
using System.Collections.Generic;
using System.Text;

namespace CodeGenergator.Model
{
    public class CodeGeneratorFactory
    {
        public string ReferenceFormat = @"using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions; 
using UniOrm; 
using UniOrm.Loggers;
{0}";
        public string NameSpaceFormat = @"{0}_Services";

        public string ClassFormat = @" public class {0} : I{1}Service
    {
        public const string LoggerName = ""{2}"";
        IDbFactory dbFactory;
        IDataGrounder Db;
        IConfig Config;
        public bool IsOpenSessionEveryTime { get; set; }
        public OrmName ormname;
        public void OpenSession()
        {
            if (Db == null)
            {
                Db = dbFactory.Orms[ormname].CreateDefaultInstance();
            }
        }
{3}
}";
        public string AddMothodFormat = @"
        public int Insert{0}<T>(T objcode) where T : class, new()
        {
            OpenSession(); 
            return Convert.ToInt32(Db.Insert<T>(objcode));
          
        }";
        public string UpdateMothodFormat = @"
        public int Update{0}(object obj)
        {
            OpenSession();
            return Db.Update(obj);
        }";

        public string DeleteMothodFormat = @"
         public bool Delete{0}(object obj)
        {
            OpenSession();
            var reint = Db.delete(obj);
            return reint > 0;
        }";

        public string DeleteByIdMothodFormat = @"
         public bool Delete{0}(int id)
        {
            OpenSession();
            var reint = Db.delete(obj);
            return reint > 0;
        }";
    }
}
