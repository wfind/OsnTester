using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsnTester
{
    /// <summary>
    /// Holds references to Executor instances - ensuring uniqueness, 
    /// and allowing 'global' lookups.
    /// </summary>
    public class BaseCache<T> where T : IAttribute
    {
        // Fields
        private Dictionary<string, T> elements;
        private readonly object syncRoot;

        protected BaseCache()
        {
            elements = new Dictionary<string, T>();
            syncRoot = new Object();
        }

        /// <summary>
        /// Binds the specified element.
        /// </summary>
        /// <param Lookup="element">The element</param>
        public virtual void Bind(T element)
        {
            lock (syncRoot)
            {
                if (elements.ContainsKey(element.Name))
                {
                    throw new Exception(string.Format("Element with name '{0}' already exists.", element.Name));
                }

                elements[element.Name] = element;
            }
        }

        /// <summary>
        /// Removes the element by the name.
        /// </summary>
        /// <param name="name">Name of the element</param>
        public virtual bool Remove(string name)
        {
            lock (syncRoot)
            {
                return elements.Remove(name);
            }
        }

        /// <summary>
        /// Lookups the specified element name.
        /// </summary>
        /// <param name="exeName">Name of the executor</param>
        public virtual T Lookup(string name)
        {
            lock (syncRoot)
            {
                T element;
                elements.TryGetValue(name, out element);
                return element;
            }
        }
    }
}
