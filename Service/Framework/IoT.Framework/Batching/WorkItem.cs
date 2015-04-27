using System.Threading;

namespace Thriot.Framework.Batching
{
    public class WorkItem<TParameter, TResult>
    {
        public ManualResetEvent Event { get; private set; }

        public TParameter Parameter { get; private set; }

        public TResult Result { get; set; }

        public WorkItem(ManualResetEvent @event, TParameter parameter)
        {
            Event = @event;
            Parameter = parameter;
        }
    }
}
