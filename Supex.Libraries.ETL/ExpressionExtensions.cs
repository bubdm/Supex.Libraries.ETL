using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Supex.Libraries.ETL
{
    public static class ExpressionExtensions
    {
        public static string GetMemberName<TType>(this Expression<Func<TType, object>> expression)
        {
            if (!(expression.Body is MemberExpression body))
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return body.Member.Name;
        }
    }
}
