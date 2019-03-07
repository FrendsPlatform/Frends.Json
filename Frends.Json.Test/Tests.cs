using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Frends.Json.Test
{
        public class Tests
        {
            private const string jsonString = @"{
              'Stores': [
                'Lambton Quay',
                'Willis Street'
              ],
              'Manufacturers': [
                {
                  'Name': 'Acme Co',
                  'Products': [
                    {
                      'Name': 'Anvil',
                      'Price': 50
                    }
                  ]
                },
                {
                  'Name': 'Contoso',
                  'Products': [
                    {
                     'Name': 'Elbow Grease',
                      'Price': 99.95
                    },
                    {
                     'Name': 'Headlight Fluid',
                      'Price': 4
                   }
                  ]
                }
              ]
            }";


            [Fact]
            public void QueryShouldWorkWithStringInput()
            {
                const string query = "$..Products[?(@.Price >= 50)].Name";
                var result = (IEnumerable<JToken>)Json.Query(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions());
                Assert.Equal(2, result.Count());
                Assert.Equal("Anvil", result.First().Value<string>());
            }

            [Fact]
            public void QueryShouldWorkWithJTokenInput()
            {
                const string query = "$..Products[?(@.Price >= 50)].Name";
                var jtoken = JToken.Parse(jsonString);
                var result = (IEnumerable<JToken>)Json.Query(new QueryInput() { Json = jtoken, Query = query }, new QueryOptions());
                Assert.Equal(2, result.Count());
                Assert.Equal("Anvil", result.First().Value<string>());
            }

            [Fact]
            public void TestQuerySingle()
            {
                const string query = "$.Manufacturers[?(@.Name == 'Acme Co')]";
                var result = (JToken)Json.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions());
                Assert.Equal("Acme Co", result["Name"].Value<string>());
            }

            [Fact]
            public void QueryShouldThrowIfOptionSetAndNothingIsFound()
            {
                const string query = "$.Manufacturer[?(@.Name == 'Acme Co')]";
                var ex = Assert.Throws<JsonException>(() => Json.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() { ErrorWhenNotMatched = true }));
                Assert.Contains(ex.Message,"Property 'Manufacturer' does not exist on JObject.");
            }

            [Fact]
            public void QuerySingleShouldNotThrowIfOptionNotSetAndNothingIsFound()
            {
                const string query = "$.Manufacturer[?(@.Name == 'Acme Co')]";
                var result = Json.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() { ErrorWhenNotMatched = false });
                Assert.Null(result);
            }

            [Fact]
            public void QueryShouldNotThrowIfOptionNotSetAndNothingIsFound()
            {
                const string query = "$..Product[?(@.Price >= 50)].Name";
                var result = Json.Query(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() { ErrorWhenNotMatched = false });
                Assert.Equal(result, Enumerable.Empty<object>());
            }

            [Fact]
            public void HandlebarShouldGenerateTemplate()
            {
                const string json = @"{'title':'Mr.', 'name':'Andersson'}";
                const string template =
                @"<div><span>{{title}}</span> <strong>{{name}}</strong></div>";
                var result = Json.Handlebars(new HandlebarInput() { Json = json, HandlebarTemplate = template, HandlebarPartials = new HandlebarPartial[0] });
                Assert.Contains(result, "<span>Mr.</span> <strong>Andersson</strong>");
            }

            [Fact]
            public void HandlebarShouldGeneratePartials()
            {
                const string json = @"{'title':'Mr.', 'name':'Andersson'}";
                const string template =
                @"<div><span>{{title}}</span> {{> strongName}}</div>";
                var partials = new[] { new HandlebarPartial { Template = "<strong>{{name}}</strong>", TemplateName = "strongName" } };
                var result = Json.Handlebars(new HandlebarInput() { Json = json, HandlebarTemplate = template, HandlebarPartials = partials });
                Assert.Contains(result, "<span>Mr.</span> <strong>Andersson</strong>");
            }

            const string ValidUserJson = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

            const string ValidUserSchema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'array'}
              }
            }";

            [Fact]
            public void JsonShouldValidate()
            {
                var result = Json.Validate(new ValidateInput() { Json = ValidUserJson, JsonSchema = ValidUserSchema }, new ValidateOption());
                Assert.True(result.IsValid);
                Assert.Empty(result.Errors);
            }

            [Fact]
            public void ShouldHaveLicenseSetForExecutingMoreThan1000Validations()
            {
                var results = Enumerable.Range(0, 2000).Select(i => Json.Validate(
                    new ValidateInput { Json = ValidUserJson, JsonSchema = ValidUserSchema },
                    new ValidateOption()
                    )).ToList();

                Assert.All(results.Select(r => r.IsValid), Assert.True);
            }

            [Fact]
            public void JsonShouldNotValidate()
            {
                const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

                const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
                var result = Json.Validate(new ValidateInput() { Json = user, JsonSchema = schema }, new ValidateOption());
                Assert.False(result.IsValid);
                Assert.Equal(1, result.Errors.Count);
                Assert.Equal("Invalid type. Expected Object but got Array. Path 'roles', line 3, position 24.", result.Errors[0]);
            }

            [Fact]
            public void InvalidJsonShouldNotValidate()
            {
                const string user = @"{
              name: Arnie Admin,
              roles: [Developer, Administrator]
            }";

                const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
                var result = Json.Validate(new ValidateInput() { Json = user, JsonSchema = schema }, new ValidateOption());
                Assert.False(result.IsValid);
                Assert.Equal(1, result.Errors.Count);
                Assert.Equal("Unexpected character encountered while parsing value: A. Path 'name', line 2, position 20.", result.Errors[0]);
            }

            [Fact]
            public void JsonValidationShouldThrow()
            {
                const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

                const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
                var ex = Assert.Throws<JsonException>(() => Json.Validate(new ValidateInput() { Json = user, JsonSchema = schema }, new ValidateOption() { ThrowOnInvalidJson = true }));
                Assert.Contains(ex.Message, "Json is not valid. Invalid type. Expected Object but got Array. Path 'roles', line 3, position 24.");

            }

            [Fact]
            public void InvalidJsonShouldThrow()
            {
                const string user = @"{
              name: Arnie Admin,
              roles: [Developer, Administrator]
            }";

                const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'object'}
              }
            }";
                var ex = Assert.Throws<JsonReaderException>(() => Json.Validate(new ValidateInput() { Json = user, JsonSchema = schema }, new ValidateOption() { ThrowOnInvalidJson = true }));
                Assert.Contains(ex.Message, "Unexpected character encountered while parsing value: A. Path 'name', line 2, position 20.");
            }

            [Fact]
            public void ShouldConvertJsonStringToJToken()
            {
                const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";
                var result = Json.ConvertJsonStringToJToken(user);
                Assert.IsType<JObject>(result);
            }

            [Fact]
            public void ShouldConvertXmlStringToJToken()
            {
                const string xml = @"<?xml version='1.0' standalone='no'?>
             <root>
               <person id='1'>
                 <name>Alan</name>
                 <url>http://www.google.com</url>
               </person>
               <person id='2'>
                <name>Louis</name>
                 <url>http://www.yahoo.com</url>
              </person>
            </root>";
                var result = Json.ConvertXmlStringToJToken(xml);
                Assert.IsType<JObject>(result);
            }
    }
}
