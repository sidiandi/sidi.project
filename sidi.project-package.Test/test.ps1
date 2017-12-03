cd out\Release
rm -r -fo test
mkdir test
cd test
nuget install -OutputDirectory tool -Verbosity detailed -Source C:\work\sidi.project\out\Release\package sidi.project
tool\sidi.project.0.1.0\tools\sidi.project.exe -vvvv --init
.\build.cmd
dir out\Release -r | %{$_.FullName}
out\Release\stage\MyProduct --help

