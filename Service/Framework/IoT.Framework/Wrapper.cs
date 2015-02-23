using System.Collections.Generic;

namespace IoT.Framework
{
    public class Wrapper<T>
        where T : class
    {
        public IList<T> Entities { get; set; }

        public Wrapper()
        {
        }

        public Wrapper(IList<T> entities)
        {
            Entities = entities ?? new List<T>();
        }

        public Wrapper(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                Entities = new List<T>();
                return;
            }

            Entities = Serializers.FromJsonString<Wrapper<T>>(str).Entities ?? new List<T>();
        }

        public string AsString()
        {
            return Serializers.ToJsonString(this);
        }
    }
}