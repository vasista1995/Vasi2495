using System;
using System.ComponentModel;
using System.Reflection;

namespace PDS.SpaceNew.PADS.Module.Helper
{
    public static class MethodInvoker
    {
        public static object InvokeMethod(Type type, string methodName, object[] parameters)
        {
            var methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (methodInfo != null)
            {
                var methodParams = methodInfo.GetParameters();
                if (parameters.Length != methodParams.Length)
                {
                    throw new ArgumentException($"Parameter count mismatch: methodParams.Length: " +
                        $"{methodParams.Length} != parameters.Length: {parameters.Length}");
                }

                var convertedParameters = ConvertParameterTypes(methodParams, parameters);
                return methodInfo.Invoke(null, convertedParameters);
            }
            else
            {
                throw new ArgumentException($"Static method '{methodName}' not found in type '{type.Name}'.");
            }
        }

        private static object[] ConvertParameterTypes(ParameterInfo[] methodParams, object[] parameters)
        {
            var convertedParameters = new object[methodParams.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                {
                    convertedParameters[i] = null;
                    continue;
                }

                ParameterInfo methodParam = methodParams[i];
                try
                {
                    if (methodParam.ParameterType.IsGenericType && methodParam.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // Target type is a nullable value type
                        Type underlyingType = Nullable.GetUnderlyingType(methodParam.ParameterType);
                        convertedParameters[i] = Convert.ChangeType(parameters[i], underlyingType);
                        convertedParameters[i] = Activator.CreateInstance(methodParam.ParameterType, convertedParameters[i]);
                    }
                    else
                    {
                        convertedParameters[i] = Convert.ChangeType(parameters[i], methodParam.ParameterType);
                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException($"Cannot convert parameter {i} from " +
                        $"{parameters[i].GetType().Name} to {methodParam.ParameterType.Name}.");
                }
            }

            return convertedParameters;
        }
    }
}
