using System;
using System.Linq;
using System.Reflection.Emit;

namespace MyIoC
{
    public class EmitActivator : ICustomActivator
    {
        public object CreateInstance(Type type, params object[] parameters)
        {
            var parametersTypes = parameters.Select(p => p.GetType()).ToArray();
            var createMethod = new DynamicMethod(string.Empty, type, parametersTypes);
            var il = createMethod.GetILGenerator();
            for (var i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldarg, i);
            }

            var ctor = type.GetConstructor(parametersTypes);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);

            return createMethod.Invoke(null, parameters);
        }
    }
}
