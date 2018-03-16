using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace Frends.Json.Tests
{
    [TestFixture]
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


        [Test]
        public void QueryShouldWorkWithStringInput()
        {
            const string query = "$..Products[?(@.Price >= 50)].Name";
            var result =  (IEnumerable<JToken>) Json.Query(new QueryInput() {Json = jsonString, Query = query}, new QueryOptions());
            Assert.That(result.Count(),Is.EqualTo(2));
            Assert.That(result.First().Value<string>(), Is.EqualTo("Anvil"));
        }

        [Test]
        public void QueryShouldWorkWithJTokenInput()
        {
            const string query = "$..Products[?(@.Price >= 50)].Name";
            var jtoken = JToken.Parse(jsonString);
            var result = (IEnumerable<JToken>)Json.Query(new QueryInput() { Json = jtoken, Query = query }, new QueryOptions());
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Value<string>(), Is.EqualTo("Anvil"));
        }

        [Test]
        public void TestQuerySingle()
        {
            const string query = "$.Manufacturers[?(@.Name == 'Acme Co')]";
            var result = (JToken) Json.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions());
            Assert.That(result["Name"].Value<string>(), Is.EqualTo("Acme Co"));
        }

        [Test]
        public void QueryShouldThrowIfOptionSetAndNothingIsFound()
        {
            const string query = "$.Manufacturer[?(@.Name == 'Acme Co')]";
            var ex = Assert.Throws<JsonException>(() => Json.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() {ErrorWhenNotMatched = true}));
            Assert.That(ex.Message, Does.Contain("Property 'Manufacturer' does not exist on JObject."));
        }

        [Test]
        public void QuerySingleShouldNotThrowIfOptionNotSetAndNothingIsFound()
        {
            const string query = "$.Manufacturer[?(@.Name == 'Acme Co')]";
            var result =  Json.QuerySingle(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() { ErrorWhenNotMatched = false });
            Assert.That(result, Is.EqualTo(null));
        }

        [Test]
        public void QueryShouldNotThrowIfOptionNotSetAndNothingIsFound()
        {
            const string query = "$..Product[?(@.Price >= 50)].Name";
            var result = Json.Query(new QueryInput() { Json = jsonString, Query = query }, new QueryOptions() { ErrorWhenNotMatched = false });
            Assert.That(result, Is.EqualTo(Enumerable.Empty<object>()));
        }

        [Test]
        public void HandlebarShouldGenerateTemplate()
        {
            const string json = @"{'title':'Mr.', 'name':'Andersson'}";
            const string template = 
            @"<div><span>{{title}}</span> <strong>{{name}}</strong></div>";
            var result = Json.Handlebars(new HandlebarInput() {Json = json, HandlebarTemplate = template, HandlebarPartials = new HandlebarPartial[0]});
            Assert.That(result,Does.Contain("<span>Mr.</span> <strong>Andersson</strong>"));
        }

        [Test]
        public void HandlebarShouldGeneratePartials()
        {
            const string json = @"{'title':'Mr.', 'name':'Andersson'}";
            const string template =
            @"<div><span>{{title}}</span> {{> strongName}}</div>";
            var partials = new [] {new HandlebarPartial {Template = "<strong>{{name}}</strong>", TemplateName = "strongName"} };
            var result = Json.Handlebars(new HandlebarInput() { Json = json, HandlebarTemplate = template, HandlebarPartials = partials });
            Assert.That(result, Does.Contain("<span>Mr.</span> <strong>Andersson</strong>"));
        }

        [Test]
        public void JsonShouldValidate()
        {
            const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";

            const string schema = @"{
              'type': 'object',
              'properties': {
                'name': {'type':'string'},
                'roles': {'type': 'array'}
              }
            }";
            var result = Json.Validate(new ValidateInput() {Json = user, JsonSchema = schema } ,new ValidateOption());
            Assert.That(result.IsValid,Is.True);
            Assert.That(result.Errors, Is.Empty);
        }

        [Test]
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
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0], Is.EqualTo("Invalid type. Expected Object but got Array. Path 'roles', line 3, position 25.") );
        }

        [Test]
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
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0], Is.EqualTo("Unexpected character encountered while parsing value: A. Path 'name', line 2, position 21."));
        }

        [Test]
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
            var ex = Assert.Throws<JsonException>(() => Json.Validate(new ValidateInput() { Json = user, JsonSchema = schema }, new ValidateOption() {ThrowOnInvalidJson = true}));
            Assert.That(ex.Message, Does.Contain("Json is not valid. Invalid type. Expected Object but got Array. Path 'roles', line 3, position 25."));

        }

        [Test]
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
            Assert.That(ex.Message, Does.Contain("Unexpected character encountered while parsing value: A. Path 'name', line 2, position 21."));
        }

        [Test]
        public void ShouldConvertJsonStringToJToken()
        {
            const string user = @"{
              'name': 'Arnie Admin',
              'roles': ['Developer', 'Administrator']
            }";
            var result = Json.ConvertJsonStringToJToken(user);
            Assert.That(result,Is.TypeOf<JObject>());
        }

        [Test]
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
            Assert.That(result, Is.TypeOf<JObject>());
        }
    }
}
