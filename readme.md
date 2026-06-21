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

## Future Vision: The Cross-Shell Orchestrator

The ultimate goal of this project is to evolve beyond a simple PowerShell automation tool into a robust **Cross-Shell Orchestration Engine**. 

In the modern development landscape, engineers frequently switch between Windows PowerShell, Linux Bash, and other terminal environments. Our vision is to bridge the gap between these isolated silos using **Windows Terminal** as the central hub.

### The Core Concept
We are building a controller that treats different terminal tabs and panes as interconnected nodes. Imagine the following workflow orchestrated by PSController:

*   **Inter-Process Communication:** Monitoring an event or a specific output log in **Tab A (PowerShell)** and automatically triggering a corresponding command in **Tab B (Ubuntu/Bash)**.
*   **Unified Macro Automation:** Implementing a "Tera Term-style" macro language that isn't confined to a single shell, but orchestrates a series of tasks across multiple operating systems and environments.
*   **Seamless Integration:** By leveraging the `wt` (Windows Terminal) command-line interface, PSController will dynamically spawn, manage, and communicate across tabs and panes, creating a cohesive, automated developer workspace.

By combining the power of modern terminal features with the reliability of classical macro-based automation, we aim to redefine how developers bridge the gap between Windows and Linux environments.

## Support for Development

This project is maintained for the purpose of stable system automation. 
If you find this tool useful and would like to support its continued development, 
you can contribute via the following platforms:

* [GitHub Sponsors](https://github.com/sponsors/ShigeruTakimoto0107)
* [Buy Me a Coffee](https://www.buymeacoffee.com/shigerutakimoto0107)

Please note that these contributions are intended to support development, 
not for special access or functional privileges.


## Security and Disclaimer

PSController executes arbitrary PowerShell commands.

- Always verify the content before executing macros provided by third parties.
- Perform sufficient testing in a staging environment before production deployment.
- This software is provided "AS IS". Use it at your own risk and judgment.