using SQLBuiler;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace UniOrm
{
    public enum OperaTag
    {
        Or,
        And
    }

    public class SE
    {
        public string Operation { get; set; }
        public object Value { get; set; } 
        public static SE<T> Start<T>(Expression<Func<T, object>> expression, string operation, object value)
        {
            SE<T> sE = new SE<T>();
            sE.Operation = operation;
            sE.Value = value;
            sE.Operation = operation;
            return sE;
        }
    }

    public class SE<T>
    {   
       Expression<Func<T, object>> Expression { get; set; }
        public string Operation { get; set; }
        public object Value { get; set; }
        public _NextSE<T,N> SetNext<N>(OperaTag operaTag, Expression<Func<N, object>> expression, string operation, object value)
        {
            var next = new _NextSE<T, N>();
            next.Current = new SE<N>() { Expression = expression, Operation = operation, Value = value }; //  this;
            next.Last = this  ;
            return next;
        }

        //public _NextSE<T, T> SetNextS<T>(Expression<Func<T, object>> expression, string operation, object value)
        //{
        //    var next = new _NextSE<T, T>();
        //    next.Current = this;
        //    next.Next = new SE<T>() { Expression = expression, Operation = operation, Value = value };
        //    return next;
        //}
        //SE<Q> GetLast<Q>()
        //{
        //    var next = new _NextSE<T, N>();
        //    next.Current = this;
        //    next.Next = new SE<N>() { Expression = expression, Operation = operation, Value = value };
        //    return next;
        //}
        //public Query MakeWhere<T>(Expression<Func<T, object>> expression, string operation, object value)
        //{
        //    Operation = operation;
        //    Value = value;
        //    expression
        //}
    }

    public class _LastSE<L, M>
    {
        public OperaTag OperaTag { get; set; }
        public SE<L> LastSE { get; set; }
        public SE<M> Current { get; set; }
    }
    public class _NextSE<M, N>
    {
        public OperaTag OperaTag { get; set; }
        public  SE<M> Last { get; set; }
        public SE<N> Current { get; set; }

        //public _NextSE<T, N> SetNext<N>(OperaTag operaTag, Expression<Func<N, object>> expression, string operation, object value)
        //{
        //    var next = new _NextSE<T, N>();
        //    next.Current = new SE<N>() { Expression = expression, Operation = operation, Value = value }; //  this;
        //    next.Last = this;
        //    return next;
        //}
    }
    public class MeddleSE<L, M, N>
    {
        public _NextSE<M, N> Next { get; set; }
        public SE Current { get; set; }
        public _LastSE<L, M> Last { get; set; }
    }
    public static class QueryWhereEnitityExtense
    {
        //public Query And<T, Q>(this SE<T> q, Expression<Func<Q, object>> expression, string operation, object value)
        //{
        //    q.When(expression)
        //}
        //public static QueryWhereEnitity Make<T>(this Expression<Func<T, object>> expression, string operation, object value)
        //{
        //    return
        //}
    }

    //public static class QueryWhereEnitityExtense
    //{
    //    public Query MakeWhere<T>(this Query q, Expression<Func<T, object>> expression, string operation, object value)
    //    {
    //        q.When(expression)
    //    }
    //    //public static QueryWhereEnitity Make<T>(this Expression<Func<T, object>> expression, string operation, object value)
    //    //{
    //    //    return
    //    //}
    //}
}
