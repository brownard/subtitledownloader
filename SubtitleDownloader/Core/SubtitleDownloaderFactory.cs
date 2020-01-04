using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SubtitleDownloader.Util;

namespace SubtitleDownloader.Core
{
    public static class SubtitleDownloaderFactory
    {
        private readonly static Dictionary<string, ISubtitleDownloader> DownloaderInstances =
            new Dictionary<string, ISubtitleDownloader>();

        static SubtitleDownloaderFactory()
        {
            try
            {
                var implementations = FindDownloaderImplementations();
                
                CreateDownloaderInstances(implementations);
            }
            catch (ReflectionTypeLoadException tLException)
            {
                var loaderMessages = new StringBuilder();
                
                loaderMessages.AppendLine("Following loader exceptions were found: ");
                
                foreach (var loaderException in tLException.LoaderExceptions)
                {
                    loaderMessages.AppendLine(loaderException.Message);
                }
                
                throw new Exception(loaderMessages.ToString(), tLException);
            }
        }

        public static List<String> GetSubtitleDownloaderNames()
        {
            return new List<String>(DownloaderInstances.Keys);
        }

        public static ISubtitleDownloader GetSubtitleDownloader(string downloaderName)
        {
            if (!DownloaderInstances.ContainsKey(downloaderName))
                throw new ArgumentException("No subtitle downloader implementation found with downloader name: " + downloaderName);

            return DownloaderInstances[downloaderName];
        }

        private static IEnumerable<Type> FindDownloaderImplementations()
        {
            var downloaderClasses = new List<Type>();

            // Get implementations in current assembly
            downloaderClasses.AddRange(
                TypesImplementingInterface(Assembly.GetExecutingAssembly(), typeof(ISubtitleDownloader)));

            // Get implementations in SubtitleDownloaders-folder
            var downloadersDirectory = FileUtils.AssemblyDirectory + "\\SubtitleDownloaders";

            if (Directory.Exists(downloadersDirectory))
            {
                var filePaths = Directory.GetFiles(downloadersDirectory);

                foreach (var s in filePaths.Where(s => s.EndsWith("dll")))
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(s);
                        downloaderClasses.AddRange(TypesImplementingInterface(assembly,
                                                                              typeof(ISubtitleDownloader)));
                    }
                    catch
                    {
                        // Cannot do anything
                    }
                }
            }

            return downloaderClasses;
        }

        private static void CreateDownloaderInstances(IEnumerable<Type> downloaderImplementations)
        {
            foreach (var instance in downloaderImplementations.Select(type => 
                    (ISubtitleDownloader) Activator.CreateInstance(type)))
            {
                DownloaderInstances.Add(instance.GetName(), instance);
            }
        }

        private static IEnumerable<Type> TypesImplementingInterface(Assembly assembly, Type desiredType)
        {
            return assembly.GetTypes().Where(type => 
                    desiredType.IsAssignableFrom(type) && IsRealClass(type));
        }

        private static bool IsRealClass(Type testType)
        {
            return testType.IsAbstract == false
                && testType.IsGenericTypeDefinition == false
                && testType.IsInterface == false;
        }
    }
}