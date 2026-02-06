# Configuration Editor (CLI Prototype)

This repository contains the logic implementation (Backend) for the Window Logger Configuration Editor.

Currently implemented as a secure CLI (Command Line Interface) tool to establish core functionality and data handling. This logic serves as the foundation for the upcoming WinForms GUI version.

## Project Status
- [x] **Core Logic:** Loading/Saving `appsettings.json`
- [x] **Data Models:** Structures for Applications, Exclusions, and Categories
- [x] **Security:** Safe file handling (no background processes)
- [ ] **GUI Layer:** WinForms implementation (In Progress)

## Features
- **Applications Management:** Add/Remove applications to track.
- **Exclusions:** Manage keywords to exclude from logging.
- **Categories:** Organize tracked windows into categories.
- **Safety First:** Validates input and ensures safe data serialization.

## How to run
1. Clone the repository.
2. Navigate to the project folder.
3. Run the following command:
   ```bash
   dotnet run

## Next Steps:
Porting the existing logic to a WinForms application to provide a user-friendly graphical interface (Icon-based).