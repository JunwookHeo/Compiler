Gplex.exe /unicode ..\scanner.lex
move scanner.cs ..\

Gppg.exe /gplex ..\parser.y > parser.cs
move parser.cs ..\

