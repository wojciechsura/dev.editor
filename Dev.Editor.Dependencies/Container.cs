using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Dependencies
{
    public static class Container
    {
        private static IContainer instance;

        public static void Build(Action<ContainerBuilder> buildActions)
        {
            if (instance != null)
                throw new InvalidOperationException("Container is already built!");

            var builder = new ContainerBuilder();
            builder.RegisterSource(new Autofac.Features.ResolveAnything.AnyConcreteTypeNotAlreadyRegisteredSource());
            buildActions(builder);

            instance = builder.Build();
        }
        

        public static IContainer Instance => instance;       
    }
}
