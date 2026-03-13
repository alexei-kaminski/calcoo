# Calcoo

Calcoo is an RPN and algebraic scientific calculator designed to provide maximum usability.

![Calcoo in algebraic mode](screenshot-algebraic.png?v=2) ![Calcoo in RPN mode](screenshot-rpn.png?v=2) ![Calcoo settings](screenshot-settings.png?v=2)

## Features

- RPN (Reverse Polish Notation) and algebraic calculation modes
- Trigonometric, logarithmic, and hyperbolic functions with arc/hyp modifiers
- Factorial, power, and root operations
- Two memory registers with store, recall, add, and swap
- Undo/redo (up to 200 steps)
- Degree and radian angle modes
- Multiple display formats (fixed, scientific, engineering)
- Copy/paste with locale-aware number parsing
- Customizable command button
- Keyboard shortcuts for all operations
- Scalable vector UI that maintains aspect ratio
- Dark/light theme following Windows system setting

## Requirements

- Windows 10 or later
- [.NET 9 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/9.0)

## Build

```bash
dotnet build Calcoo.sln
```

## Test

```bash
dotnet test Calcoo.Test/Calcoo.Test.csproj
```

## Installer

```bash
dotnet publish Calcoo -c Release -r win-x64 --no-self-contained
```

Requires [Inno Setup 6](https://jrsoftware.org/isinfo.php). Output: `Calcoo.Setup/Release/Calcoo.Setup.exe`

The installer is not required to compile and run Calcoo.

## License

GPL-3.0-or-later. See the License tab in the application for details.

## Miscellanea

Icon Font: Arian Rounded MT Bold

## Previous Versions

Calcoo 2 was written in Java, and is available at https://calcoo.sourceforge.net/.
