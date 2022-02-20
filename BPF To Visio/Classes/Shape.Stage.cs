using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinkeD365.BPFToVisio
{
    public class Stage : BotShape
    {
        // p
        public Stage(JObject stageObject, BotShape parent, int noChildren = 1, int currentChild = 1) : base(stageObject, parent, noChildren, currentChild)
        {
            Shape = new XElement(GetTemplateShape("Stage"));
            Utils.Shapes.Add(this);
            AddProp("ActionType", BotShapeObject.SelectToken("$.description").ToString());
            AddName(BotShapeObject.SelectToken("$.steps.list[0].description").ToString());
            //	string triggers = TriggerObject["triggerQueries"] == null ? string.Empty : string.Join(Environment.NewLine, ((JArray)TriggerObject["triggerQueries"]).Select(trig => trig.ToString()));
            //AddText(triggers);
            AddText();


            if (ParentShape == null)
            {
                PinX = double.TryParse(Shape.Elements().First(el => el.Attribute("N").Value == "PinX").Attribute("V").Value, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var tempPinX) ? tempPinX : 0.0;
                PinY = double.TryParse(Shape.Elements().First(el => el.Attribute("N").Value == "PinY").Attribute("V").Value, NumberStyles.Any,
                    CultureInfo.InvariantCulture, out var tempPiny) ? tempPiny : 0.0;
            }
            else
            {
                CalcPosition();
                AddLine();
            }
        }

        public Stage(StageNode stageNode, BotShape parent, int noChildren = 1, int currentChild = 1) : this(stageNode.Object, parent, noChildren, currentChild)
        {

        }

        protected void AddText()
        {
            StringBuilder sb = new StringBuilder();
            if (BotShapeObject.SelectTokens("steps.list[0].steps.list[?(@.__class=='StepStep:#Microsoft.Crm.Workflow.ObjectModel')]").Any())
            {
                var selectTokens = BotShapeObject.SelectTokens("steps.list[0].steps.list[?(@.__class=='StepStep:#Microsoft.Crm.Workflow.ObjectModel')]").ToList();
                sb.AppendLine("Step" + (selectTokens.Count() > 1 ? "s(" + selectTokens.Count() + ")" : ""));
                foreach (JObject stepObj in selectTokens)
                {
                    switch (stepObj.SelectToken("steps.list[0].__class").ToString())
                    {
                        case "ControlStep:#Microsoft.Crm.Workflow.ObjectModel":
                            sb.AppendLine(stepObj.SelectToken("$.stepLabels.list[0].description").ToString() +
                                       " | " + stepObj.SelectToken("$.steps.list[0].controlDisplayName").ToString() +
                                       " (" + stepObj.SelectToken("$.steps.list[0].controlId").ToString() + ")" +
                                       (stepObj.SelectToken("$.isProcessRequired").ToString() == "true" ? " Reqd" : ""));
                            break;
                        case "ActionStep:#Microsoft.Crm.Workflow.ObjectModel":

                            sb.AppendLine("Action: " + stepObj.SelectToken("$.stepLabels.list[0].description").ToString());
                            var attTokens = stepObj.SelectTokens("steps.list[0].steps.list[?(@.__class=='AssignVariableStep:#Microsoft.Crm.Workflow.ObjectModel')]").ToList();
                            foreach (JObject attObject in attTokens.Cast<JObject>())
                            {
                                sb.AppendLine(attObject["variableName"].ToString() + " = " + attObject.SelectToken("valueExpression.entity.entityName").ToString() + "." + attObject.SelectToken("valueExpression.attributeName").ToString());
                            }
                            break;
                        case "FlowStep:#Microsoft.Crm.Workflow.ObjectModel":
                            sb.AppendLine("Flow Step: " + stepObj.SelectToken("$.stepLabels.list[0].description").ToString() + " | " + stepObj["description"].ToString());

                            break;
                    }


                }
            }

            if (BotShapeObject.SelectTokens("steps.list[0].steps.list[?(@.__class=='ActionStep:#Microsoft.Crm.Workflow.ObjectModel')]").Any())
            {
                var selectTokens = BotShapeObject.SelectTokens("steps.list[0].steps.list[?(@.__class=='ActionStep:#Microsoft.Crm.Workflow.ObjectModel')]").ToList();

                sb.AppendLine("Triggered Process" + (selectTokens.Count() > 1 ? "es(" + selectTokens.Count() + ")" : ""));
                foreach (JObject stepObj in selectTokens.Cast<JObject>())
                {
                    sb.AppendLine(stepObj["uniqueName"].ToString() + " (" + stepObj.SelectToken("triggerEvents[0].eventName").ToString() + ")");
                }
            }

            AddText(sb.ToString());

        }


    }
}
