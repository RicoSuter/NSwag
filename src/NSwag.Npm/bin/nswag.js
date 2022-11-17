#!/usr/bin/env node
"use strict";

var defaultCoreVersion = "Net70";
var supportedCoreVersions = [
    { ver: '2.1', dir: "NetCore21", },
    { ver: '2.2', dir: "NetCore22", },
    { ver: '3.0', dir: "NetCore30", },
    { ver: '3.1', dir: "NetCore31", },
    { ver: '5.0', dir: "Net50", },
    { ver: '6.0', dir: "Net60", },
    { ver: '7.0', dir: "Net70", },
];

// Initialize
process.title = 'nswag';
console.log("NSwag NPM CLI");
var args = process.argv.splice(2, process.argv.length - 2).map(function (a) { return a.indexOf(" ") === -1 ? a : '"' + a + '"' }).join(" ");

// Legacy support
args = args.replace("--x86", "/runtime:WinX86");
args = args.replace("/runtime:x86", "/runtime:WinX86");
args = args.replace("--core 2.1", "/runtime:NetCore21");
args = args.replace("--core 3.1", "/runtime:NetCore31");
args = args.replace("--core 5.0", "/runtime:Net50");
args = args.replace("--core 6.0", "/runtime:Net60");
args = args.replace("--core 7.0", "/runtime:Net70");
args = args.replace("--core", "/runtime:" + defaultCoreVersion);

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
    var defaultCmd = 'dotnet "' + __dirname + '/binaries/' + defaultCoreVersion + '/dotnet-nswag.dll" ' + args;
    var infoCmd = "dotnet --version";
    c.exec(infoCmd, (error, stdout, _stderr) => {
        for (let version of supportedCoreVersions) {
            var coreCmd = 'dotnet "' + __dirname + '/binaries/' + version.dir + '/dotnet-nswag.dll" ' + args;

            if (args.toLowerCase().indexOf("/runtime:" + version.dir.toLocaleLowerCase()) != -1) {
                c.execSync(coreCmd, { stdio: [0, 1, 2] });
                return;
            } else {
                if (!error) {
                    var coreVersion = stdout;

                    if (coreVersion.startsWith(version.ver)) {
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
