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

var c = require('child_process');
if (hasFullDotNet) {
    // Run full .NET version
    var cmd = '"' + __dirname + '/binaries/full/nswag.exe" ' + args;
    var code = c.execSync(cmd, {stdio:[0,1,2]});	
} else {
    // Run .NET Core version
    var cmd = "dotnet";
    var dotnetVersion = c.exec(cmd, (error, stdout, stderr) => {
        var dotnetVersion = stdout; 
        if (dotnetVersion.indexOf("Version  : 1.0.0") !== -1)
            cmd = 'dotnet "' + __dirname + '/binaries/netcoreapp1.0/dotnet-nswag.dll" ' + args;
        else if (dotnetVersion.indexOf("Version  : 1.1.0") !== -1)
            cmd = 'dotnet "' + __dirname + '/binaries/netcoreapp1.1/dotnet-nswag.dll" ' + args;

        c.execSync(cmd, {stdio:[0,1,2]});	
    });
}