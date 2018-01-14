# APP4-Compilateur

Mikaël Gendreau<br>
Sébastien Klasa

## Linux (testé sous Ubuntu)

Prérequis :

- mono-devel (testé avec la version 4.2.1)
- Une larme de rhinocéros

Compilation :

`xbuild CLightCompiler/CLightCompiler/CLightCompiler.csproj` - Il faut compiler le projet CLightCompiler directement et non pas la solution car les tests unitaires ne compilent pas avec xbuild.

Utilisation :

`cd CLightCompiler/CLightCompiler/bin/Debug/`
`mono CLightCompiler.exe <fichier_source> <fichier_de_sortie>`

## Windows

Prérequis :

- Une licence Microsoft Windows
- Microsoft Visual Studio (2017 de préférence)

Compilation :

Compilation avec Visual Studio.

Utilisation :

`CLightCompiler/CLightCompiler/bin/Debug/CLightCompiler.exe <fichier_source> <fichier_de_sortie>`
