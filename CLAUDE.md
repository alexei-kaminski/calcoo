# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Calcoo is a WPF scientific calculator for Windows, written in C# targeting .NET 10 (`net10.0-windows`). It supports both RPN (Reverse Polish Notation) and Algebraic calculation modes, similar to classic HP calculators.

## Build and Test Commands

```bash
# Build the solution
dotnet build Calcoo.sln

# Run all tests
dotnet test Calcoo.Test/Calcoo.Test.csproj
```

The solution file is `Calcoo.sln` containing two projects: `Calcoo` (WinExe) and `Calcoo.Test` (test library).

## Architecture

The app follows an MVC-like pattern with these core components:

- **Cpu** (`Cpu.cs`) — The computation engine. Executes all calculator operations (arithmetic, trig, logarithmic, memory, stack manipulation). Implements `ICpuOutput` interface. Supports cloning for undo/redo.
- **CpuStack** (`CpuStack.cs`) — Stack backing the CPU. Two modes: `Xyzt` (fixed 4-level HP-style) and `Infinite`. Handles parentheses nesting for algebraic mode. Implements `ICpuStackGetters`.
- **DoubleByDigit** (`DoubleByDigit.cs`) — Manages digit-by-digit numeric input (integer part, fractional part, exponent, signs). Converts accumulated digits to `double`. Implements `IDoubleByDigitGetters`.
- **Body** (`Body.cs`) — Internal UI controller. Bridges CPU state to WPF display elements. Manages button collections, keyboard shortcut translation, Arc/Hyp toggle state, and display refresh.
- **MainWindow** (`MainWindow.xaml.cs`) — WPF window and orchestrator. Owns the undo/redo stacks (`LinkedList<Cpu>`, max 200 entries). Routes button clicks and keyboard input through Body to CPU.
- **Command** (`Command.cs`) — Enum of 76+ calculator commands with extension methods for mode-specific availability and trig function transformations.
- **Settings** (`Settings.cs`) — Configuration enums (Mode, StackMode, AngleUnits, DisplayFormat, etc.) persisted to Windows Registry.
- **MathUtil** (`MathUtil.cs`) — Factorial (Stirling + recursive), specialized trig/hyperbolic implementations with Taylor series.
- **TextUtil** (`TextUtil.cs`) — Number formatting for display and clipboard parsing (locale-aware and heuristic modes).
- **Display classes** (`NumberDisplay.cs`, `OperationDisplay.cs`, `IndicatorDisplay.cs`, `LabelDisplay.cs`, `BaseDisplay.cs`) — UI rendering components for calculator display elements.

## Data Flow

User input → `MainWindow` → `Body.ButtonPressed()` → `Cpu` executes command → `Body` refreshes displays from CPU state. Undo/redo works by cloning the entire `Cpu` object before each operation.

## Testing

- **Framework:** NUnit 4.4 with `nunit.framework.legacy` for `ClassicAssert` syntax
- **Test files:** `CpuTest.cs`, `CpuStackTest.cs`, `DoubleByDigitTest.cs`, `MathUtilTest.cs`, `TextUtilTest.cs`, `CommandTest.cs`
- **68 test methods** covering core computation logic
- Tests use `[TestFixture]` and `[Test]` attributes

## Key Design Details

- All source lives in the `Calcoo` namespace (tests in `Calcoo.Test`)
- CPU precision constant: `1e-15`
- Two calculation modes affect command availability: RPN-only commands (Enter, RotateUp, RotateDown, ExchXY) and Alg-only commands (OpenBracket, CloseBracket, Equals)
- Vector icon resources are in `Resources/Icons/` (XAML format)
- SDK-style .csproj format with PackageReference
