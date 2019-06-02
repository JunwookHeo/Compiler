#!/bin/bash

mono Gplex.exe /unicode ../scanner.lex
mv scanner.cs ../

mono Gppg.exe /gplex ../parser.y > parser.cs
mv parser.cs ../
