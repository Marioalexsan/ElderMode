Rem Copy the required files

Rem Set your paths here
Rem The first path should point to the build directory (don't modifiy it unless you modify all .csproj too)
Rem The second path points to your game directory
set solution_dir=%1
set build_dir=%solution_dir%ElderMode\bin\x86\Debug
set sog_dir="C:\Program Files (x86)\Steam\steamapps\common\SecretsOfGrindea"

echo Build directory is %build_dir%

Rem Copy the mod
xcopy %build_dir%\ElderMode.dll %sog_dir%\Mods\ /y /f /i

echo Finished copying