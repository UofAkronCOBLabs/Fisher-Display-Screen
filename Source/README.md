# Fisher Display Screen — source

C# source for `Display test.exe`, the College of Business marketing touchscreen kiosk.
It was reconstructed from the compiled Release build in the repo root, since the original project wasn't checked in.

## What it does

- **Home screen:** social handles, "Touch Screen to Interact", and three tiles: Alumni Spotlight, Experience Business and Social Media.
- **Tiles:** each opens a full-screen embedded Chromium browser (CefSharp) showing a uakron.edu page or the Keyhole social wall.
- **Navigation:** Home/Back buttons, plus « / » to move them to the other corner.
- **Popups:** links that try to open a new window load in place instead (`ChromiumLifeSpanHandler`).
- **Idle timeout:** after 2 minutes without a touch, a "Still Watching?" prompt appears. With no answer in 20 seconds it returns to Home.
- **Usage log:** taps, page changes and idle events go to `%USERPROFILE%\Documents\displaytest_output\data.csv`.

## Build

1. Open `Display test.sln` in Visual Studio 2022. It needs the .NET Framework 4.7.2 targeting pack.
2. Restore NuGet packages. The project uses `CefSharp.WinForms` 117.2.40, the same version as the shipped build.
3. Build **Release | x64**.

The output in `bin\x64\Release\` replaces the files in the repo root.

## Notes on the reconstruction

- **Fidelity:** every type, field and method signature matches the shipped exe. All 77 images were extracted byte-for-byte into `Resources/`.
- **Tested:** the rebuilt app ran under Mono with the Chromium control stubbed out, and its screens were pixel-identical to the original exe.
- **Comments:** the original comments and local variable names don't survive compilation. Names here were chosen to match what the code does.
- **Known quirk kept as-is:** `lblText.ForeColor` on the "Still Watching?" popup is `Color.Transparent`, so the prompt text is invisible. Change it to `Color.White` in `InActivityWindow.Designer.cs` to show it.
- **Unused code kept as-is:** `pictureBox3_Click` (a 5-tap debug toggle) and `gridbuttonResize` aren't wired to any control. `DebugEnable()` also only logs `DebugOn`, so the debug overlay never appears.
