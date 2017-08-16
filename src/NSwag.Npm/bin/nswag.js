#!/usr/bin/env node
"use strict";

var defaultCoreVersion = "1.1";
var supportedCoreVersions = ["1.0", "1.1", "2.0"];

// Initialize
process.title = 'nswag';
console.log("NSwag NPM CLI");
var args = process.argv.splice(2, process.argv.length - 2).map(function (a) { return a.indexOf(" ") === -1 ? a : '"' + a + '"' }).join(" ");

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
if (hasFullDotNet && args.indexOf("--core") == -1 && args.indexOf("/runtime:core") == -1) {
    // Run full .NET version
    if (args.indexOf("--x86") != -1 || args.toLowerCase().indexOf("/runtime:x86") != -1) {
        var cmd = '"' + __dirname + '/binaries/full/nswag.x86.exe" ' + args;
        var code = c.execSync(cmd, { stdio: [0, 1, 2] });
    } else {
        var cmd = '"' + __dirname + '/binaries/full/nswag.exe" ' + args;
        var code = c.execSync(cmd, { stdio: [0, 1, 2] });
    }
} else {
    // Run .NET Core version
    var defaultCmd = 'dotnet "' + __dirname + '/binaries/netcoreapp' + defaultCoreVersion + '/dotnet-nswag.dll" ' + args;
    var infoCmd = "dotnet --version";
    c.exec(infoCmd, (error, stdout, stderr) => {
        for (let version of supportedCoreVersions) {
            var coreCmd = 'dotnet "' + __dirname + '/binaries/netcoreapp' + version + '/dotnet-nswag.dll" ' + args;

            if (args.indexOf("--core " + version) != -1 || args.indexOf("/runtime:core" + version) != -1) {
                c.execSync(coreCmd, { stdio: [0, 1, 2] });
                return;
            } else {
                if (!error) {
                    var coreVersion = stdout;
                    if (coreVersion.indexOf(version + ".0") !== -1) {
                        c.execSync(coreCmd, { stdio: [0, 1, 2] });
                        return;
                    }
                }
            }
        }
        c.execSync(defaultCmd, { stdio: [0, 1, 2] });
        return;
    });
}