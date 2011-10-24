using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
					else if (expression.Type == typeof(bool))
					{
						// Convert bool values to lowercase strings 
						return Expression.Constant(string.Format("{0}", (bool)((ConstantExpression)expression).Value ? "true" : "false"));
					}

					return (ConstantExpression)expression;

				case ExpressionType.Call:
					MethodCallExpression mce = (MethodCallExpression)expression;

					List<ParameterExpression> ParameterExpressions = new List<ParameterExpression>();
					List<Expression> AssignExpressions = new List<Expression>();
					foreach (Expression argument in mce.Arguments)
					{
						ParameterExpression p = Expression.Parameter(argument.Type);
						ParameterExpressions.Add(p);

						AssignExpressions.Add(GetAssignExpression(p, argument));
					}

					if (mce.Object == null)
					{
						// Static Method Call
						AssignExpressions.Add(Expression.Call(mce.Method, ParameterExpressions));
					}
					else
					{
						// Instance Method Call
						switch (mce.Object.NodeType)
						{
							case ExpressionType.Constant:
								AssignExpressions.Add(Expression.Call(mce.Object, mce.Method, ParameterExpressions));
								break;

							case ExpressionType.Parameter:
								AssignExpressions.Add(Expression.Call(Expression.New(mce.Object.Type), mce.Method, ParameterExpressions));
								break;

							case ExpressionType.Call:

								AssignExpressions.Add(Expression.Call(ParseExpression(mce.Object), mce.Method, ParameterExpressions));
								break;

							default:
								throw new Exception(string.Format("Instance type expression of type '{0}' not supported", expression.Type));
						}
					}

					BlockExpression block = Expression.Block(ParameterExpressions, AssignExpressions);

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
					return Expression.Assign(parameter, (ConstantExpression)expression);

				case ExpressionType.Parameter:
					return Expression.Assign(parameter, Expression.New(expression.Type));

				case ExpressionType.Call:
					Expression e = ParseExpression(expression);
					return Expression.Assign(parameter, e);

				case ExpressionType.Convert:
					return Expression.Assign(parameter, Expression.Convert(expression, expression.Type));

				case ExpressionType.NewArrayInit:
					return Expression.Assign(parameter, Expression.NewArrayInit(expression.Type.GetElementType(), ((NewArrayExpression)expression).Expressions));

				default:
					throw new Exception(string.Format("Expression of type '{0}' not supported", expression.Type));
			}
		}
	}
}
