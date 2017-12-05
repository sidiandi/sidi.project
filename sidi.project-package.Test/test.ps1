$ErrorActionPreference = "Stop";

$chocolateySource = Get-Item out\Release\Chocolatey | % { $_.FullName }

function EnsureCdEmptyDirectory
{
	Param ([string] $dir)

	if (Test-Path -Path $dir)
	{
		rm -r -fo $testDir
	}
	mkdir $dir
	cd $dir
}

$testDir = "out\Release\test\end-to-end-test"
EnsureCdEmptyDirectory $testDir

nuget install -OutputDirectory tool -Verbosity detailed -Source $chocolateySource sidi.project
$toolDir = Get-Item tool\sidi.project.*

$product="FooBarTest"
$exe = $toolDir.FullName + "\tools\sidi.project.exe"
start-process $exe "-vvvv $product" -wait -NoNewWindow
cd $product
.\build.cmd
exit

dir out\Release -r | %{$_.FullName}
out\Release\$product\bin\$product --help