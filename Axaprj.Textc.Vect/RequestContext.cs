using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Axaprj.Textc.Vect
{
    /// <summary>
    /// Defines a context that uses an internal dictionary to store the variables.
    /// (tag:thread_safe)
    /// </summary>
    public class RequestContext : Takenet.Textc.IRequestContext
    {
        readonly ConcurrentDictionary<string, object> ContextVariableDictionary;
        public CultureInfo Culture { get; }

        public RequestContext()
            : this(CultureInfo.InvariantCulture)        {        }

        public RequestContext(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            Culture = culture;
            ContextVariableDictionary = new ConcurrentDictionary<string, object>();
        }


        public virtual void SetVariable(string name, object value)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            object obj;
            ContextVariableDictionary.TryRemove(name, out obj);
            ContextVariableDictionary.TryAdd(name, value);
        }

        public virtual object GetVariable(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            object value;
            if(ContextVariableDictionary.TryGetValue(name, out value))
                return value;
            else
                return null;
        }

        public virtual void RemoveVariable(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            object obj;
            ContextVariableDictionary.TryRemove(name, out obj);
        }

        public void Clear()
        {
            ContextVariableDictionary.Clear();
        }
    }
}
