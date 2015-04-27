using System;

namespace Thriot.Framework.Batching
{
    public class BatchItem<TParameter>
    {
        public Guid Id { get; private set; }

        public TParameter Parameter { get; private set; }

        public BatchItem(Guid id, TParameter parameter)
        {
            Id = id;
            Parameter = parameter;
        }    
    }
}
