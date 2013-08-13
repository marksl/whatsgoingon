rd builds /s/q
md builds

rd build /s/q
md build

xcopy /E .\ver-1.0.0.0\*.* .\build
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe .\build\ver.csproj /t:Clean;Build /p:Configuration=Debug /p:DeployOnBuild=true /p:DeployTarget=Package
md .\builds\1.0.0.0-g1234567\
copy .\build\obj\Debug\Package\ver.zip .\builds\1.0.0.0-g1234567\

rd build /s/q
md build
xcopy /E .\ver-1.1.0.0\*.* .\build
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe .\build\ver.csproj /t:Clean;Build /p:Configuration=Debug /p:DeployOnBuild=true /p:DeployTarget=Package
md .\builds\1.1.0.0-g2345678\
copy .\build\obj\Debug\Package\ver.zip .\builds\1.1.0.0-g2345678\

rd build /s/q
md build
xcopy /E .\ver-1.1.1.0\*.* .\build
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe .\build\ver.csproj /t:Clean;Build /p:Configuration=Debug /p:DeployOnBuild=true /p:DeployTarget=Package
md .\builds\1.1.1.0-g3456789\
copy .\build\obj\Debug\Package\ver.zip .\builds\1.1.1.0-g3456789\

rd build /s/q
md build
xcopy /E .\ver-2.0.0.0\*.* .\build
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe .\build\ver.csproj /t:Clean;Build /p:Configuration=Debug /p:DeployOnBuild=true /p:DeployTarget=Package
md .\builds\2.0.0.0-g4567890\
copy .\build\obj\Debug\Package\ver.zip .\builds\2.0.0.0-g4567890\

rd build /s/q
