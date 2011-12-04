using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Neo4jRestNet.ExpressionTreeParser
{
	public class ExpressionParser
	{
		public Expression ParseExpression(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Constant:
					if (expression.Type == typeof(string))
					{
						// Quote string values
						return Expression.Constant(string.Format("'{0}'", ((ConstantExpression)expression).Value));
					}
					
					if (expression.Type == typeof(bool))
					{
						// Convert bool values to lowercase strings 
						return Expression.Constant(string.Format("{0}", (bool)((ConstantExpression)expression).Value ? "true" : "false"));
					}

					return expression;

				case ExpressionType.Call:
					var mce = (MethodCallExpression)expression;

					var parameterExpressions = new List<ParameterExpression>();
					var assignExpressions = new List<Expression>();
					foreach (Expression argument in mce.Arguments)
					{
						ParameterExpression p = Expression.Parameter(argument.Type);
						parameterExpressions.Add(p);

						assignExpressions.Add(GetAssignExpression(p, argument));
					}

					if (mce.Object == null)
					{
						// Static Method Call
						assignExpressions.Add(Expression.Call(mce.Method, parameterExpressions));
					}
					else
					{
						// Instance Method Call
						switch (mce.Object.NodeType)
						{
							case ExpressionType.Constant:
								assignExpressions.Add(Expression.Call(mce.Object, mce.Method, parameterExpressions));
								break;

							case ExpressionType.Parameter:
								assignExpressions.Add(Expression.Call(Expression.New(mce.Object.Type), mce.Method, parameterExpressions));
								break;

							case ExpressionType.Call:

								assignExpressions.Add(Expression.Call(ParseExpression(mce.Object), mce.Method, parameterExpressions));
								break;

							case ExpressionType.MemberAccess:
								if (mce.Object.Type == typeof(string)) // Quote String values ie. Enums are strings
								{
									assignExpressions.Add(Expression.Constant(String.Format("'{0}'", Expression.Lambda(mce.Object).Compile().DynamicInvoke().ToString())));
								}
								else
								{
									assignExpressions.Add(Expression.Constant(Expression.Lambda(mce.Object).Compile().DynamicInvoke().ToString()));
								}
								break; 

							default:
								throw new Exception(string.Format("Instance type expression of type '{0}' not supported", mce.Object.NodeType));
						}
					}

					BlockExpression block = Expression.Block(parameterExpressions, assignExpressions);

					return block;

				default:
					throw new Exception(string.Format("Expression of type '{0}' not supported", expression.Type));
			}
		}

		private Expression GetAssignExpression(ParameterExpression parameter, Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Constant:
					return Expression.Assign(parameter, expression);

				case ExpressionType.Parameter:
					return Expression.Assign(parameter, Expression.New(expression.Type));

				case ExpressionType.Call:
					Expression e = ParseExpression(expression);
					return Expression.Assign(parameter, e);

				case ExpressionType.Convert:
					return Expression.Assign(parameter, Expression.Convert(expression, expression.Type));

				case ExpressionType.NewArrayInit:
					return Expression.Assign(parameter, Expression.NewArrayInit(expression.Type.GetElementType(), ((NewArrayExpression)expression).Expressions));

				case ExpressionType.MemberAccess:
					var me = (MemberExpression)expression;
					return Expression.Assign(parameter, Expression.MakeMemberAccess(me.Expression, me.Member ));

				default:
					throw new Exception(string.Format("Expression of type '{0}' not supported", expression.Type));
			}
		}
	}
}
