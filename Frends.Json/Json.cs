using Frends.Tasks.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
#pragma warning disable 1591

namespace Frends.Json
{
   
    public class Json
    {
        /// <summary>
        /// Query a json string / json token for a single result. See https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>JToken</returns>
        public static object QuerySingle([CustomDisplay(DisplayOption.Tab)] QueryInput input, [CustomDisplay(DisplayOption.Tab)] QueryOptions options)
        {
            JToken jToken = GetJTokenFromInput(input.Json);
           
            return jToken.SelectToken(input.Query,options.ErrorWhenNotMatched);
        }

        /// <summary>
        /// Query a json string / json token. See https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>JToken[]</returns>
        public static IEnumerable<object> Query([CustomDisplay(DisplayOption.Tab)] QueryInput input, [CustomDisplay(DisplayOption.Tab)] QueryOptions options)
        {
            JToken jToken = GetJTokenFromInput(input.Json);

            return jToken.SelectTokens(input.Query, options.ErrorWhenNotMatched);
        }

        /// <summary>
        /// Handlebars provides the power necessary to let you build semantic templates effectively with no frustration. See https://github.com/rexm/Handlebars.Net and https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>string</returns>
        public static string Handlebars(HandlebarInput input)
        {
            var template = HandlebarsDotNet.Handlebars.Compile(input.HandlebarTemplate);

            foreach (var partial in input.HandlebarPartials)
            {
                using (var reader = new StringReader(partial.Template))
                {
                    var partialTemplate = HandlebarsDotNet.Handlebars.Compile(reader);
                    HandlebarsDotNet.Handlebars.RegisterTemplate(partial.TemplateName, partialTemplate);
                }
            }

            JToken jToken = GetJTokenFromInput(input.Json);
            return template(jToken);
        }

        /// <summary>
        /// Validate your json with Json.NET Schema. See http://www.newtonsoft.com/jsonschema and https://github.com/FrendsPlatform/Frends.Json
        /// </summary>
        /// <returns>Object { bool IsValid, string Error }</returns>
        public static ValidateResult Validate([CustomDisplay(DisplayOption.Tab)]ValidateInput input, [CustomDisplay(DisplayOption.Tab)] ValidateOption options)
        {
            var schema = JSchema.Parse(input.JsonSchema);
            JToken jToken = GetJTokenFromInput(input.Json);

            IList<string> errors;
            var isValid = jToken.IsValid(schema, out errors);
            if (!isValid && options.ThrowOnInvalidJson)
            {
                throw new JsonException($"Json is not valid. {string.Join("; ", errors)}");
            }
            return new ValidateResult() {IsValid = isValid, Errors = errors};
        }

        /// <summary>
        /// Convert a json string to JToken
        /// </summary>
        /// <returns>JToken</returns>
        public static object ConvertJsonStringToJToken(string json)
        {
            return JToken.Parse(json);
        }

        /// <summary>
        /// Convert XML string to JToken
        /// </summary>
        /// <returns>JToken</returns>
        public static object ConvertXmlStringToJToken(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var jsonString = JsonConvert.SerializeXmlNode(doc);
            return JToken.Parse(jsonString);

        }

        private static object GetJTokenFromInput(dynamic json)
        {
            if (json is string)
            {
                return JToken.Parse(json);
            }

            if (json is JToken)
            {
                return json;
            }

            throw new InvalidDataException("The input data was not recognized. Supported formats are JSON string and JToken.");
        }
    }
}
