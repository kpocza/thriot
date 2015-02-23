using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace IoT.Framework
{
    public class SingleContainer
    {
        private readonly UnityContainer _container;

        private SingleContainer()
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
        }

        public static readonly SingleContainer Instance = new SingleContainer();

        public IUnityContainer Container
        {
            get { return _container; }
        }

        public object Resolve(Type t)
        {
            return _container.Resolve(t);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
    }
}
