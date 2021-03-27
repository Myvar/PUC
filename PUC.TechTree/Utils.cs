using System.IO;
using System.Reflection;

namespace PUC.TechTree
{
    public static class Utils
    {
        public static string GetResourceFile(string file)
        {
            using (var manifestResourceStream =
                typeof(Utils).GetTypeInfo().Assembly.GetManifestResourceStream("PUC.TechTree._res." + file))
            {
                using (var streamReader = new StreamReader(manifestResourceStream))
                    return streamReader.ReadToEnd();
            }
        }
    }
}