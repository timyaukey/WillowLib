using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Willowsoft.WillowLib.Data.Entity;

namespace Willowsoft.WillowLib.Data.Misc
{
    // Store data defined for the entire context in which
    // the code is running, which for a Windows or console app is the
    // current process, and for a web app is the HTTP request.
    // The behavior of the members of this interface is the same
    // as the behavior of a Dictionary<string,object>.
    public interface IAmbientDataProvider
    {
        object this[string key] { get; set; }
        bool TryGetValue(string key, out object value);
    }

    // An IAmbientDataProvider for Windows or console apps.
    public class StaticAmbientDataProvider : IAmbientDataProvider
    {
        private IDictionary<string, object> mValues = new Dictionary<string, object>();

        public object this[string key]
        {
            get
            {
                return mValues[key];
            }
            set
            {
                mValues[key] = value;
            }
        }
        public bool TryGetValue(string key, out object value)
        {
            return mValues.TryGetValue(key, out value);
        }
    }

    // Expose the appropriate IAmbientDataProvider.
    // Must be set when the context begins, e.g. Windows or console app startup
    // or the beginning of the HTTP request.
    public static class Ambient
    {
        private static IAmbientDataProvider mProvider;

        private const string mDbSessionKey = "DbSession";
        
        public static void Configure(IAmbientDataProvider provider)
        {
            mProvider = provider;
        }
        
        public static IAmbientDataProvider Data
        {
            get { return mProvider; }
        }

        public static bool TryGetValue(string key, out object value)
        {
            return mProvider.TryGetValue(key, out value);
        }

        public static IDbSession DbSession
        {
            get { return (IDbSession)mProvider[mDbSessionKey]; }
            set { mProvider[mDbSessionKey] = value; }
        }
    }
}
