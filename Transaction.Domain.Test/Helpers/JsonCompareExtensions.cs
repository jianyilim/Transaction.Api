namespace Transaction.Api.UnitTest.Helpers
{
    using Newtonsoft.Json;

    public static class JsonCompareExtensions
    {
        /// <summary>
        /// Get object from embedded JSON resource.
        /// </summary>
        /// <typeparam name="T">Type to convert JSON to.</typeparam>
        /// <param name="namespaceAndFileName">Full namespace and JSON file name.</param>
        /// <returns>Object of type T.</returns>
        public static bool EqualJson(this object object1, object object2)
        {
            return JsonConvert.SerializeObject(object1) == JsonConvert.SerializeObject(object2);
        }
    }
}
