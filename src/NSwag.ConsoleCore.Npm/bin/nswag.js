#!/usr/bin/env node
"use strict";

process.title = 'nswag';

var args = process.argv.splice(2, process.argv.length - 2).join(" ");
var cmd = 'dotnet "' + __dirname + '/binaries/nswag.dll" ' + args;

console.log("NSwag NPM (requires installation of .NET Core)");

const execSync = require('child_process').execSync;
var code = execSync(cmd, {stdio:[0,1,2]});