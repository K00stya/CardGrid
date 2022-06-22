using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGrid
{
    public static class IoCContainer
    {
        private static Dictionary<Type, object> _services = new Dictionary<Type, object>();
    
        public static void Register<TImplementation>() where TImplementation : class
        {
            Register<TImplementation, TImplementation>();
        }
        
        public static void Register<TInterface, TImplementation>() where TImplementation : class, TInterface
        {
            try
            {
                RegisterInstance<TInterface>(Activator.CreateInstance<TImplementation>());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        
        public static void RegisterInstance<TInterface>(TInterface implementation)
        {
            _services[typeof(TInterface)] = implementation;
        }
        
        public static T Get<T>()
        {
            if(TryGet(typeof(T), out object instance))
            {
                return (T)instance;
            }
        
            Debug.LogError($"IoC Container haven't instance for {typeof(T).ToString()}");
            return default;
        }
    
        private static bool TryGet(Type objectType, out object instance)
        {
            if (!_services.TryGetValue(objectType, out instance))
                return false;

            return true;
        }
    }
}