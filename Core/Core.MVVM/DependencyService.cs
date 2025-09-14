using System;
using System.Collections.Generic;

namespace Sysne.Core.OS
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
        /// <typeparam name="C"></typeparam>
        /// <param name="instance"></param>
        public static void Register<C>(C? instance = default, Func<object>? implementationFactory = null) where C : class, new()
        {
            Register<C, C>(ServiceLifetime.Singleton, implementationFactory, instance);
        }

        /// <summary>
        /// Get instance of dependency
        /// </summary>
        /// <typeparam name="I">Type of dependency</typeparam>
        /// <returns>Instance of Type dependency</returns>
        public static I Get<I>()
        {
            if (Dependencies.TryGetValue(typeof(I), out var dep))
            {
                I GetInstance()
                {
                    if (dep.Dependency.ImplementationFactory != null)
                        return (I)dep.Dependency.ImplementationFactory();
                    else
                        return (I)Activator.CreateInstance(dep.Type)!;
                }
                switch (dep.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                    case ServiceLifetime.Scoped:
                        if (dep.Dependency.Instance == null)
                        {
                            dep.Dependency.Instance = GetInstance();
                            Dependencies[typeof(I)] = dep;//Tupplas are not refrerenced objects, so we need set new value to dictionary
                        }
                        return (I)dep.Dependency.Instance;
                    case ServiceLifetime.Transient:
                        return GetInstance();
                    default: return default!;
                }
            }
            else
                throw new KeyNotFoundException($"Unregistered {typeof(I).Name} type");
        }

        /// <summary>
        /// Release resources of dependency
        /// </summary>
        /// <typeparam name="I">Dependency type</typeparam>
        public static void Release<I>()
        {
            if (Dependencies.TryGetValue(typeof(I), out var dep))
            {
                if (dep.Dependency.Instance != null)
                {
                    (dep.Dependency.Instance as IDisposable)?.Dispose();
                    dep.Dependency.Instance = null;
                }
                dep.Dependency.ImplementationFactory = null;
                Dependencies.Remove(typeof(I));
            }
            else
                throw new KeyNotFoundException($"Unregistered {typeof(I)} type");
        }
    }

}
