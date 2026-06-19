# PSController

PowerShell Automation Engine for Windows Standard Environments

PSController is a lightweight macro engine designed to automate PowerShell with the operational feel of Tera Term Macro. It eliminates the need for additional software installation by leveraging the native PowerShell environment already present in Windows, enabling robust automation and operational tasks.

## Why PSController?

In many enterprise environments, the introduction of third-party tools is restricted by security policies. PSController utilizes native Windows PowerShell to enable automation even under such restrictive conditions:

- Automation of routine tasks
- Server operation efficiency
- Windows service monitoring
- Browser automation

## Features

- Single EXE distribution
- Implemented in C# 5
- No external libraries required
- Focused on Windows standard environments
- Fail-Fast design

## Key Functions

- PowerShell automation
- Windows service monitoring and auto-recovery
- Browser automation (integrated with Microsoft Edge debugging features)

## Build Procedure

Build the application on a Windows environment using the following steps:

1. Double-click and execute `build.bat` located in the project root.
2. Upon completion, the console will display prompts for:
   - Verification of test macro execution.
   - Setting the file association between the program (`PowerShellController.exe`) and macro files (`.pscm`).
3. Enter "Y" for each prompt to complete the environment setup and file association.

- [Build Manual](docs/Build/Build.md)

## Execution

Executing `build.bat` will generate `PowerShellController.exe` within the `bin` folder.

## Documentation

- [Command Manual](docs/guide/macros.md)
- [Sample Manual: Browser Automation](docs/sample001/001.md)
- [Sample Manual: Service Monitoring](docs/sample002/002.md)

## Bug Reports and Feature Requests

Please use GitHub Issues.

## Contact

pscontroller.project@gmail.com

---

## Development Philosophy

PSController prioritizes the following principles:

- Utilize Windows standard environments
- Minimize external dependencies
- Distribute as a single EXE
- Maintain C# 5 compatibility
- Emphasize maintainability
- Uphold Fail-Fast principles

PSController aims to provide an environment where "PowerShell can be automated with the feel of Tera Term Macro."

## Security and Disclaimer

PSController executes arbitrary PowerShell commands.

- Always verify the content before executing macros provided by third parties.
- Perform sufficient testing in a staging environment before production deployment.
- This software is provided "AS IS". Use it at your own risk and judgment.