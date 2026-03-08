# Code Review — Round 2

## Bugs

### 1. ~~Closing SettingsDialog via X button applies spurious changes~~ (FIXED)
- `SettingsDialog.xaml.cs:85-86,96-103` / `MainWindow.xaml.cs:241`
- The `RoundingDigitsComboBox_SelectionChanged` handler fired during construction when `SelectedItem` was set, which set `WasChanged = true`. The Cancel button resets this flag, but closing via the window's X button did not. MainWindow then saw `WasChanged == true` and applied/saved unchanged settings.
- **Fix:** Added `_initialized` flag to skip `WasChanged = true` in `SelectionChanged` during construction.

### 2. ~~Custom command sequence doesn't stop on CPU error~~ (FIXED)
- `MainWindow.xaml.cs:324-329`
- When executing a custom command sequence, each command runs in a loop with no check for overflow/error between commands. If the first command causes an error (e.g., `1/0`), subsequent commands continue executing against the error state, producing confusing results.
- **Fix:** Added `double.IsNaN(cpu.X)` break check in the command loop.

### 3. ~~Trig zero-detection heuristic fragile for large angles~~ (FIXED)
- `Cpu.cs:924-931` (sin), `949` (cos), `976,980` (tan)
- The special-case check `Math.Abs(X % Math.PI) < CpuPrecision * Math.Abs(X)` uses a relative epsilon that scales with X. For very large exact multiples of 180 degrees, the modulo residual can exceed the threshold, so sin/cos/tan won't return exact zero as intended.
- **Fix:** Replaced with `IsMultipleOf`/`IsOddMultipleOfHalf`/`IsNearInteger` helpers using division-based relative epsilon for radians.

### 4. ~~Duplicate test data in TextUtilTest instead of exponent test cases~~ (FIXED)
- `TextUtilTest.cs:163-183`
- The comment says "with decimal point and exponent" but the test cases are exact duplicates of lines 142-161 (the "with decimal point" section). The intended exponent-combination tests are missing, leaving a coverage gap.
- **Fix:** Removed the 20 duplicate lines. The real exponent tests already existed below.

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

### 17. ~~AngleUnits (DegRad) toggle is not persisted to registry~~ (FIXED)
- `MainWindow.xaml.cs:337` (default case)
- The `DegRad` button toggles `cpu.AngleUnits` in memory but never saves to registry. Compare with `Format` (line 288) which calls `Settings.SaveDisplayFormat()`. If the user switches Deg/Rad and closes the app, the change is lost on next launch.
- **Fix:** Added dedicated `DegRad` case with `Settings.SaveAngleUnits()` call.

### Warnings

### 18. ~~`SmartSum` takes unnecessary rounding path when sum is exactly zero~~ (FIXED)
- `MathUtil.cs:206`
- When `abssum == 0` (e.g., `5 + (-5)`), `Math.Log10(0)` = `-Infinity` causes the early-return condition to fail, sending the calculation through the rounding path. The result is correct (0.0) but only because `Math.Round(0 / cutoff) = 0`. An early return for `abssum == 0` would be simpler and more robust.
- **Fix:** Added early return for `abssum == 0`.

### 19. ~~`DoubleByDigit.FromDouble` uses `long` arithmetic that could overflow for large mantissa lengths~~ (IGNORED)
- Not a practical concern with current `mantissaLength` of 14.

### Suggestions

### 20. ~~`Atanh(+/-1)` returns infinity instead of NaN~~ (IGNORED)
- No visible effect: both Infinity and NaN display as "error" via `DoubleByDigit.FromDouble`.

### 21. ~~Dead `Padding` assignment on ThemedMessageBox button~~ (FIXED)
- `ThemedMessageBox.cs:52`
- The `Button.Padding` is set but ignored because the custom `ControlTemplate` defines its own padding without a `TemplateBinding`. The button-level value is dead code.
- **Fix:** Removed the dead `Padding` assignment.

## Round 4

### Bugs

### 22. Ctrl+Shift+letter shortcuts fall through to unmodified shortcut, triggering wrong command
- `MainWindow.xaml.cs:191`
- `Keyboard.Modifiers == ModifierKeys.Control` is an exact equality check, so when Ctrl+Shift are both held, it evaluates to `false` (Modifiers is `Control | Shift`). Likewise for `Shift`. Both `ctrl` and `shift` become `false`, mapping to shortcut index 0 (no modifiers). Pressing Ctrl+Shift+S triggers `Sin`, Ctrl+Shift+T triggers `Tan`, etc.

### 23. Custom command error check misses Infinity
- `MainWindow.xaml.cs:330`
- The custom command loop breaks on `double.IsNaN(cpu.X)` but not on `double.IsInfinity(cpu.X)`. If a command produces Infinity (e.g., `InvX` of 0, or `Atanh(1)`), subsequent commands continue executing against the infinity value. The display shows "error" via `DoubleByDigit.FromDouble`, but the CPU keeps running commands.

### 24. `IsMultipleOf` / `IsOddMultipleOfHalf` degrees branch uses absolute epsilon — fragile for large angles
- `Cpu.cs:552,571`
- The degrees branches use `Math.Abs(angle % degPeriod) < CpuPrecision` with absolute epsilon `1e-15`. For very large degree values (e.g., `1e16`), floating-point `%` loses precision and the residual exceeds `1e-15`. The radians branches correctly use relative epsilon via `IsNearInteger`, but the degrees branches do not. This is a residual aspect of issue #3 that wasn't fully addressed.

### Warnings

### 25. `ExecuteLeftParen` silently ignores `(` unless last action was `Binop`
- `Cpu.cs:884-891`
- `ExecuteLeftParen` only calls `_stack.HeadParenAdd()` when `_lastAction == Action.Binop`. Pressing `(` at the start of an expression (when `_lastAction` is `Clear` or `Enter`) does nothing. The expression `(2+3)*4` evaluates as `2+3*4=14` instead of `20`. The `)` handler partially compensates by acting as `=`, but nested/leading parens don't work as users would expect.

### 26. ~~`ExchXy` is not in the `RpnOnly` list, allowing it in ALG mode~~ (NOT A BUG — intentional)
- `Command.cs:83-84`, `Cpu.cs:860-866`
- `ExchXy` in ALG mode swaps X with the left operand of a pending binary operation. This is intentional behavior.

### Suggestions

### 27. Truncated comment in Cpu constructor
- `Cpu.cs:165`
- Comment reads `// setting the s` — appears truncated (likely "setting the state" or similar).

### 28. `DoubleByDigit` inconsistently uses `.Count()` (LINQ) vs `.Count` (property)
- `DoubleByDigit.cs:81,85,87,124,134`
- Several places call the LINQ extension `List<int>.Count()` instead of the O(1) `.Count` property. Other places in the same file use `.Count` correctly. Functionally identical but inconsistent and slightly less efficient.

### 29. Unreachable `return Double.NaN` in TextUtil
- `TextUtil.cs:187`
- The final `return Double.NaN` (labeled "should not get here") is unreachable — all preceding branches are exhaustive. Dead code.
