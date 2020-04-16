using System;
using System.Reflection;

namespace SqlStreamStore.Demo
{
    public static class MethodInvoker
    {
        private static BindingFlags NonPublic = BindingFlags.Instance | BindingFlags.NonPublic;
        private static BindingFlags Public = BindingFlags.Instance | BindingFlags.Public;

        public static object CallNonPublicIfExists(
            this object instance,
            string methodName,
            object parameter)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, MethodInvoker.NonPublic, (Binder)null, new Type[1]
            {
                parameter.GetType()
            }, (ParameterModifier[])null);
            return !(method == (MethodInfo)null) ? MethodInvoker.Execute(method, instance, parameter) : (object)null;
        }

        public static object CallNonPublicIfExists(
            this object instance,
            string[] methodNames,
            object parameter)
        {
            foreach (string methodName in methodNames)
            {
                MethodInfo method = instance.GetType().GetMethod(methodName, MethodInvoker.NonPublic, (Binder)null, new Type[1]
                {
                    parameter.GetType()
                }, (ParameterModifier[])null);
                if (method != (MethodInfo)null)
                    return MethodInvoker.Execute(method, instance, parameter);
            }
            return (object)null;
        }

        public static object CallPublicIfExists(
            this object instance,
            string methodName,
            object parameter)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, MethodInvoker.Public, (Binder)null, new Type[1]
            {
                parameter.GetType()
            }, (ParameterModifier[])null);
            return !(method == (MethodInfo)null) ? MethodInvoker.Execute(method, instance, parameter) : (object)null;
        }

        public static object CallPublic(this object instance, string methodName, object parameter)
        {
            MethodInfo method = instance.GetType().GetMethod(methodName, MethodInvoker.Public, (Binder)null, new Type[1]
            {
                parameter.GetType()
            }, (ParameterModifier[])null);
            if (method == (MethodInfo)null)
                throw new MissingMethodException(instance.GetType().FullName, methodName);
            return MethodInvoker.Execute(method, instance, parameter);
        }

        private static object Execute(MethodInfo mi, object instance, object parameter)
        {
            try
            {
                return mi.Invoke(instance, new object[1]
                {
                    parameter
                });
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
    }
}