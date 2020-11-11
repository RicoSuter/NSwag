#!/usr/bin/env node
"use strict";

var defaultCoreVersion = "Core21";
var supportedCoreVersions = ["Core21", "Core22", "Core30", "Core31", "50"];

// Initialize
process.title = 'nswag';
console.log("NSwag NPM CLI");
var args = process.argv.splice(2, process.argv.length - 2).map(function (a) { return a.indexOf(" ") === -1 ? a : '"' + a + '"' }).join(" ");

// Legacy support
args = args.replace("--x86", "/runtime:WinX86");
args = args.replace("/runtime:x86", "/runtime:WinX86");
args = args.replace("--core 2.1", "/runtime:NetCore21");
args = args.replace("--core 2.2", "/runtime:NetCore22");
args = args.replace("--core 3.0", "/runtime:NetCore30");
args = args.replace("--core 3.1", "/runtime:NetCore31");
args = args.replace("--core 5.0", "/runtime:Net50");
args = args.replace("--core", "/runtime:Net" + defaultCoreVersion);

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
if (hasFullDotNet && args.toLowerCase().indexOf("/runtime:win") != -1) {
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
    var defaultCmd = 'dotnet "' + __dirname + '/binaries/Net' + defaultCoreVersion + '/dotnet-nswag.dll" ' + args;
    var infoCmd = "dotnet --version";
    c.exec(infoCmd, (error, stdout, stderr) => {
        for (let version of supportedCoreVersions) {
            var coreCmd = 'dotnet "' + __dirname + '/binaries/Net' + version + '/dotnet-nswag.dll" ' + args;

            if (args.toLowerCase().indexOf("/runtime:net" + version.toLocaleLowerCase()) != -1) {
                c.execSync(coreCmd, { stdio: [0, 1, 2] });
                return;
            } else {
                if (!error) {
                    var coreVersion = stdout;
                    if (coreVersion.indexOf(version.replace('Core', '') + ".0") !== -1) {
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
