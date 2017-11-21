using System;
using System.Linq.Expressions;

namespace Autumn.Mvc.Data.Dapper.Repositories
{
    public class QueryBuilder<TEntity>
    {
        private Expression<Func<TEntity,true>> _where;
        
        public QueryBuilder(Expression<Func<TEntity,true>> where)
        {
            _where = where;
        }
        
        
        
        
    }
}