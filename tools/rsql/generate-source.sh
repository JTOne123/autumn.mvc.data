#!/bin/sh
echo '--------------------------------------------'
echo ' source generation C# - rsql                ' 
echo '--------------------------------------------'
java -jar antlr-4.6-complete.jar Rsql.g -o ../../src/Autumn.Data.Rest/Queries -Dlanguage=CSharp -listener -encoding UTF-8 -visitor -package Autumn.Data.Rest.Queries


