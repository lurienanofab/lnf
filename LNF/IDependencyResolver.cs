using System;

namespace LNF
{
    public interface IDependencyResolver
    {
        T GetInstance<T>();
        void BuildUp(object target);
        void Register<TPluginType, TConcreteType>() where TConcreteType : TPluginType;
        void Register(Type pluginType, Type concreteType);
        void RegisterSingleton<TPluginType, TConcreteType>() where TConcreteType : TPluginType;
        void RegisterSingleton(Type pluginType, Type concreteType);
    }
}
