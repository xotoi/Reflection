using System;

namespace MyIoC
{
    public interface ICustomActivator
    {
        object CreateInstance(Type type, params object[] parameters);
    }
}
