using Newtonsoft.Json.Linq;

namespace LinkeD365.BPFToVisio
{
    public class BotShape : BaseShape
    {
        public JObject BotShapeObject { get; private set; }

        public BotShape(JObject botShapeObject, BotShape parent, int noChildren = 1, int curChild = 1)
        {
            BotShapeObject = botShapeObject;
            ParentShape = parent;
            NoChildren = noChildren;
            CurrentChild = curChild;

            Utils.ActionCount++;
        }

        public string StageId
        {
            get
            {
                if (this is Condition)
                    return string.Empty;
                else return BotShapeObject.SelectToken("steps.list[0].stageId").ToString();
            }
        }
    }
}
