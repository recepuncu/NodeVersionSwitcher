# Node Version Switcher

🚀 **Support the Project!**  
If you find Node Version Switcher useful and want to see more features, updates, and support for macOS/Linux in the future, consider [sponsoring me on GitHub](https://github.com/sponsors/recepuncu). Your contributions help keep this project alive and growing! ❤️  

---
![image](https://github.com/user-attachments/assets/b98cccb9-b76e-48cd-9686-ac886901eb7d)

Easily manage and switch between multiple Node.js versions using this lightweight tray application.  

This application integrates with **NVM (Node Version Manager)** to provide a user-friendly interface for managing Node.js versions directly from your system tray. Switch Node.js versions on the fly, view your installed versions, and set your desired version with just a few clicks.  

## Features  
- List and switch between installed Node.js versions via NVM.  
- Automatically detects and integrates with your existing NVM installation.  
- Intuitive system tray icon for quick access.  
- Option to start the application at system startup.  
- Lightweight and easy to use.  
- **New UI Components:**  
  - Added `NodeVersionsForm` for displaying and installing Node.js versions with a filter box, data grid, and progress bar.

## Recent Updates  
### Refactor and Add UI for Node.js Version Management  
**Significant refactor and modularization of the codebase:**  
- Removed `NodeVersionSwitcherContext.cs` and split functionalities into multiple helper classes and services.  
- Added new helper classes for HTTP requests, notifications, Node.js version management, regular expressions, registry interactions, startup management, symbolic link creation, and system information retrieval.  
- Introduced `NodeVersionInfo` model and `INodeVersionDownloader` interface.  
- Updated `NodeVersionSwitcher.csproj` with new properties for versioning and language.  
- Added `FolderProfile.pubxml` for project publishing.  

## Requirements  
- **NVM for Windows** installed on your system.  

## Getting Started  
1. Download the latest release from the [Releases](https://github.com/recepuncu/NodeVersionSwitcher/releases) page.  
2. Install the application using the setup file.  
3. Launch the application and access it from the system tray.  

## License  
This project is licensed under the [MIT License](LICENSE).
