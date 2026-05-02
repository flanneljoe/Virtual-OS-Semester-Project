[![Open in Visual Studio Code](https://classroom.github.com/assets/open-in-vscode-2e0aaae1b6195c2367325f4f02e2d04e9abb55f0b24a779b69b11b9e10269abc.svg)](https://classroom.github.com/online_ide?assignment_repo_id=22186168&assignment_repo_type=AssignmentRepo)
# MidOS Project
## Course: CSCI 480 - Spring 2026

# Introduction
Spring 2026 CSCI 480 MidOS semester Project by Joe Feltz. 

This project is written in C# and intended to run with .NET version 9.0.

The project is most easily viewed in Visual Studio, or Visual Studio Code.

# Project Configuration
The file `osconfig.json` holds configuration settings to be used by the operating system when running.

The project attempts to get the configuration settings at runtime from `osconfig.json`.

When running the project from an executable, `osconfig.json` is expected to be in the root directory where the executable is being run from.

If `osconfig.json` is not present in the root directory, then internal fallback values are used.

# Bulding the Project
The project can easily be built in Visual Studio, using the build command. `Ctrl+Shift+B` by default.

On the command line, the project can be built with:
`dotnet clean`
`dotnet build`

# Running the Project
The project can be run on the command line using the executable, or using dotnet.

## Using the Project Executable
The project builds to an executable named `MidOS.exe`, which can be run using:
`MidOS.exe --page <Size in bytes of a virtual memory page> <Path to MidOS assemby file> <Path to MidOS assemby file> ...`

`--page <Size in bytes of a virtual memory page>` is an optional first parameter pair, which allows for specifying the size in bytes to be used for a vitual memory page. This value can be different from, and supercedes, the value in `osconfig.json`.

`<Path to MidOS assembly file>` are parameters that represent paths to files containing valid MidOS assembly language files. As many paths as desired can be passed, though at least one path is required to be passed to the system.

## Using Dotnet
The project can also be run from the project root directory using the following command:
`dotnet run --page <size in bytes of a virtual memory page> <Path to MidOS assemby file> <Path to MidOS assemby file> ...`

Just as with the executable, `--page <Size in bytes of a virtual memory page>` is an optional first parameter pair for the size of virtual memory pages. Additionally, at least one MidOS assembly file must still be passed to the system.

## Running Test Files
To run the project's test files, the following commands can be used:
`MidOS.exe --test`
`dotnet run --test`

This will cause all 18 test files to run.

Alternativly, to one of the project's test files the following commands can be used:

`MidOS.exe --test <test number 1-18>`
`dotnet run --test <test number 1-18>`

These commands run the project using test files, where a number in the range [1, 18] represents the test to run.


Just as when running the project outside of test mode, `osconfig.json` is expected to be in the root directory for the solution, or in the root directory where the executable is being run, else default values will be used.
Moreover, the test files rely on their relative paths to their MidOS assemly files to remain the same. I do not recommend running the tests from the executable from outside the project's root directory.