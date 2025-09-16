#!/usr/bin/env node
"use strict";

var defaultCoreVersion = "Net80";
var supportedCoreVersions = [
    { ver: '8.0', dir: "Net80", },
    { ver: '9.0', dir: "Net90", },
];

const path = require('path');
var c = require('child_process');

// Initialize
process.title = 'nswag';
console.log("NSwag NPM CLI");

let args = process.argv.slice(2);

// Legacy support
args = args.map(arg => arg === '--x86' ? '/runtime:WinX86' : arg);
args = args.map(arg => arg === '--core' ? '/runtime:' + defaultCoreVersion : arg);
args = args.map(arg => arg === '--core 8.0' ? '/runtime:Net80' : arg);
args = args.map(arg => arg === '--core 9.0' ? '/runtime:Net90' : arg);

// Remove /runtime:* parameter from args, but remember its value
let runtimeIndices = [];
let runtimeValue = null;
args.forEach((arg, idx) => {
    const match = arg.match(/^\/runtime:(.+)$/i);
    if (match) {
        runtimeIndices.push(idx);
        runtimeValue = match[1];
    }
});

if (runtimeIndices.length > 1) {
    console.error("Error: Multiple /runtime:* arguments detected. Please specify only one. Maybe remove the legacy --core argument?");
    process.exit(1);
}

if (runtimeValue) {
    if (runtimeValue.toLowerCase() === "netcore") {

        // detect latest installed NetCore Version
        console.log("Trying to detect latest installed NetCore Version.");
        var infoCmd = "dotnet";
        var infoArgs = ["--version"];

        try {
            var result = c.spawnSync(infoCmd, infoArgs, { encoding: 'utf8' });
            if (result.error) {
                throw result.error;
            }
            if (result.status !== 0) {
                throw new Error(result.stderr ? result.stderr.toString() : "Unknown error");
            }
            var coreVersion = result.stdout.trim();
            const version = supportedCoreVersions.find(v => coreVersion.startsWith(v.ver));
            if (!version) {
                console.error("Error: Detected .NET Core version '" + coreVersion + "' is not supported.");
                process.exit(1);
            }
            console.log("Using supported .NET Core version: " + version.dir);
            runtimeValue = version.dir;
        } catch (error) {
            console.error("Error: Could not detect .NET Core version.");
            console.debug(error);
            process.exit(1);
        }
    }

    if(!runtimeValue.toLowerCase().startsWith("win")) {
        const isSupported = supportedCoreVersions.some(v => v.dir.toLowerCase() === runtimeValue.toLowerCase());
        if (!isSupported) {
            console.error("Error: Unsupported /runtime: argument '" + runtimeValue + "'.");
            process.exit(1);
        }
    }
}

var hasFullDotNet = false;
if (runtimeValue && runtimeValue.toLowerCase().startsWith("win")) {
    // Search for full .NET installation
    var fs = require('fs');

    if (process.env["windir"]) {

        try {
            var stats = fs.lstatSync(process.env["windir"] + '/Microsoft.NET');
            if (stats.isDirectory())
            {
                hasFullDotNet = true;
            }
        }
        catch (e) {
            console.warn("Could not verify the presence of the full .NET Framework installation.");
        }
    }
}

let childResult = null;

if (hasFullDotNet && runtimeValue && runtimeValue.toLowerCase().startsWith("win")) {
    
    // Run full .NET version
    if (runtimeValue.toLowerCase() === "winx86") {
        var exePath = path.join(__dirname, 'binaries', 'Win', 'nswag.x86.exe');
    } else {
        var exePath = path.join(__dirname, 'binaries', 'Win', 'nswag.exe');
    }

    try {
        childResult = c.spawnSync(exePath, args, { stdio: 'inherit' });
    } catch (error) {
        if (error.status !== undefined) {
            process.exit(error.status);
        } else {
            console.error(error);
            process.exit(1);
        }
    }
} else {

    // No Runtime specified or full .Net Installation not found, run default command
    if (!runtimeValue || runtimeValue.toLowerCase().startsWith("win")) {
        runtimeValue = defaultCoreVersion;
    }

    console.log("Using runtime: " + runtimeValue);

    var dllPath = path.join(__dirname, 'binaries', runtimeValue, 'dotnet-nswag.dll');
    var spawnArgs = [dllPath].concat(args);

    try {
        childResult = c.spawnSync('dotnet', spawnArgs, { stdio: 'inherit' });
    } catch (error) {
        if (typeof error.status === 'number') {
            process.exit(error.status);
        } else {
            console.error(error);
            process.exit(1);
        }
    }
}

// Exit with the same exit code as the child process
if (childResult && typeof childResult.status === 'number') {
    process.exit(childResult.status);
} else if (childResult) {
    // If status is undefined, exit with code 1 to indicate an error
    process.exit(1);
}
// End of script
