using System;

namespace MyIoC
{
    public class SimpleActivator : ICustomActivator
    {
        public object CreateInstance(Type type, params object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }
    }
}
