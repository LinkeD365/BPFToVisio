
using System.ComponentModel;

namespace LinkeD365.BPFToVisio
{
    internal class WorkFlow
    {
        [Browsable(false)]
        public string Id { get; set; }
        public string Name { get; set; }
        [Browsable(false)]
        public string Schema { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                WorkFlow p = (WorkFlow)obj;
                return (Id == p.Id);
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
