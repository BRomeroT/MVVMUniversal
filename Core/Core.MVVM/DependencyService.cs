using System;
using System.Collections.Generic;

namespace Codeland.Core.OS
{
    /// <summary>
    /// Allow register dependencies and their lifetime
    /// </summary>
    public static class DependencyService
    {
        /// <summary>
        /// Dependency lifetime
        /// </summary>
        public enum ServiceLifetime
        {
            /// <summary>
            /// For internal use only
            /// </summary>
            Default = -1,
            /// <summary>
            /// Creates a single instance of the service. All components requiring a Singleton service receive an instance of the same service.
            /// </summary>
            Singleton = 0,
            /// <summary>
            /// A scoped service registration is scoped to the connection
            /// </summary>
            Scoped = 1,
            /// <summary>
            /// Whenever a component obtains an instance of a Transient service from the service container, it receives a new instance of the service.
            /// </summary>
            Transient = 2,
        }

        /// <summary>
        /// Defualt lifetime for each dependency registered
        /// </summary>
        public static ServiceLifetime DefaultLifetime { get; set; } = ServiceLifetime.Transient;

        /// <summary>
        /// Registered dependency
        /// </summary>
        static Dictionary<Type, (ServiceLifetime Lifetime, Type Type, (Func<object>? ImplementationFactory, object? Instance) Dependency)> Dependencies { get; set; } = new Dictionary<Type, (ServiceLifetime, Type, (Func<object>?, object?))>();

        /// <summary>
        /// Registers a dependency with Lifetime, constructor implementation and/or instance
        /// </summary>
        /// <typeparam name="C">Class that implements dependency</typeparam>
        /// <typeparam name="I">Interface shared</typeparam>
        /// <param name="lifetime">Lifetime for instance</param>
        /// <param name="implementationFactory">Function to get an instance</param>
        /// <param name="instance">Specific instance</param>
        public static void Register<C, I>(ServiceLifetime lifetime = ServiceLifetime.Default, Func<object>? implementationFactory = null, C? instance = default) where C : class, I, new()
        {
            if (lifetime == ServiceLifetime.Default) lifetime = DefaultLifetime;

            void Add((ServiceLifetime Lifetime, Type Type, (Func<object>? ImplementationFactory, object? Instance) Dependency) value)
            {
                value.Lifetime = lifetime;
                value.Type = typeof(C);
                value.Dependency.ImplementationFactory = implementationFactory;
                value.Dependency.Instance = instance;
                Dependencies.Add(typeof(I), value);
            }

            if (!Dependencies.TryGetValue(typeof(I), out var dep))
            {
                Add((lifetime, typeof(C), (implementationFactory, instance)));
            }
            else
            {
                if (Dependencies.ContainsKey(typeof(I)))
                    Dependencies.Remove(typeof(I));
                Add(dep);
            }
        }

        /// <summary>
        /// Registers a singleton dependency
        /// </summary>
        /// <typeparam name="C">Class that implements dependency</typeparam>
        /// <typeparam name="I">Interface shared</typeparam>
        /// <param name="instance">Specific instance</param>
        public static void RegisterSingleton<C, I>(C? instance = default) where C : class, I, new() => Register<C, I>(ServiceLifetime.Singleton, null, instance);

        /// <summary>
        /// Gets an instance of registered dependency 
        /// </summary>
        /// <typeparam name="I">Interface shared</typeparam>
        /// <returns>Instance</returns>
        public static I Get<I>()
        {
            if (Dependencies.TryGetValue(typeof(I), out var dep))
            {
                return dep.Lifetime switch
                {
                    ServiceLifetime.Singleton => GetSingleton<I>(dep),
                    ServiceLifetime.Scoped => GetScoped<I>(dep),
                    ServiceLifetime.Transient => GetTransient<I>(dep),
                    _ => throw new NotImplementedException(),
                };
            }
            else
                throw new InvalidOperationException($"Service {typeof(I).Name} is not registered");
        }

        private static I GetSingleton<I>((ServiceLifetime Lifetime, Type Type, (Func<object>? ImplementationFactory, object? Instance) Dependency) dep)
        {
            if (dep.Dependency.Instance is null)
            {
                dep.Dependency.Instance = dep.Dependency.ImplementationFactory?.Invoke() ?? Activator.CreateInstance(dep.Type);

                Dependencies[typeof(I)] = dep;
            }

            return (I)dep.Dependency.Instance;
        }

        private static I GetScoped<I>((ServiceLifetime Lifetime, Type Type, (Func<object>? ImplementationFactory, object? Instance) Dependency) dep) => GetSingleton<I>(dep);

        private static I GetTransient<I>((ServiceLifetime Lifetime, Type Type, (Func<object>? ImplementationFactory, object? Instance) Dependency) dep) => (I)(dep.Dependency.ImplementationFactory?.Invoke() ?? Activator.CreateInstance(dep.Type));
    }
}
