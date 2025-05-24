# 🏗️ Beam Reinforcement Generation API (Tekla Structures)

This project implements an automated C# application for generating reinforcement in simply supported concrete beams using the Tekla Structures Open API. The tool is designed to automate the modeling of both longitudinal and stirrup reinforcement, streamlining BIM workflows and reducing manual modeling time for structural engineers.

---

## 🚀 Features

- 🏷️ **Automatic selection of beam elements** within the Tekla Structures model
- 🔗 **Placement of longitudinal rebars** at user-defined or code-compliant positions
- 🔁 **Generation and placement of stirrup (transverse) reinforcement** along the beam span
- ⚙️ **User-configurable parameters:** bar diameter, spacing, concrete cover, reinforcement class
- 📊 **Outputs reinforcement placement and quantities** in the BIM model
- 🛠️ **Direct BIM model modification** via the Tekla Open API

---

## 🛠️ How to Build & Run

**Requirements:**
- Tekla Structures 2020 or newer (with Open API access)
- Visual Studio 2022 or newer
- .NET Framework version compatible with your Tekla installation

**Steps:**
1. Clone this repository or download as ZIP
2. Open `ReinforcementSample.sln` in Visual Studio
3. Ensure Tekla Open API references are set (see project references)
4. Build the solution (`Ctrl+Shift+B`)
5. Open your Tekla Structures model
6. Run the program (`F5` or `Ctrl+F5`)
7. Reinforcement is automatically generated and placed in the model

---

## 📂 Project Structure

```text
📁 ReinforcementSample
├── Program.cs                  # Main logic: selection & reinforcement generation
├── ReinforcementSample.csproj
├── App.config                  # Optional: configuration settings
├── README.md
└── .gitignore
