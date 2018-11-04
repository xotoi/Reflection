using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MyIoC.Attributes;
using MyIoC.Exceptions;

namespace MyIoC
{
    public class Container
    {
        private readonly IDictionary<Type, Type> _typesDictionary;
        private readonly ICustomActivator _activator;

        public Container(ICustomActivator activator)
        {
            _activator = activator;
            _typesDictionary = new Dictionary<Type, Type>();
        }

        public void AddAssembly(Assembly assembly)
        {
            var types = assembly.ExportedTypes;
            foreach (var type in types)
            {
                var constructorImportAttribute = type.GetCustomAttribute<ImportConstructorAttribute>();
                if (constructorImportAttribute != null || HasImportProperties(type))
                {
                    _typesDictionary.Add(type, type);
                }

                var exportAttributes = type.GetCustomAttributes<ExportAttribute>();
                foreach (var exportAttribute in exportAttributes)
                {
                    _typesDictionary.Add(exportAttribute.Contract ?? type, type);
                }
            }
        }

        public void AddType(Type type)
        {
            _typesDictionary.Add(type, type);
        }

        public void AddType(Type type, Type baseType)
        {
            _typesDictionary.Add(baseType, type);
        }

        public object CreateInstance(Type type)
        {
            return ConstructInstanceOfType(type);
        }

        public T CreateInstance<T>()
        {
            return (T)ConstructInstanceOfType(typeof(T));
        }

        private object ConstructInstanceOfType(Type type)
        {
            if (!_typesDictionary.ContainsKey(type))
            {
                throw new DependencyException($"Cannot create instance of {type.Name}. Dependency is not provided");
            }

            var dependentType = _typesDictionary[type];
            var constructorInfo = GetConstructor(dependentType);
            var instance = CreateFromConstructor(dependentType, constructorInfo);

            if (dependentType.GetCustomAttribute<ImportConstructorAttribute>() != null)
            {
                return instance;
            }

            ResolveProperties(dependentType, instance);
            return instance;
        }

        private static bool HasImportProperties(Type type)
        {
            var propertiesInfo = GetPropertiesRequiredImport(type);
            return propertiesInfo.Any();
        }

        private static IEnumerable<PropertyInfo> GetPropertiesRequiredImport(Type type)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute<ImportAttribute>() != null);
        }

        private void ResolveProperties(Type type, object instance)
        {
            var propertiesInfo = GetPropertiesRequiredImport(type);
            foreach (var property in propertiesInfo)
            {
                var resolvedProperty = ConstructInstanceOfType(property.PropertyType);
                property.SetValue(instance, resolvedProperty);
            }
        }

        private static ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors();

            if (constructors.Length == 0)
            {
                throw new DependencyException($"There are no public constructors for type {type.FullName}");
            }

            return constructors.First();
        }

        private object CreateFromConstructor(Type type, ConstructorInfo constructorInfo)
        {
            var parameters = constructorInfo.GetParameters();
            var parametersInstances = new List<object>(parameters.Length);
            Array.ForEach(parameters, p => parametersInstances.Add(ConstructInstanceOfType(p.ParameterType)));

            return _activator.CreateInstance(type, parametersInstances.ToArray());
        }
    }
}
