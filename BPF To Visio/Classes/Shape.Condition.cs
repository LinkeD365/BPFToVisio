using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinkeD365.BPFToVisio
{
    public class Condition : BotShape
    {
        public StringBuilder Expression { get; set; }
        public Condition(JObject stageObject, BotShape parent, int noChildren = 1, int currentChild = 1) : base(stageObject, parent, noChildren, currentChild)
        {
            Shape = new XElement(GetTemplateShape("Condition"));

            //AddName(BotShapeObject.SelectToken("steps.list[0].description"));
            AddType("Condition");

            CalcPosition();
            AddLine();

            // if (Utils.Shapes.Any<BotShape>(bs => bs.StageId == ))
        }

        internal Condition AddCondition(JObject conditionStep, int noConditions, int currentCondition)
        {
            if (Expression == null)
            {// No expression yet, so first, just add the text
                Expression = new StringBuilder();
                string binaryOp = GetBinaryOp(conditionStep.SelectToken("$.conditionExpression.conditionOperatoroperator").ToString());
                AddEntityExpression((JObject)conditionStep.SelectToken("$.conditionExpression"), binaryOp);

                var indexOf = Expression.ToString().LastIndexOf(binaryOp);
                if (indexOf != -1)
                {
                    AddText(Expression.ToString().Substring(0, indexOf));
                }

                AddName(conditionStep.SelectToken("description"));
                return this;
            }
            var newCondition = new Condition(conditionStep, this, 2, 2);
            newCondition.AddCondition(conditionStep, noConditions, currentCondition);
            return newCondition;
        }

        private void AddEntityExpression(JObject expressionObject, string condition)
        {
            if (expressionObject["conditionOperatoroperator"].ToString() == "0" || expressionObject["conditionOperatoroperator"].ToString() == "1")
            {
                Expression.AppendLine(expressionObject.SelectToken("operand.entity.entityName").ToString() + "." + expressionObject.SelectToken("operand.attributeName").ToString() + GetOperator(expressionObject["conditionOperatoroperator"].ToString()));
            }
            else
            {
                JObject left = (JObject)expressionObject.SelectToken("$.left");

                if (left["__class"].ToString() == "BinaryExpression:#Microsoft.Crm.Workflow.Expressions")
                {
                    AddEntityExpression(left, condition);
                }
                var rights = ((JArray)expressionObject["right"]).ToList();
                JObject right = (JObject)expressionObject["right"][0];
                switch (right["__class"].ToString())
                {
                    case "PrimitiveExpression:#Microsoft.Crm.Workflow.Expressions":
                        Expression.Append(left["entity"]["entityName"].ToString() + "." + left["attributeName"].ToString());
                        Expression.Append(GetOperator(expressionObject["conditionOperatoroperator"].ToString()));
                        Expression.AppendLine("'" + String.Join("', '", rights.Select(splitRight => splitRight["primitiveValue"].ToString())) + "'");// rights.Join( right => right["primitiveValue"]. right["primitiveValue"].ToString() + "'");
                        break;
                    case "BinaryExpression:#Microsoft.Crm.Workflow.Expressions":
                        AddEntityExpression(right, condition);
                        return;
                    case "EntityAttributeExpression:#Microsoft.Crm.Workflow.Expressions":
                        Expression.Append(left["entity"]["entityName"].ToString() + "." + left["attributeName"].ToString());
                        Expression.Append(GetOperator(expressionObject["conditionOperatoroperator"].ToString()));
                        Expression.AppendLine(right["entity"]["entityName"].ToString() + "." + right["attributeName"].ToString());
                        break;
                    case "MethodCallExpression:#Microsoft.Crm.Workflow.Expressions":
                        Expression.Append(left["entity"]["entityName"].ToString() + "." + left["attributeName"].ToString());
                        Expression.Append(GetOperator(expressionObject["conditionOperatoroperator"].ToString()));
                        Expression.Append(right.SelectToken("parameters[1].parameters[0].entity.entityName").ToString() + "." + right.SelectToken("parameters[1].parameters[0].attributeName").ToString());
                        Expression.Append(GetFormulaOperator(right.SelectToken("parameters[0]").ToString()));
                        if (right.SelectToken("parameters[2].parameters[0].__class").ToString() == "EntityAttributeExpression:#Microsoft.Crm.Workflow.Expressions")
                            Expression.AppendLine(right.SelectToken("parameters[2].parameters[0].entity.entityName").ToString() + "." + right.SelectToken("parameters[2].parameters[0].attributeName").ToString());
                        else Expression.Append(right.SelectToken("parameters[2].parameters[0].primitiveValue"));
                        break;
                    default:
                        return;
                }
            }
            Expression.AppendLine(condition);
        }

        private string GetFormulaOperator(string opString)
        {
            switch (opString)
            {
                case "0":
                    return " + ";
                case "1":
                    return " - ";
                case "2":
                    return " * ";
                case "3":
                    return " / ";
                default:
                    return String.Empty;
            }
        }

        private string GetBinaryOp(string condition)
        {

            switch (condition)
            {
                case "3":
                    return "OR";
                case "2":
                    return "AND";
                default:
                    return string.Empty;
            }
        }

        private string GetOperator(string opString)
        {
            switch (opString)
            {
                case "0":
                    return " does not contain data";
                case "1":
                    return " contains data";
                case "3":
                    return "+";
                case "6":
                    return " = ";
                case "7":
                    return " != ";
                case "8":
                    return " contains ";
                case "9":
                    return " does not contain ";
                case "10":
                    return " begins with ";
                case "11":
                    return " does not begin with ";
                case "12":
                    return " ends with ";
                case "13":
                    return " does not end with ";
                default:
                    return string.Empty;
            }
        }
    }
}
