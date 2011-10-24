using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Neo4jRestNet.ExpressionTreeParser;

namespace Neo4jRestNet.CypherPlugin
{
	public class ParseWhereLambda
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
					if (body.ToString() == "null")
					{
						sbFilter.Append("null");
					}
					else
					{
						sbFilter.Append(Expression.Lambda(ep.ParseExpression(body)).Compile().DynamicInvoke().ToString());
					}
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
		
					string statement = Parse(ue.Operand);

					if (statement.StartsWith("("))
					{
						sbFilter.AppendFormat("not{0}", statement);
					}
					else
					{
						sbFilter.AppendFormat("not({0})", statement);
					}

					break;

				case ExpressionType.Equal:
					be = (BinaryExpression)body;

					if (be.Right.NodeType == ExpressionType.Constant && be.Right.ToString() == "null")
					{
						sbFilter.AppendFormat("{0} is null", Parse(be.Left));
					}
					else if (be.Left.NodeType == ExpressionType.Constant && be.Left.ToString() == "null")
					{
						sbFilter.AppendFormat("{0} is null", Parse(be.Right));
					}
					else
					{
						sbFilter.Append(InvokeExpression(be.Left, "=", be.Right));
					}

					break;

				case ExpressionType.NotEqual:
					be = (BinaryExpression)body;
					if (be.Right.NodeType == ExpressionType.Constant && be.Right.ToString() == "null")
					{
						sbFilter.AppendFormat("{0} is not null", Parse(be.Left)); 
					}
					else if (be.Left.NodeType == ExpressionType.Constant && be.Left.ToString() == "null")
					{
						sbFilter.AppendFormat("{0} is not null", Parse(be.Right));
					}
					else
					{
						sbFilter.Append(InvokeExpression(be.Left, "!=", be.Right));
					}

					break;

				case ExpressionType.AndAlso:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, "and", be.Right));
					break;

				case ExpressionType.OrElse:
					be = (BinaryExpression)body;
					sbFilter.Append(InvokeExpression(be.Left, "or", be.Right));
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
