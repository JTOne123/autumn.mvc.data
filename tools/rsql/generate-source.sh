#!/bin/sh
echo '--------------------------------------------'
echo ' source generation C# - rsql                ' 
echo '--------------------------------------------'
java -jar antlr-4.6-complete.jar Query.g -o ../../src/Autumn.Mvc.Data/Models/Queries -Dlanguage=CSharp -listener -encoding UTF-8 -visitor -package Autumn.Mvc.Data.Models.Queries


