[TOC]

# Task documentation #

## Json.Query ##

Query a json string / json token using JSONPath query.

### Input ###

| Property        | Type                          | Description                  | Example                  |
|-----------------|-------------------------------|------------------------------|--------------------------|
| Json            | dynamic ( JToken / string )   | Json for the query.          | `{"key":"value"}`        |
| Query           | string                        | Query that uses JSONPath syntax. See http://goessner.net/articles/JsonPath/ for details | `$.store.book[0].title` `$['store']['book'][0]['title']`  |

### Options ###

| Property                | Type           | Description                                    |
|-------------------------|----------------|------------------------------------------------|
| ErrorWhenNotMatched     | bool           | A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression. |

### Result ###
Array [ JToken ]

## Json.QuerySingle ##

Query a json string / json token for a single result

### Input ###

| Property        | Type                          | Description                  | Example                  |
|-----------------|-------------------------------|------------------------------|--------------------------|
| Json            | dynamic ( JToken / string )   | Json for the query.          | `{"key":"value"}`        |
| Query           | string                        | Query that uses JSONPath syntax. See http://goessner.net/articles/JsonPath/ for details | `$.store.book[0].title` `$['store']['book'][0]['title']`  |

### Options ###

| Property                | Type           | Description                                    |
|-------------------------|----------------|------------------------------------------------|
| ErrorWhenNotMatched     | bool           | A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression. |

### Result ###
JToken

## Json.Handlebars ##

Handlebars provides the power necessary to let you build semantic templates effectively with no frustration. See https://github.com/rexm/Handlebars.Net

### Input ###

| Property          | Type                          | Description                         | Example                                |
|-------------------|-------------------------------|-------------------------------------|----------------------------------------|
| Json              | dynamic ( JToken / string )                   | Object used to fill the template                                                                          | `{"title":"mr.", "name":"foo"}`        |
| HandlebarTemplate | string                                        | Template for handlebars. This needs to be in expression mode. Using {{ }} in other modes breaks the task. | `<xml> {{title}} {{> strongName}} </xml>`                 |
| HandlebarPartials | Array{string TemplateName, string Template}   | Partials for a handlebars template. A partial is inicated by adding a {{'>' templateName }} inside the parent HandlebarTemplate  | `TemplateName: "strongName", Template: "<strong>{{name}}</strong>"`   |

### Result ###
string

## Json.Validate ##

Validate your json with Json.NET Schema. See http://www.newtonsoft.com/jsonschema

### Input ###

| Property        | Type     | Description                         | Example                 |
|-----------------|----------|-------------------------------------|-------------------------|
| Json            | dynamic ( JToken / string )   | Json to be validated                | `{"key":"value"}`        |
| JsonSchema      | string                        | JsonSchema to use for validation    | `{"type": "object", "properties": {"key": {"type":"string"} } }`  |

### Result ###

| Property        | Type         | Description                         |
|-----------------|--------------|-------------------------------------|
| IsValid         | bool         | Indicates if the json is valid      |
| Errors          | List<string> | List of error messages for the validation    |


## Json.ConvertXmlStringToJToken ##

Convert an XML string to JToken

### Input ###

| Property        | Type     | Description                         | 
|-----------------|----------|-------------------------------------|
| xml             | string   |                                     |


### Result ###
JToken

## Json.ConvertJsonStringToJToken ##

Convert an Json string to JToken

### Input ###

| Property        | Type     | Description                          |
|-----------------|----------|--------------------------------------|
| json            | string   |                                      | 

### Result ###
JToken