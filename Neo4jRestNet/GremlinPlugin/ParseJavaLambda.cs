using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Neo4jRestNet.GremlinPlugin
{
	public class ParseJavaLambda
	{
		public string Parse(Expression expression)
		{
			StringBuilder sbFilter = new StringBuilder();
			Expression body = expression.NodeType == ExpressionType.Lambda ? ((LambdaExpression)expression).Body : expression;
			BinaryExpression be;

			switch (body.NodeType)
			{
				case ExpressionType.Constant:
					sbFilter.Append(Expression.Lambda(ParseExpression(body)).Compile().DynamicInvoke().ToString());
					break;

				case ExpressionType.Call:
					if (body.Type == typeof(string)) // Quote String values ie. Enums are strings
					{
						sbFilter.AppendFormat("'{0}'", Expression.Lambda(body).Compile().DynamicInvoke().ToString());
					}
					else
					{
						sbFilter.Append(Expression.Lambda(ParseExpression(body)).Compile().DynamicInvoke().ToString());
					}

					break;

				case ExpressionType.MemberAccess:
					if (body.Type == typeof(string)) // Quote String values ie. Enums are strings
					{
						sbFilter.AppendFormat("'{0}'", Expression.Lambda(body).Compile().DynamicInvoke().ToString());
					}
					else
					{
						sbFilter.AppendFormat("{0}", Expression.Lambda(body).Compile().DynamicInvoke().ToString());
					}
					break; 

				case ExpressionType.Convert:
					UnaryExpression convert = (UnaryExpression)body;
					sbFilter.Append(Parse(convert.Operand));
					break;

				case ExpressionType.Not:
					UnaryExpression ue = (UnaryExpression)body;
					sbFilter.Append("!");
					sbFilter.Append(Parse(ue.Operand));
					break;

				case ExpressionType.Equal:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, "==", be.Right));
					break;

				case ExpressionType.NotEqual:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, "!=", be.Right));
					break;

				case ExpressionType.AndAlso:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, "&&", be.Right));
					break;

				case ExpressionType.OrElse:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, "||", be.Right));
					break;

				default:
					throw new Exception(string.Format("NodeType '{0}' not supported", body.NodeType));

			}

			return sbFilter.ToString();
		}

		private string InvokeExpression(Expression LeftExpression, string Operator, Expression RightExpression)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(Parse(LeftExpression));
			sb.AppendFormat(" {0} ", Operator);
			sb.Append(Parse(RightExpression));

			return sb.ToString();
		}

		private Expression ParseExpression(Expression expression)
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
						// Instance Method Call
						AssignExpressions.Add(Expression.Call(mce.Method, ParameterExpressions));
					}
					else
					{
						// Static Method Call
						AssignExpressions.Add(Expression.Call(mce.Object, mce.Method, ParameterExpressions));
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
