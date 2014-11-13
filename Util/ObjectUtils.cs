using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Collections.Specialized;
using System.Globalization;
using System.Configuration;
using System.ComponentModel;
using System.ServiceModel.Configuration;

namespace OsnTester.Util
{
    /// <summary>
    /// Utility methods that are used to convert objects from one type into another.
    /// </summary>
    /// <author>Aleksandar Seovic</author>
    /// <author>Marko Lahma</author>
    public static class ObjectUtils
    {
        /// <summary>
        /// Instantiates an instance of the type specified.
        /// </summary>
        /// <returns></returns>
        public static T InstantiateType<T>(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type", "Cannot instantiate null");
            }
            ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
            if (ci == null)
            {
                throw new ArgumentException("Cannot instantiate type which has no empty constructor", type.Name);
            }
            return (T)ci.Invoke(new object[0]);
        }

        /// <summary>
        /// Sets the object properties using reflection.
        /// </summary>
        /// <param name="obj">The object to set values to.</param>
        /// <param name="props">The properties to set to object.</param>
        public static void SetObjectProperties(object obj, NameValueCollection props)
        {
            // remove the type
            props.Remove("type");

            foreach (string name in props.Keys)
            {
                string propertyName = name.Substring(0, 1).ToUpper(CultureInfo.InvariantCulture) + name.Substring(1);

                try
                {
                    object value = props[name];
                    SetPropertyValue(obj, propertyName, value);
                }
                catch (Exception nfe)
                {
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "Could not parse property '{0}' into correct data type: {1}", name, nfe.Message), nfe);
                }
            }
        }

        public static void SetPropertyValue(object target, string propertyName, object value)
        {
            Type t = target.GetType();

            PropertyInfo pi = t.GetProperty(propertyName);

            if (pi == null || !pi.CanWrite)
            {
                // try to find from interfaces
                foreach (var interfaceType in target.GetType().GetInterfaces())
                {
                    pi = interfaceType.GetProperty(propertyName);
                    if (pi != null && pi.CanWrite)
                    {
                        // found suitable
                        break;
                    }
                }
            }

            if (pi == null)
            {
                // not match from anywhere
                throw new MemberAccessException(string.Format(CultureInfo.InvariantCulture, "No writable property '{0}' found", propertyName));
            }

            MethodInfo mi = pi.GetSetMethod();

            if (mi == null)
            {
                throw new MemberAccessException(string.Format(CultureInfo.InvariantCulture, "Property '{0}' has no setter", propertyName));
            }

            if (mi.GetParameters()[0].ParameterType == typeof(List<string>))
            {
                // special handling
                value = GetListValueForProperty(value);
            }
            else
            {
                value = ConvertValueIfNecessary(mi.GetParameters()[0].ParameterType, value);
            }

            mi.Invoke(target, new object[] { value });
        }

        public static List<string> GetListValueForProperty(object value)
        {
            string vals = value as string;
            if (string.IsNullOrEmpty(vals))
                return null;

            string[] items = vals.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> strs = new List<string>(items);

            return strs;
        }

        /// <summary>
        /// Convert the value to the required <see cref="System.Type"/> (if necessary from a string).
        /// </summary>
        /// <param name="newValue">The proposed change value.</param>
        /// <param name="requiredType">
        /// The <see cref="System.Type"/> we must convert to.
        /// </param>
        /// <returns>The new value, possibly the result of type conversion.</returns>
        public static object ConvertValueIfNecessary(Type requiredType, object newValue)
        {
            if (newValue != null)
            {
                // if it is assignable, return the value right away
                if (IsAssignableFrom(newValue, requiredType))
                {
                    return newValue;
                }

                // try to convert using type converter
                TypeConverter typeConverter = TypeDescriptor.GetConverter(requiredType);
                if (typeConverter.CanConvertFrom(newValue.GetType()))
                {
                    return typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, newValue);
                }
                typeConverter = TypeDescriptor.GetConverter(newValue);
                if (typeConverter.CanConvertTo(requiredType))
                {
                    return typeConverter.ConvertTo(null, CultureInfo.InvariantCulture, newValue, requiredType);
                }
                if (requiredType == typeof(Type))
                {
                    return Type.GetType(newValue.ToString(), true);
                }

                throw new NotSupportedException(newValue + " is no a supported value for a target of type " + requiredType);
            }
            if (requiredType.IsValueType)
            {
                return Activator.CreateInstance(requiredType);
            }

            // return default
            return null;
        }

        /// <summary>
        /// Gets the WCF proxy client endpoints in app.config file.
        /// </summary>
        /// <returns><see cref="ChannelEndpointElementCollection"/></returns>
        public static ChannelEndpointElementCollection GetClientEndpoint()
        {
            var appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceModel = ServiceModelSectionGroup.GetSectionGroup(appConfig);
            return serviceModel.Client.Endpoints;
        }

        /// <summary>
        /// Determines whether value is assignable to required type.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="requiredType">Type of the required.</param>
        /// <returns>
        /// 	<c>true</c> if value can be assigned as given type; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsAssignableFrom(object value, Type requiredType)
        {
            return requiredType.IsInstanceOfType(value);
        }
    }
}
