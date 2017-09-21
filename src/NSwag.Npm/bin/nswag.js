#!/usr/bin/env node
"use strict";

var defaultCoreVersion = "11";
var supportedCoreVersions = ["10", "11", "20"];

// Initialize
process.title = 'nswag';
console.log("NSwag NPM CLI");
var args = process.argv.splice(2, process.argv.length - 2).map(function (a) { return a.indexOf(" ") === -1 ? a : '"' + a + '"' }).join(" ");

// Legacy support
args = args.replace("--x86", "/runtime:WinX86");
args = args.replace("/runtime:x86", "/runtime:WinX86");
args = args.replace("--core 1.0", "/runtime:NetCore10");
args = args.replace("--core 1.1", "/runtime:NetCore11");
args = args.replace("--core 2.0", "/runtime:NetCore20");
args = args.replace("--core", "/runtime:NetCore" + defaultCoreVersion);

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
if (hasFullDotNet && args.toLowerCase().indexOf("/runtime:netcore") == -1) {
    // Run full .NET version
    if (args.toLowerCase().indexOf("/runtime:winx86") != -1) {
        var cmd = '"' + __dirname + '/binaries/Win/nswag.x86.exe" ' + args;
        var code = c.execSync(cmd, { stdio: [0, 1, 2] });
    } else {
        var cmd = '"' + __dirname + '/binaries/Win/nswag.exe" ' + args;
        var code = c.execSync(cmd, { stdio: [0, 1, 2] });
    }
} else {
    // Run .NET Core version
    var defaultCmd = 'dotnet "' + __dirname + '/binaries/NetCore' + defaultCoreVersion + '/dotnet-nswag.dll" ' + args;
    var infoCmd = "dotnet --version";
    c.exec(infoCmd, (error, stdout, stderr) => {
        for (let version of supportedCoreVersions) {
            var coreCmd = 'dotnet "' + __dirname + '/binaries/NetCore' + version + '/dotnet-nswag.dll" ' + args;

            if (args.toLowerCase().indexOf("/runtime:netcore" + version) != -1) {
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