# gb-net
A Game Boy emulator in C# / .NET, heavily based on https://github.com/davidwhitney/CoreBoy after realizing I forgot about timing 😅. 

The WinForms version is running pretty smoothly, including sound, although not all of the test ROMs out there are passing and GBC support is not all there yet.
In short: there are better emulators out there if you want to play games.

But if you want to hack something together, go ahead and have fun!

## New!

The GB.WASM folder contains a WebAssembly version of the emulator, running in the browser. It's not as fast as the WinForms version, but it's still pretty cool to see it running in the browser!
If you want to try that one, use `launch.bat` to publish an AOT version of the emulator and start a local web server.

Key bindings are not working yet, but you can see the game starting and playing it's demo level :)