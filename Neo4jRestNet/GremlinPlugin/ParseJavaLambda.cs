using System;
using System.Text;
using System.Linq.Expressions;
using Neo4jRestNet.ExpressionTreeParser;

namespace Neo4jRestNet.GremlinPlugin
{
	public class ParseJavaLambda
	{
		readonly ExpressionParser _ep = new ExpressionParser();

		public string Parse(Expression expression)
		{
			var sbFilter = new StringBuilder();
			Expression body = expression.NodeType == ExpressionType.Lambda ? ((LambdaExpression)expression).Body : expression;
			BinaryExpression be;

			switch (body.NodeType)
			{
				case ExpressionType.Constant:
					sbFilter.Append(Expression.Lambda(_ep.ParseExpression(body)).Compile().DynamicInvoke().ToString());
					break;

				case ExpressionType.Call:
					if (body.Type == typeof(string)) // Quote String values ie. Enums are strings
					{
						sbFilter.AppendFormat("'{0}'", Expression.Lambda(body).Compile().DynamicInvoke());
					}
					else
					{
						sbFilter.Append(Expression.Lambda(_ep.ParseExpression(body)).Compile().DynamicInvoke().ToString());
					}

					break;

				case ExpressionType.MemberAccess:
						sbFilter.AppendFormat("{0}", Expression.Lambda(body).Compile().DynamicInvoke());
					break; 

				case ExpressionType.Convert:
					var convert = (UnaryExpression)body;
					
					sbFilter.Append( convert.Operand.Type == typeof(DateTime) ? 
										string.Format("'{0:s}'", Expression.Lambda(body).Compile().DynamicInvoke()) : // Format DateTime to ISO8601 
										Parse(convert.Operand) );
					break;

				case ExpressionType.Not:
					var ue = (UnaryExpression)body;
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

		private string InvokeExpression(Expression leftExpression, string Operator, Expression rightExpression)
		{
			var sb = new StringBuilder();

			sb.Append(Parse(leftExpression));
			sb.AppendFormat(" {0} ", Operator);
			sb.Append(Parse(rightExpression));

			return sb.ToString();
		}
	}
}
