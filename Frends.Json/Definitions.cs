using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable 1591

namespace Frends.Json
{
    public class QueryInput
    {
        /// <summary>
        /// Json input needs to be of type string or JToken
        /// </summary>
        [DefaultValue("{\"key\":\"value\"}")]
        [DisplayFormat(DataFormatString = "Json")]
        public dynamic Json { get; set; }

        /// <summary>
        /// The query is of type JSONPath. More details: http://goessner.net/articles/JsonPath/
        /// </summary>
        [DefaultValue("\"$.key\"")]
        public string Query { get; set; }
    }

    public class QueryOptions
    {
        /// <summary>
        /// A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.
        /// </summary>
        public bool ErrorWhenNotMatched { get; set; }
    }

    public class HandlebarInput
    {
        /// <summary>
        /// Json input needs to be of type string or JToken
        /// </summary>
        [DefaultValue("{\"title\":\"Mr.\", \"name\":\"Foo\" }")]
        [DisplayFormat(DataFormatString = "Json")]
        public dynamic Json { get; set; }

        /// <summary>
        /// Template for handlebars. > indicates a partial. This needs to be in expression mode. Using {{ }} in other modes breaks the task.
        /// </summary>
        [DefaultValue("\"<xml> {{title}} {{> strongName}} </xml>\"")]
        [DisplayFormat(DataFormatString = "Expression")]
        public string HandlebarTemplate { get; set; }

        /// <summary>
        /// Partials for template.
        /// </summary>
        public HandlebarPartial[] HandlebarPartials { get; set; }
    }

    public class HandlebarPartial
    {
        /// <summary>
        /// Template name that exists in the HandlebarTemplate.
        /// </summary>
        [DefaultValue("\"strongName\"")]
        public string TemplateName { get; set; }

        /// <summary>
        /// Partial template. This needs to be in expression mode. Using {{ }} in other modes breaks the task.
        /// </summary>
        [DefaultValue("\"<strong>{{name}}</strong>\"")]
        [DisplayFormat(DataFormatString = "Expression")]
        public string Template { get; set; }
    }

    public class ValidateInput
    {
        /// <summary>
        /// Json input needs to be of type string or JToken
        /// </summary>
        [DefaultValue("{\"name\": \"Foo\"}")]
        [DisplayFormat(DataFormatString = "Json")]
        public dynamic Json { get; set; }

        /// <summary>
        /// Json Schema to validate to. Uses Newtonsoft JsonSchema
        /// </summary>
        [DefaultValue("{\"type\": \"object\", \"properties\": {\"name\": {\"type\":\"string\"} } }")]
        [DisplayFormat(DataFormatString = "Json")]
        public string JsonSchema { get; set; }
    }

    public class ValidateOption
    {
        public bool ThrowOnInvalidJson { get; set; }
    }

    public class ValidateResult
    {
        public bool IsValid { get; set; }
        public IList<string> Errors { get; set; }
    }
}
