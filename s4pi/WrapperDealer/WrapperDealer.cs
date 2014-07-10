using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using s4pi.Interfaces;

namespace s4pi.WrapperDealer
{
    /// <summary>
    /// Responsible for associating ResourceType in the IResourceIndexEntry with a particular class (a "wrapper") that understands it
    /// or the default wrapper.
    /// </summary>
    public static class WrapperDealer
    {
        /// <summary>
        /// Create a new Resource of the requested type, allowing the wrapper to initialise it appropriately
        /// </summary>
        /// <param name="APIversion">API version of request</param>
        /// <param name="resourceType">Type of resource (currently a string like "0xDEADBEEF")</param>
        /// <returns></returns>
        public static IResource CreateNewResource(int APIversion, string resourceType)
        {
            return WrapperForType(resourceType, APIversion, null);
        }


        /// <summary>
        /// Retrieve a resource from a package, readying the appropriate wrapper
        /// </summary>
        /// <param name="APIversion">API version of request</param>
        /// <param name="pkg">Package containing <paramref name="rie"/></param>
        /// <param name="rie">Identifies resource to be returned</param>
        /// <returns>A resource from the package</returns>
        public static IResource GetResource(int APIversion, IPackage pkg, IResourceIndexEntry rie) { return GetResource(APIversion, pkg, rie, false); }


        /// <summary>
        /// Retrieve a resource from a package, readying the appropriate wrapper or the default wrapper
        /// </summary>
        /// <param name="APIversion">API version of request</param>
        /// <param name="pkg">Package containing <paramref name="rie"/></param>
        /// <param name="rie">Identifies resource to be returned</param>
        /// <param name="AlwaysDefault">When true, indicates WrapperDealer should always use the DefaultResource wrapper</param>
        /// <returns>A resource from the package</returns>
        public static IResource GetResource(int APIversion, IPackage pkg, IResourceIndexEntry rie, bool AlwaysDefault)
        {
            return WrapperForType(AlwaysDefault ? "*" : rie["ResourceType"], APIversion, (pkg as APackage).GetResource(rie));
        }

        /// <summary>
        /// Retrieve the resource wrappers known to WrapperDealer.
        /// </summary>
        public static ICollection<KeyValuePair<string, Type>> TypeMap { get { return new List<KeyValuePair<string, Type>>(typeMap); } }

        /// <summary>
        /// Access the collection of wrappers on the &quot;disabled&quot; list.
        /// </summary>
        /// <remarks>Updates to entries in the collection will be used next time a wrapper is requested.
        /// Existing instances of a disabled wrapper will not be invalidated and it will remain possible to
        /// bypass WrapperDealer and instantiate instances of the wrapper class directly.</remarks>
        public static ICollection<KeyValuePair<string, Type>> Disabled { get { return disabled; } }

        #region Implementation
        static List<KeyValuePair<string, Type>> typeMap = null;

        static List<KeyValuePair<string, Type>> disabled = new List<KeyValuePair<string, Type>>();

        static WrapperDealer()
        {
            string folder = Path.GetDirectoryName(typeof(WrapperDealer).Assembly.Location);
            typeMap = new List<KeyValuePair<string, Type>>();
            foreach (string path in Directory.GetFiles(folder, "*.dll"))
            {
                try
                {
                    Assembly dotNetDll = Assembly.LoadFile(path);
                    Type[] types = dotNetDll.GetTypes();
                    foreach (Type t in types)
                    {
                        if (!t.IsSubclassOf(typeof(AResourceHandler))) continue;

                        AResourceHandler arh = (AResourceHandler)t.GetConstructor(new Type[] { }).Invoke(new object[] { });

                        if (arh == null) continue;

                        foreach (Type k in arh.Keys)
                        {
                            foreach (string s in arh[k])
                                typeMap.Add(new KeyValuePair<string, Type>(s, k));
                        }
                    }
                }
                catch { }
            }
            typeMap.Sort((x, y) => x.Key.CompareTo(y.Key));
        }

        static IResource WrapperForType(string type, int APIversion, Stream s)
        {
            Type t = null;
            t = typeMap.Find(x => !disabled.Contains(x) && x.Key == type).Value;

            if (t == null)
                t = typeMap.Find(x => !disabled.Contains(x) && x.Key == "*").Value;

            if (Settings.Settings.Checking) if (t == null)
                    throw new InvalidOperationException("Could not find a resource handler");

            return (IResource)t.GetConstructor(new Type[] { typeof(int), typeof(Stream), }).Invoke(new object[] { APIversion, s });
        }
        #endregion
    }
}
