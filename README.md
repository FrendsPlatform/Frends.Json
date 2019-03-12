- [Frends.Json](#frendsjson)
  - [Installing](#installing)
  - [Building](#building)
  - [Contributing](#contributing)
  - [Documentation](#documentation)
    - [Json.Query](#jsonquery)
      - [Input](#input)
      - [Options](#options)
      - [Result](#result)
    - [Json.QuerySingle](#jsonquerysingle)
      - [Input](#input-1)
      - [Options](#options-1)
      - [Result](#result-1)
    - [Json.Handlebars](#jsonhandlebars)
      - [Input](#input-2)
      - [Result](#result-2)
    - [Json.Validate](#jsonvalidate)
      - [Input](#input-3)
      - [Result](#result-3)
    - [Json.ConvertXmlStringToJToken](#jsonconvertxmlstringtojtoken)
      - [Input](#input-4)
      - [Result](#result-4)
    - [Json.ConvertJsonStringToJToken](#jsonconvertjsonstringtojtoken)
      - [Input](#input-5)
      - [Result](#result-5)
  - [License](#license)

# Frends.Json

## Installing
You can install the task via FRENDS UI Task view, by searching for packages. You can also download the latest NuGet package from https://www.myget.org/feed/frends/package/nuget/Frends.Json and import it manually via the Task view.

## Building
Clone a copy of the repo

`git clone https://github.com/FrendsPlatform/Frends.Json.git`


Restore dependencies

`dotnet restore`

Rebuild the project

`dotnet build`

Run Tests

`dotnet test Frends.Json.Test`

Create a nuget package

`dotnet pack Frends.Json`

## Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## Documentation

### Json.Query 

Query a json string / json token using JSONPath query.

#### Input

| Property        | Type                          | Description                  | Example                  |
|-----------------|-------------------------------|------------------------------|--------------------------|
| Json            | dynamic ( JToken / string )   | Json for the query.          | `{"key":"value"}`        |
| Query           | string                        | Query that uses JSONPath syntax. See http://goessner.net/articles/JsonPath/ for details | `$.store.book[0].title` `$['store']['book'][0]['title']`  |

#### Options

| Property                | Type           | Description                                    |
|-------------------------|----------------|------------------------------------------------|
| ErrorWhenNotMatched     | bool           | A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression. |

#### Result
Array [ JToken ]

### Json.QuerySingle 

Query a json string / json token for a single result

#### Input

| Property        | Type                          | Description                  | Example                  |
|-----------------|-------------------------------|------------------------------|--------------------------|
| Json            | dynamic ( JToken / string )   | Json for the query.          | `{"key":"value"}`        |
| Query           | string                        | Query that uses JSONPath syntax. See http://goessner.net/articles/JsonPath/ for details | `$.store.book[0].title` `$['store']['book'][0]['title']`  |

#### Options

| Property                | Type           | Description                                    |
|-------------------------|----------------|------------------------------------------------|
| ErrorWhenNotMatched     | bool           | A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression. |

#### Result
JToken

### Json.Handlebars

Handlebars provides the power necessary to let you build semantic templates effectively with no frustration. See https://github.com/rexm/Handlebars.Net

#### Input

| Property          | Type                          | Description                         | Example                                |
|-------------------|-------------------------------|-------------------------------------|----------------------------------------|
| Json              | dynamic ( JToken / string )                   | Object used to fill the template                                                                          | `{"title":"mr.", "name":"foo"}`        |
| HandlebarTemplate | string                                        | Template for handlebars. This needs to be in expression mode. Using {{ }} in other modes breaks the task. | `<xml> {{title}} {{> strongName}} </xml>`                 |
| HandlebarPartials | Array{string TemplateName, string Template}   | Partials for a handlebars template. A partial is inicated by adding a {{'>' templateName }} inside the parent HandlebarTemplate  | `TemplateName: "strongName", Template: "<strong>{{name}}</strong>"`   |

#### Result
string

### Json.Validate

Validate your json with Json.NET Schema. See http://www.newtonsoft.com/jsonschema

#### Input

| Property        | Type     | Description                         | Example                 |
|-----------------|----------|-------------------------------------|-------------------------|
| Json            | dynamic ( JToken / string )   | Json to be validated                | `{"key":"value"}`        |
| JsonSchema      | string                        | JsonSchema to use for validation    | `{"type": "object", "properties": {"key": {"type":"string"} } }`  |

#### Result

| Property        | Type         | Description                         |
|-----------------|--------------|-------------------------------------|
| IsValid         | bool         | Indicates if the json is valid      |
| Errors          | List<string> | List of error messages for the validation    |


### Json.ConvertXmlStringToJToken

Convert an XML string to JToken

#### Input

| Property        | Type     | Description                         | 
|-----------------|----------|-------------------------------------|
| xml             | string   |                                     |


#### Result
JToken

### Json.ConvertJsonStringToJToken

Convert an Json string to JToken

#### Input

| Property        | Type     | Description                          |
|-----------------|----------|--------------------------------------|
| json            | string   |                                      | 

#### Result
JToken

## License
This project is licensed under the MIT License - see the LICENSE file for details
