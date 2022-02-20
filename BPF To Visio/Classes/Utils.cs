using LinkeD365.BPFToVisio.Properties;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace LinkeD365.BPFToVisio
{

    public static partial class Utils
    {
        private const string aiEndpoint = "https://dc.services.visualstudio.com/v2/track";

        private const string aiKey = "cc383234-dfdb-429a-a970-d17847361df3";
        private static AppInsights ai;

        public static AppInsights Ai
        {
            get
            {
                if (ai == null)
                {
                    ai = new AppInsights(aiEndpoint, aiKey, Assembly.GetExecutingAssembly());
                }
                return ai;
            }
        }

        public static List<string> VisioTemplates = new List<string>() { "Condition", "Stage", "Connector" };

        public static int ActionCount { get; set; }

        private static Package templatePackage;

        private static PackagePart templateDocument;

        private static PackagePart templatePages;
        private static PackagePart templatePage;

        public static XDocument XmlPage;
        private static JObject wfObject;
        public static List<BaseShape> Shapes;

        public static List<StageNode> StageNodes;
        private static XElement connects;

        public static XElement Connects
        {
            get
            {
                if (connects == null)
                {
                    IEnumerable<XElement> elements =
                      from element in XmlPage.Descendants()
                      where element.Name.LocalName == "Connects"
                      select element;
                    if (!elements.Any())
                    {
                        IEnumerable<XElement> pageContents =
                      from element in XmlPage.Descendants()
                      where element.Name.LocalName == "PageContents"
                      select element;
                        connects = new XElement("Connects");
                        pageContents.FirstOrDefault().Add(connects);
                    }
                    else
                    {
                        connects = elements.FirstOrDefault();
                    }
                }
                return connects;
            }
        }
        internal static void CreateVisio(WorkFlow workFlow, string fileName, int bpfCount)
        {
            if (templatePackage == null)
            {

                File.WriteAllBytes(fileName, Resources.VisioTemplate_BPF);

                templatePackage = Package.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                templateDocument = GetPackagePart(templatePackage, "http://schemas.microsoft.com/visio/2010/relationships/document");
                templatePages = GetPackagePart(templatePackage, templateDocument, "http://schemas.microsoft.com/visio/2010/relationships/pages");
                templatePage = GetPackagePart(templatePackage, templatePages, "http://schemas.microsoft.com/visio/2010/relationships/page");
            }
            connects = null;
            //_nodes = null;
            //_variables = null;
            //_namedEntities = null;
            //_messages = null;
            //_actions = null;
            XmlPage = GetXMLFromPart(templatePage);

            wfObject = JObject.Parse(workFlow.Schema);
            Shapes = new List<BaseShape>();
            StageNodes = wfObject.SelectTokens("$.steps.list[?(@.__class=='EntityStep:#Microsoft.Crm.Workflow.ObjectModel')]").Select(jt => new StageNode(jt)).ToList();
            var trigger = new Stage(StageNodes.First(), null);
            Shapes.Add(trigger);
            CreateChildStages(trigger);
            //var trigger = new Stage()
            //Shapes.Add(trigger);

            //var rootNode = Nodes.First(node => node.Id == pvaObject["dialogs"].First()["rootNodeId"].ToString());

            //CreateNode(rootNode, trigger, 1, 1);

            RemoveTemplateShapes();

            CreateNewPage(workFlow.Name, bpfCount);
        }

        private static void RemoveTemplateShapes()
        {
            foreach (var shapeName in Utils.VisioTemplates)
                Shapes.First().GetTemplateShape(shapeName).Remove();
        }

        private static void CreateChildStages(BotShape parent)
        {
            if (parent.BotShapeObject.SelectTokens("$..list[?(@.__class == 'ConditionStep:#Microsoft.Crm.Workflow.ObjectModel')]").Any())
            {
                int curChild = 1;
                foreach (JObject condition in parent.BotShapeObject.SelectTokens("$..list[?(@.__class == 'ConditionStep:#Microsoft.Crm.Workflow.ObjectModel')]"))
                {
                    var conditionShape = new Condition(condition, parent, parent.BotShapeObject.SelectTokens("$..list[?(@.__class == 'ConditionStep:#Microsoft.Crm.Workflow.ObjectModel')]").Count(), curChild);
                    Shapes.Add(conditionShape);
                    CreateBranches(conditionShape, condition);

                    CreateChildStages(conditionShape);
                }
            }
            else if (!string.IsNullOrEmpty(parent.BotShapeObject.SelectToken("steps.list[0].nextStageId")?.ToString()))
            {
                var child = Shapes.FirstOrDefault(bs => ((BotShape)bs).StageId == parent.BotShapeObject.SelectToken("steps.list[0].nextStageId").ToString());
                if (child != null)
                {
                    parent.AddLine(child);
                    //child.AddLine(parent);
                }
                else
                {
                    //create shape
                    var stage = new Stage(StageNodes.First(sn => sn.Id == parent.BotShapeObject.SelectToken("steps.list[0].nextStageId").ToString()), parent);
                    CreateChildStages(stage);
                }
            }
        }

        private static void CreateBranches(Condition conditionShape, JObject condition)
        {
            int stepCount = condition.SelectTokens("$.steps.list[?(@.__class=='ConditionBranchStep:#Microsoft.Crm.Workflow.ObjectModel')]").Count();
            var origConditionShape = conditionShape;
            for (int i = 0; i < stepCount; i++)
            {
                JObject conditionStep = (JObject)condition.SelectTokens("$.steps.list[?(@.__class=='ConditionBranchStep:#Microsoft.Crm.Workflow.ObjectModel')]").ToList()[i];
                if (conditionStep.SelectToken("$.conditionExpression") != null)
                {
                    conditionShape = conditionShape.AddCondition(conditionStep, stepCount, i);
                }
                var child = Shapes.FirstOrDefault(bs => ((BotShape)bs).StageId == conditionStep.SelectToken("steps.list[0].stageId").ToString());
                if (child != null)
                {
                    conditionShape.AddLine(child);
                }
                else
                {
                    var stage = i == stepCount - 1 && condition.SelectToken("containsElsebranch").ToString() == "True" ? new Stage(StageNodes.First(sn => sn.Id == conditionStep.SelectToken("steps.list[0].stageId").ToString()), conditionShape, 2, 2)
                      : new Stage(StageNodes.First(sn => sn.Id == conditionStep.SelectToken("steps.list[0].stageId").ToString()), conditionShape);
                    CreateChildStages(stage);
                }

                if (i == stepCount - 1 && condition.SelectToken("containsElsebranch").ToString() == "False")
                {
                    child = Shapes.FirstOrDefault(bs => ((BotShape)bs).StageId == ((BotShape)origConditionShape.ParentShape).BotShapeObject.SelectToken("steps.list[0].nextStageId").ToString());
                    if (child != null) conditionShape.AddLine(child, 2, 2);
                }
            }
        }

        internal static void CompleteVisio(string fileName)
        {
            RemoveTemplate();
            RecalcDocument(templatePackage);

            templatePackage.Close();
            templatePackage = null;
        }


    }
}
