﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoDI
{
    public static class DI
    {
        public const string Namespace = "AutoDI";
        public const string TypeName = "<AutoDI>";
        public const string GlobalServiceProviderName = "_globalServiceProvider";

        public static void Init(Assembly containerAssembly = null, Action<IApplicationBuilder> configureMethod = null)
        {
            Type autoDI = GetAutoDIType(containerAssembly);

            var method = autoDI.GetRuntimeMethod(nameof(Init), new[] { typeof(Action<IApplicationBuilder>) });
            if (method == null) throw new RequiredMethodMissingException($"Could not find {nameof(Init)} method on {autoDI.FullName}");
            method.Invoke(null, new object[] { configureMethod });
        }

        public static void AddServices(IServiceCollection collection, Assembly containerAssembly = null)
        {
            Type autoDI = GetAutoDIType(containerAssembly);

            var method = autoDI.GetRuntimeMethod(nameof(AddServices), new[] { typeof(IServiceCollection) });
            if (method == null) throw new RequiredMethodMissingException($"Could not find {nameof(AddServices)} method on {autoDI.FullName}");
            method.Invoke(null, new object[] { collection });
        }

        public static void Dispose(Assembly containerAssembly = null)
        {
            Type autoDI = GetAutoDIType(containerAssembly);

            var method = autoDI.GetRuntimeMethod(nameof(Dispose), new Type[0]);
            if (method == null) throw new RequiredMethodMissingException($"Could not find {nameof(Dispose)} method on {autoDI.FullName}");
            method.Invoke(null, new object[0]);
        }

        public static IServiceProvider GetGlobalServiceProvider(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            Type autoDI = GetAutoDIType(assembly);
            FieldInfo field = autoDI.GetRuntimeFields().SingleOrDefault(f => f.Name == GlobalServiceProviderName) ??
                        throw new GlobalServiceProviderNotFoundException($"Could not find {GlobalServiceProviderName} field");
            return (IServiceProvider)field.GetValue(null);
        }

        private static Type GetAutoDIType(Assembly containerAssembly)
        {
            const string typeName = Namespace + "." + TypeName;

            Type containerType = containerAssembly != null
                ? containerAssembly.GetType(typeName)
                : Type.GetType(typeName);

            if (containerType == null)
                throw new GeneratedClassMissingException("Could not find AutoDI class. Was the fody weaver run on this assembly?");
            return containerType;
        }

    }
}