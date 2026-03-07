# Code Review — Round 2

## Bugs

### 1. ~~Closing SettingsDialog via X button applies spurious changes~~ (FIXED)
- `SettingsDialog.xaml.cs:85-86,96-103` / `MainWindow.xaml.cs:241`
- The `RoundingDigitsComboBox_SelectionChanged` handler fired during construction when `SelectedItem` was set, which set `WasChanged = true`. The Cancel button resets this flag, but closing via the window's X button did not. MainWindow then saw `WasChanged == true` and applied/saved unchanged settings.
- **Fix:** Added `_initialized` flag to skip `WasChanged = true` in `SelectionChanged` during construction.

### 2. Custom command sequence doesn't stop on CPU error
- `MainWindow.xaml.cs:324-329`
- When executing a custom command sequence, each command runs in a loop with no check for overflow/error between commands. If the first command causes an error (e.g., `1/0`), subsequent commands continue executing against the error state, producing confusing results.

### 3. Trig zero-detection heuristic fragile for large angles
- `Cpu.cs:924-931` (sin), `949` (cos), `976,980` (tan)
- The special-case check `Math.Abs(X % Math.PI) < CpuPrecision * Math.Abs(X)` uses a relative epsilon that scales with X. For very large exact multiples of 180 degrees, the modulo residual can exceed the threshold, so sin/cos/tan won't return exact zero as intended.

### 4. Duplicate test data in TextUtilTest instead of exponent test cases
- `TextUtilTest.cs:163-183`
- The comment says "with decimal point and exponent" but the test cases are exact duplicates of lines 142-161 (the "with decimal point" section). The intended exponent-combination tests are missing, leaving a coverage gap.

## Warnings

### 5. ~~`DoBinaryOpChain` can crash when stack becomes empty in paren-close path~~ (DOWNGRADED to Suggestion)
- `Cpu.cs:711-718`
- `HeadParenExists()` throws on empty stack, but `ExecuteRightParen` always guards with `ExistOpenParen()` first, ensuring a paren-holding element is always found before the stack empties. Not actually triggerable. Regression test added (`CloseParenWithDeepStackDoesNotCrash`).

### 6. `HeadParenAdd` has no empty-stack guard
- `CpuStack.cs:213`
- `_stack.First().NumberOfParens++` will throw `InvalidOperationException` if the stack is empty. Currently protected by caller logic, but fragile.

### 7. `Pow` is left-associative instead of right-associative
- `Cpu.cs:665-668`
- The `>=` comparison in `BinaryOpPriority` evaluation makes equal-priority operators left-associative. For `+`, `-`, `*`, `/` this is correct, but `2^3^4` evaluates as `(2^3)^4 = 4096` instead of the mathematical convention `2^(3^4)`. May be intentional to match physical calculator behavior.

### 8. `WasChanged` set true even when clicking already-selected option
- `SettingsDialog.xaml.cs:109,150`
- Any RadioButton/CheckBox click sets `WasChanged = true`, even when the value doesn't actually change. Causes unnecessary registry writes.

### 9. `_mainDisplayContent` could be null
- `Body.cs:87`
- `GetMainDisplayString()` calls `_mainDisplayContent.ToString()` but `_mainDisplayContent` is initialized to null. Currently protected by the catch-all in Copy and by `Refresh()` running during construction, but fragile.

### 10. `NumberDisplay.Show()` pushes nullable `_error` without null guard
- `NumberDisplay.cs:110`
- When `_hasError` is true, `_error` (declared `DisplayGlyph?`) is pushed onto `ShownGlyphs`. Currently safe because the constructor keeps `_hasError` and `_error` in sync, but these are independent fields — a refactor could introduce a NullReferenceException.

### 11. `_numBase` vs hardcoded `10` inconsistency
- `Cpu.cs:656` vs `793,799,874`
- `ExecuteBinaryOp` uses hardcoded `10` for `_input.ToDouble(10)` instead of `_numBase`, while some other methods use `_numBase`. If non-base-10 modes were ever supported, binary/unary operations would use the wrong base.

## Suggestions

### 12. `FactJr` recursion has no depth guard
- `MathUtil.cs:43-58`
- Recursion depth equals `floor(x)` with no explicit limit. Mitigated by the Stirling branch for large values, but fragile if thresholds change.

### 13. All exceptions silently swallowed in Copy/Paste
- `MainWindow.xaml.cs:291,312`
- `catch { }` blocks swallow all exceptions including unexpected ones, making debugging harder.

### 14. Single-separator heuristic assumes decimal, not grouping
- `TextUtil.cs:98-102`
- Pasting `"1,234"` from a locale using comma as thousands separator parses as `1.234` instead of `1234`.

### 15. Duplicate invalid test case in TextUtilTest
- `TextUtilTest.cs:279,287`
- `"12.34.56"` appears twice in `testCasesInvalid`. The second may have been intended as a different invalid pattern.

### 16. Missing `[TestFixture]` on `MathUtilTest` and `TextUtilTest`
- `MathUtilTest.cs:7`, `TextUtilTest.cs:8`
- Inconsistent with the other four test classes. Tests still run in NUnit 4 but could break under stricter configurations.
