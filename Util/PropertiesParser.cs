using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Specialized;

namespace OsnTester.Util
{
    /// <summary>
	/// This is an utility class used to parse the properties.
	/// </summary>
	/// <author> James House</author>
    /// <author>Marko Lahma (.NET)</author>
    public class PropertiesParser
    {
        internal readonly NameValueCollection props;

        /// <summary>
        /// Gets the underlying properties.
        /// </summary>
        /// <value>The underlying properties.</value>
        public virtual NameValueCollection UnderlyingProperties
        {
            get { return props; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesParser"/> class.
        /// </summary>
        /// <param name="props">The props.</param>
        public PropertiesParser(NameValueCollection props)
        {
            this.props = props;
        }

        /// <summary>
        /// Gets the string property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual string GetStringProperty(string name)
        {
            string val = props.Get(name);
            if (val == null)
            {
                return null;
            }
            return val.Trim();
        }

        /// <summary>
        /// Gets the string property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual string GetStringProperty(string name, string defaultValue)
        {
            string val = props[name] ?? defaultValue;
            if (val == null)
            {
                return defaultValue;
            }
            val = val.Trim();
            if (val.Length == 0)
            {
                return defaultValue;
            }
            return val;
        }

        /// <summary>
        /// Gets the string array property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public virtual IList<string> GetStringArrayProperty(string name)
        {
            return GetStringArrayProperty(name, null);
        }

        /// <summary>
        /// Gets the string array property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual IList<string> GetStringArrayProperty(string name, string[] defaultValue)
        {
            string vals = GetStringProperty(name);
            if (vals == null)
            {
                return defaultValue;
            }

            string[] items = vals.Split(',');
            List<string> strs = new List<string>(items.Length);
            try
            {
                strs.AddRange(items.Select(s => s.Trim()));

                return strs;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the property group.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns></returns>
        public virtual NameValueCollection GetPropertyGroup(string prefix)
        {
            return GetPropertyGroup(prefix, false);
        }

        /// <summary>
        /// Gets the property group.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="stripPrefix">if set to <c>true</c> [strip prefix].</param>
        /// <returns></returns>
        public virtual NameValueCollection GetPropertyGroup(string prefix, bool stripPrefix)
        {
            return GetPropertyGroup(prefix, stripPrefix, null);
        }


        /// <summary>
        /// Get all properties that start with the given prefix.  
        /// </summary>
        /// <param name="prefix">The prefix for which to search.  If it does not end in a "." then one will be added to it for search purposes.</param>
        /// <param name="stripPrefix">Whether to strip off the given <paramref name="prefix" /> in the result's keys.</param>
        /// <param name="excludedPrefixes">Optional array of fully qualified prefixes to exclude.  For example if <see paramfref="prefix" /> is "a.b.c", then <see paramref="excludedPrefixes" /> might be "a.b.c.ignore".</param>
        /// <returns>Group of <see cref="NameValueCollection" /> that start with the given prefix, optionally have that prefix removed, and do not include properties that start with one of the given excluded prefixes.</returns>
        public virtual NameValueCollection GetPropertyGroup(string prefix, bool stripPrefix, string[] excludedPrefixes)
        {
            NameValueCollection group = new NameValueCollection();

            if (!prefix.EndsWith("."))
            {
                prefix += ".";
            }

            foreach (string key in props.Keys)
            {
                if (key.StartsWith(prefix))
                {
                    bool exclude = false;
                    if (excludedPrefixes != null)
                    {
                        for (int i = 0; (i < excludedPrefixes.Length) && (exclude == false); i++)
                        {
                            exclude = key.StartsWith(excludedPrefixes[i]);
                        }
                    }

                    if (exclude == false)
                    {
                        string value = GetStringProperty(key, "");
                        if (String.IsNullOrEmpty(value))
                            continue;

                        if (stripPrefix)
                        {
                            group[key.Substring(prefix.Length)] = value;
                        }
                        else
                        {
                            group[key] = value;
                        }
                    }
                }
            }

            return group;
        }
    }
}
