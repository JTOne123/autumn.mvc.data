#!/bin/sh
echo '--------------------------------------------'
echo ' source generation C# - rsql                ' 
echo '--------------------------------------------'
java -jar antlr-4.6-complete.jar Rsql.g4 -o ../../src/WebApplication1/Commons -Dlanguage=CSharp -listener -encoding UTF-8 -visitor -long-messages -package WebApplication1.Commons


