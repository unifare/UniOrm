using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UniOrm.Common; 

namespace UniOrm.Adaption
{
    public abstract class OrmAdaptorBase
    {
        public Func<object[], object> SelfGetCeateAction { get; set; }
        public DcConnectionConfig ConnectionConfig { get; set; }
        public IResover Resover { get; set; }
        public bool IsSelfDefineCreation { get; set; }
        public List<Assembly> RegistedModelAssemblies { get; set; }
        public RegistedModelTypes RegistedModelTypes { get; }
        public Type DataGrounderType { get; set; }
        public virtual DBType dBType { get; }
        public OrmName OrmName { get; set; }
        public bool IsExcuteImmediately { get; set; }
        public virtual OrmType OrmType { get; }
        public virtual bool GetOpenSessionAction(object dbOperator, params object[] objparameters)
        {
            return true;
        }
        public OrmAdaptorBase()
        {
            RegistedModelTypes = new RegistedModelTypes();
            ConnectionConfig = new DcConnectionConfig();
        }
        public virtual void GetCloseAction(object dbOperator)
        {
            // var db = dbOperator as Database; 
        }
    }
}
