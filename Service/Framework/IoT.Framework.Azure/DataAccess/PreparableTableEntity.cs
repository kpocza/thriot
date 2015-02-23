using System;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace IoT.Framework.Azure.DataAccess
{
    public abstract class PreparableTableEntity : TableEntity
    {
        public virtual void PrepareBeforeSave()
        {
        }

        public virtual void PrepareAfterLoad()
        {
        }

        protected string BuildJsonFromByteArrays(params Func<byte[]>[] arrayFuncs)
        {
            int size = arrayFuncs.Where(f => f() != null).Sum(f => f().Length);

            var targetArray = new byte[size];

            int top = 0;
            foreach (var f in arrayFuncs)
            {
                var array = f();
                if (array != null)
                {
                    Array.Copy(array, 0, targetArray, top, array.Length);
                    top += array.Length;
                }
            }

            return Encoding.UTF8.GetString(targetArray);
        }

        protected void BuildByteArraysFromJson(string json, params Action<byte[]>[] arrayActions)
        {
            const int arrayLength = 65000;

            var targetArray = Encoding.UTF8.GetBytes(json);

            int idx = 0;
            for (int i = 0; i < targetArray.Length; i += arrayLength)
            {
                int remaining = (targetArray.Length - i) < arrayLength ? targetArray.Length - i : arrayLength;

                var part = new byte[remaining];
                Array.Copy(targetArray, i, part, 0, remaining);
                arrayActions[idx](part);
                idx++;
            }
        }
    }
}
