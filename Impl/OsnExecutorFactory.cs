using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.Specialized;
using System.Reflection;

using OsnTester.Util;

namespace OsnTester.Impl
{
    /// <summary>
    /// An implementation of <see cref="IExecutorFactory" /> that
    /// does all of it's work of creating a <see cref="OsnExecutor" /> instance
    /// based on the contents of a properties file.
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class OsnExecutorFactory : IExecutorFactory
    {
        // Fields
        private PropertiesParser cfg;
        private IEnumerable<Type> exeTypes;

        // Public instance constructors
        public OsnExecutorFactory() : this(null) { }
        public OsnExecutorFactory(NameValueCollection props)
        {
            cfg = new PropertiesParser(props);

            Assembly ass = Assembly.GetEntryAssembly();
            exeTypes = ass.GetTypes().Where(t => t.Name.Contains(TYPE_SUFFIX));
        }

        // Methods
        /// <summary>
        /// Initialize the <see cref="IExecutorFactory" />.
        /// </summary>
        /// <remarks>
        /// By default a properties file named "osntester.properties" is loaded from
        /// the 'current working directory'. If you wish to use a file other than these defaults,
        /// you must define the system property 'osntester.properties' to point to
        /// the file you want.
        /// </remarks>
        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void Initialize(NameValueCollection props)
        {
            cfg = new PropertiesParser(props);
            ValidateConfiguration();
        }

        protected virtual void ValidateConfiguration()
        {
            // determine currently supported configuration keys via reflection
            List<string> supportedKeys = new List<string>();
            List<FieldInfo> fields = new List<FieldInfo>(GetType().GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
            // choose constant string fields
            fields = fields.FindAll(field => field.FieldType == typeof(string));

            // read value from each field
            foreach (FieldInfo field in fields)
            {
                string value = (string)field.GetValue(null);
                if (value != null && value.StartsWith(PropertyExecutorPrefix) && value != PropertyExecutorPrefix)
                {
                    supportedKeys.Add(value);
                }
            }

            // now check against allowed))
            foreach (string configurationKey in cfg.UnderlyingProperties.AllKeys)
            {
                if (!configurationKey.StartsWith(PropertyExecutorPrefix))
                {
                    // don't bother if truly unknown property
                    continue;
                }

                bool isMatch = false;
                foreach (string supportedKey in supportedKeys)
                {
                    if (configurationKey.StartsWith(supportedKey, StringComparison.InvariantCulture))
                    {
                        isMatch = true;
                        break;
                    }
                }
                if (!isMatch)
                {
                    throw new Exception("Unknown configuration property '" + configurationKey + "'");
                }
            }
        }

        /// <summary>
        /// Returns a handle to the Executor produced by this factory.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the executor has been created, then the executor will be gotten from
        /// cache and set it properties.
        /// </para>
        /// <para>
        /// If one of the <see cref="Initialize()" /> methods has not be previously
        /// called, then the default (no-arg) <see cref="Initialize()" /> method
        /// will be called by this method.
        /// </para>
        /// </remarks>
        public IExecutor CreateExecutor()
        {
            if (cfg == null)
            {
                Initialize();
            }

            // Properties ignore case.
            string typeName = cfg.GetStringProperty(PropertyExecutorType);
            typeName = typeName.Substring(0, 1).ToUpper() + typeName.Substring(1).ToLower() + TYPE_SUFFIX;

            ExecutorCache exeCache = ExecutorCache.Instance;
            IExecutor executor = exeCache.Lookup(typeName);

            if (executor == null)
            {
                Type typeExecutor = LoadType(typeName);
                executor = ObjectUtils.InstantiateType<IExecutor>(typeExecutor);
                exeCache.Bind(executor);
            }

            ObjectUtils.SetObjectProperties(executor, cfg.GetPropertyGroup(PropertyExecutorPrefix, true));

            return executor;
        }

        public IExecutor GetExecutor(string exeName)
        {
            ExecutorCache exeCache = ExecutorCache.Instance;
            return exeCache.Lookup(exeName);
        }

        private Type LoadType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            return exeTypes.First(t => t.Name.Equals(typeName, StringComparison.InvariantCulture));
        }

        // Constant
        private const string PropertyExecutorType = "osnTester.executor.type";
        private const string PropertyExecutorCom = "osnTester.executor.com";
        private const string PropertyExecutorParas = "osnTester.executor.paras";
        private const string PropertyExecutorPrefix = "osnTester.executor";
        private const string TYPE_SUFFIX = "Executor";
    }
}
