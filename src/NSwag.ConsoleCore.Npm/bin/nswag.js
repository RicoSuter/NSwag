#!/usr/bin/env node
"use strict";

process.title = 'nswag';

console.log("NSwag NPM module (experimental, currently requires installation of .NET Core)");
var args = process.argv.splice(2, process.argv.length - 2).join(" ");

// TODO: Try to run "full" version first, if this fails (i.e. full .NET not installed), run .NET Core version

// Run .NET Core version
var cmd = 'dotnet "' + __dirname + '/binaries/core/nswag.dll" ' + args;
const execSync = require('child_process').execSync;
var code = execSync(cmd, {stdio:[0,1,2]});