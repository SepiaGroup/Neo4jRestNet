using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Neo4jRestNet.ExpressionTreeParser;

namespace Neo4jRestNet.GremlinPlugin
{
	public class ParseJavaLambda
	{
		ExpressionParser ep = new ExpressionParser();

		public string Parse(Expression expression)
		{
			StringBuilder sbFilter = new StringBuilder();
			Expression body = expression.NodeType == ExpressionType.Lambda ? ((LambdaExpression)expression).Body : expression;
			BinaryExpression be;

			switch (body.NodeType)
			{
				case ExpressionType.Constant:
					sbFilter.Append(Expression.Lambda(ep.ParseExpression(body)).Compile().DynamicInvoke().ToString());
					break;

				case ExpressionType.Call:
					if (body.Type == typeof(string)) // Quote String values ie. Enums are strings
					{
						sbFilter.AppendFormat("'{0}'", Expression.Lambda(body).Compile().DynamicInvoke().ToString());
					}
					else
					{
						sbFilter.Append(Expression.Lambda(ep.ParseExpression(body)).Compile().DynamicInvoke().ToString());
					}

					break;

				case ExpressionType.MemberAccess:
					//if (body.Type == typeof(string)) // Quote String values ie. Enums are strings
					//{
					//    sbFilter.AppendFormat("'{0}'", Expression.Lambda(body).Compile().DynamicInvoke().ToString());
					//}
					//else
					//{
						sbFilter.AppendFormat("{0}", Expression.Lambda(body).Compile().DynamicInvoke().ToString());
					//}
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

				case ExpressionType.LessThan:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, "<", be.Right));
					break;

				case ExpressionType.LessThanOrEqual:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, "<=", be.Right));
					break;

				case ExpressionType.GreaterThan:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, ">", be.Right));
					break;

				case ExpressionType.GreaterThanOrEqual:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, ">=", be.Right));
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
	}
}
