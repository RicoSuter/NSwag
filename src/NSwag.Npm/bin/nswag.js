#!/usr/bin/env node
"use strict";

process.title = 'nswag';

console.log("NSwag NPM CLI");
var args = process.argv.splice(2, process.argv.length - 2).join(" ");

// Search for full .NET installation
var hasFullDotNet = false; 
var fs = require('fs');
if (process.env["windir"]) {
    try {
	    var stats = fs.lstatSync(process.env["windir"] + '/Microsoft.NET');
        if (stats.isDirectory())
    		hasFullDotNet = true; 
    }
    catch (e) {
	    console.log(e);
    }
}

if (hasFullDotNet) {
    // Run full .NET version
    var cmd = '"' + __dirname + '/binaries/full/nswag.exe" ' + args;
    const execSync = require('child_process').execSync;
    var code = execSync(cmd, {stdio:[0,1,2]});	
} else {
    // Run .NET Core version
    var cmd = 'dotnet "' + __dirname + '/binaries/core/dotnet-nswag.dll" ' + args;
    const execSync = require('child_process').execSync;
    var code = execSync(cmd, {stdio:[0,1,2]});	
}