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

### 7. ~~`Pow` is left-associative instead of right-associative~~ (NOT A BUG — intentional)
- `Cpu.cs:665-668`
- Left-associative `Pow` matches physical calculator behavior by design.

### 8. ~~`WasChanged` set true even when clicking already-selected option~~ (FIXED)
- Used `when` guards to skip no-op clicks on already-selected controls.

### 9. `_mainDisplayContent` could be null
- `Body.cs:87`
- `GetMainDisplayString()` calls `_mainDisplayContent.ToString()` but `_mainDisplayContent` is initialized to null. Currently protected by the catch-all in Copy and by `Refresh()` running during construction, but fragile.

### 10. `NumberDisplay.Show()` pushes nullable `_error` without null guard
- `NumberDisplay.cs:110`
- When `_hasError` is true, `_error` (declared `DisplayGlyph?`) is pushed onto `ShownGlyphs`. Currently safe because the constructor keeps `_hasError` and `_error` in sync, but these are independent fields — a refactor could introduce a NullReferenceException.

### 11. ~~`_numBase` vs hardcoded `10` inconsistency~~ (FIXED)
- Replaced all `ToDouble(10)` with `ToDouble(_numBase)`.

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

### 15. ~~Duplicate invalid test case in TextUtilTest~~ (FIXED)
- Replaced duplicate `"12.34.56"` with `"12.34,56"`.

### 16. ~~Missing `[TestFixture]` on `MathUtilTest` and `TextUtilTest`~~ (FIXED)
- Added `[TestFixture]` to both classes for consistency.

## Round 3

### Bugs

### 17. AngleUnits (DegRad) toggle is not persisted to registry
- `MainWindow.xaml.cs:337` (default case)
- The `DegRad` button toggles `cpu.AngleUnits` in memory but never saves to registry. Compare with `Format` (line 288) which calls `Settings.SaveDisplayFormat()`. If the user switches Deg/Rad and closes the app, the change is lost on next launch.

### Warnings

### 18. `SmartSum` takes unnecessary rounding path when sum is exactly zero
- `MathUtil.cs:206`
- When `abssum == 0` (e.g., `5 + (-5)`), `Math.Log10(0)` = `-Infinity` causes the early-return condition to fail, sending the calculation through the rounding path. The result is correct (0.0) but only because `Math.Round(0 / cutoff) = 0`. An early return for `abssum == 0` would be simpler and more robust.

### 19. ~~`DoubleByDigit.FromDouble` uses `long` arithmetic that could overflow for large mantissa lengths~~ (IGNORED)
- Not a practical concern with current `mantissaLength` of 14.

### Suggestions

### 20. ~~`Atanh(+/-1)` returns infinity instead of NaN~~ (IGNORED)
- No visible effect: both Infinity and NaN display as "error" via `DoubleByDigit.FromDouble`.

### 21. Dead `Padding` assignment on ThemedMessageBox button
- `ThemedMessageBox.cs:52`
- The `Button.Padding` is set but ignored because the custom `ControlTemplate` defines its own padding without a `TemplateBinding`. The button-level value is dead code.
