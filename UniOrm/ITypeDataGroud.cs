using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace UniOrm
{
    public interface ITypeDataGroud:IDataGrounder
    {
        IQueryable<TSource> QueryList<TSource >(Expression<Func<TSource, bool>> predicate);
    }
}
