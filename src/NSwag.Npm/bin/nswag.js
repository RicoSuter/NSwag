#!/usr/bin/env node
"use strict";

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
if (hasFullDotNet &&
    args.indexOf("--core") == -1 &&
    args.indexOf("--core 1.0") == -1 &&
    args.indexOf("--core 1.1") == -1 &&
    args.indexOf("--core 2.0") == -1) {
    // Run full .NET version
    if (args.indexOf("--x86") != -1) {
        var cmd = '"' + __dirname + '/binaries/full/nswag.x86.exe" ' + args;
        var code = c.execSync(cmd, { stdio: [0, 1, 2] });
    } else {
        var cmd = '"' + __dirname + '/binaries/full/nswag.exe" ' + args;
        var code = c.execSync(cmd, { stdio: [0, 1, 2] });
    }
} else {
    // Run .NET Core version
    var core10cmd = 'dotnet "' + __dirname + '/binaries/netcoreapp1.0/dotnet-nswag.dll" ' + args;
    var core11cmd = 'dotnet "' + __dirname + '/binaries/netcoreapp1.1/dotnet-nswag.dll" ' + args;
    var core20cmd = 'dotnet "' + __dirname + '/binaries/netcoreapp2.0/dotnet-nswag.dll" ' + args;
    var defaultCmd = core11cmd;

    if (args.indexOf("--core 1.0") != -1)
        c.execSync(core10cmd, { stdio: [0, 1, 2] });
    else if (args.indexOf("--core 1.1") != -1)
        c.execSync(core11cmd, { stdio: [0, 1, 2] });
    else if (args.indexOf("--core 2.0") != -1)
        c.execSync(core20cmd, { stdio: [0, 1, 2] });
    else {
        var cmd = "dotnet --version";
        c.exec(cmd, (error, stdout, stderr) => {
            if (!error) {
                var dotnetVersion = stdout;
                if (dotnetVersion.indexOf("2.0.0") !== -1)
                    c.execSync(core20cmd, { stdio: [0, 1, 2] });
                else if (dotnetVersion.indexOf("1.1.0") !== -1)
                    c.execSync(core11cmd, { stdio: [0, 1, 2] });
                else if (dotnetVersion.indexOf("1.0.0") !== -1)
                    c.execSync(core10cmd, { stdio: [0, 1, 2] });
                else // default
                    c.execSync(defaultCmd, { stdio: [0, 1, 2] });
            } else // default
                c.execSync(defaultCmd, { stdio: [0, 1, 2] });
        });
    }
}