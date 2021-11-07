namespace Transaction.Api.UnitTest.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Embedded JSON resource reader helper.
    /// </summary>
    internal static class EmbeddedJsonResourceReaderHelper
    {
        public static IEnumerable<T[]> GetMemberData<T>(string namespaceAndFileName)
        {
            T[] result = Get<T[]>(namespaceAndFileName);
            return result
                .Select(res => new[] { res });
        }
        /// <summary>
        /// Get object from embedded JSON resource.
        /// </summary>
        /// <typeparam name="T">Type to convert JSON to.</typeparam>
        /// <param name="namespaceAndFileName">Full namespace and JSON file name.</param>
        /// <returns>Object of type T.</returns>
        public static T Get<T>(string namespaceAndFileName)
        {
            string jsonString = GetEmbeddedJsonResource(namespaceAndFileName);
            T result = JsonConvert.DeserializeObject<T>(jsonString);
            return result;
        }

        public static Stream GetFileStream(string namespaceAndFileName)
        {
            return typeof(EmbeddedJsonResourceReaderHelper).GetTypeInfo().Assembly.GetManifestResourceStream(namespaceAndFileName);
        }

        private static string GetEmbeddedJsonResource(string namespaceAndFileName)
        {
            using (Stream stream = GetFileStream(namespaceAndFileName))
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
