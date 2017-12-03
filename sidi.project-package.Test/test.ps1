cd out\Release
rm -r -fo test
mkdir test
cd test
$chocolateySource = Get-Item ..\Chocolatey | % { $_.FullName }
nuget install -OutputDirectory tool -Verbosity detailed -Source $chocolateySource sidi.project
tool\sidi.project.0.1.0\tools\sidi.project.exe -vvvv --Product FooBar --Company ACME --init
.\build.cmd
dir out\Release -r | %{$_.FullName}
out\Release\FooBar\bin\FooBar --help