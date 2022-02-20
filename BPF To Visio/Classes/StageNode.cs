using Newtonsoft.Json.Linq;

namespace LinkeD365.BPFToVisio
{
    public class StageNode
    {

        public JObject Object { get; private set; }

        public string Id
        {
            get { return Object.SelectToken("steps.list[0].stageId").ToString(); }
        }

        public StageNode(JToken token)
        {
            Object = (JObject)token;
        }
    }
}
