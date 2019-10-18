using System;
using System.Collections.Generic;
using System.Text;
using UniNote.Web.Model;
using UniOrm;

namespace CodeGenergator.Model
{
    public class MainWindowsModel
    {
        IDbFactory dbFactory;
        public MainWindowsModel()
        {
           
            dbFactory = APP.Container.Resolve<IDbFactory>();  

            var ss = dbFactory.EFCore<pigcms_adma>().CreateDefaultInstance();

            var qlist = ss.From<pigcms_adma>();
            var allist = qlist.ToList<pigcms_adma>();
        }
    }
}
