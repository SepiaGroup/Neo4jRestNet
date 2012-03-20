using System;
using System.Text;
using System.Linq.Expressions;
using Neo4jRestNet.ExpressionTreeParser;

namespace Neo4jRestNet.CypherPlugin
{
	public class ParseWhereLambda
	{
		readonly ExpressionParser _ep = new ExpressionParser();

		public string Parse(Expression expression)
		{
			var leftParens = false;
			var rightParens = false;

			var sbFilter = new StringBuilder();
			var body = expression.NodeType == ExpressionType.Lambda ? ((LambdaExpression)expression).Body : expression;
			BinaryExpression be;

			switch (body.NodeType)
			{
				case ExpressionType.Constant:
					sbFilter.Append(body.ToString() == "null"
					                	? "null"
					                	: Expression.Lambda(_ep.ParseExpression(body)).Compile().DynamicInvoke().ToString());
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
					sbFilter.AppendFormat(body.Type == typeof (string) ? "'{0}'" : "{0}", Expression.Lambda(body).Compile().DynamicInvoke());
					break;

				case ExpressionType.Convert:
					var convert = (UnaryExpression)body;
					
					sbFilter.Append( convert.Operand.Type == typeof(DateTime) ? 
										string.Format("'{0:s}'", Expression.Lambda(body).Compile().DynamicInvoke()) : // Format DateTime to ISO8601 
										Parse(convert.Operand) );
					break;

				case ExpressionType.Not:
					var ue = (UnaryExpression)body;
		
					var statement = Parse(ue.Operand);

					sbFilter.AppendFormat(statement.StartsWith("(") ? "not{0}" : "not({0})", statement);

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
		
					if (be.Left.NodeType == ExpressionType.OrElse)
					{
						// Check to see if NullOp 
						if (!IsNullOp(((BinaryExpression)be.Left).Left))
						{
							leftParens = true;
						}
					}

					if (be.Right.NodeType == ExpressionType.OrElse)
					{
						rightParens = true;
					}

					sbFilter.Append(InvokeExpression(be.Left, "and", be.Right, leftParens, rightParens));
					break;

				case ExpressionType.OrElse:
					be = (BinaryExpression)body;
		
					if (be.Left.NodeType == ExpressionType.AndAlso)
					{
						// Check to see if NullOp 
						if (!IsNullOp(((BinaryExpression)be.Left).Left))
						{
							leftParens = true;
						}
					}

					if (be.Right.NodeType == ExpressionType.AndAlso)
					{
						rightParens = true;
					}

					sbFilter.Append(InvokeExpression(be.Left, "or", be.Right, leftParens, rightParens));
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

		private string InvokeExpression(Expression leftExpression, string strOperator, Expression rightExpression, bool leftParens = false, bool rightParens = false)
		{
			var sb = new StringBuilder();

			if (leftParens)
			{
				sb.AppendFormat("({0})", Parse(leftExpression));
			}
			else
			{
				sb.Append(Parse(leftExpression));
			}

			sb.AppendFormat(" {0} ", strOperator);

			if (rightParens)
			{
				sb.AppendFormat("({0})", Parse(rightExpression));
			}
			else
			{
				sb.Append(Parse(rightExpression));
			}

			return sb.ToString();
		}

		private bool IsNullOp(Expression expression)
		{
			if(expression.NodeType != ExpressionType.Convert)
			{
				return false;
			}

			var ue = (UnaryExpression)expression;

			if(ue.Operand.NodeType != ExpressionType.Call)
			{
				return false;
			}

			var val = Expression.Lambda(_ep.ParseExpression(ue.Operand)).Compile().DynamicInvoke().ToString();
			
			return string.IsNullOrWhiteSpace(val);
		}
	}
}
