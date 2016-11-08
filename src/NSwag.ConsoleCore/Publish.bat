rmdir Output /Q /S nonemptydir
mkdir Output

dotnet restore --no-cache
dotnet build

dotnet publish -c release -r win7-x86
dotnet publish -c release -r win7-x64

dotnet publish -c release -r win8-x86
dotnet publish -c release -r win8-x64

dotnet publish -c release -r win81-x86
dotnet publish -c release -r win81-x64

dotnet publish -c release -r win10-x86
dotnet publish -c release -r win10-x64

dotnet publish -c release -r osx.10.10-x64

dotnet publish -c release -r linuxmint.17-x64
dotnet publish -c release -r ol.7-x64
dotnet publish -c release -r opensuse.13.2-x64
dotnet publish -c release -r fedora.23-x64
dotnet publish -c release -r centos.7-x64
dotnet publish -c release -r ubuntu.14.04-x64
dotnet publish -c release -r rhel.7.0-x64
dotnet publish -c release -r debian.8-x64

xcopy "bin\release\netcoreapp1.0\win7-x86\publish" "Output\win7-x86" /E /I
xcopy "bin\release\netcoreapp1.0\win7-x64\publish" "Output\win7-x64" /E /I

xcopy "bin\release\netcoreapp1.0\win8-x86\publish" "Output\win8-x86" /E /I
xcopy "bin\release\netcoreapp1.0\win8-x64\publish" "Output\win8-x64" /E /I

xcopy "bin\release\netcoreapp1.0\win81-x86\publish" "Output\win81-x86" /E /I
xcopy "bin\release\netcoreapp1.0\win81-x64\publish" "Output\win81-x64" /E /I

xcopy "bin\release\netcoreapp1.0\win10-x86\publish" "Output\win10-x86" /E /I
xcopy "bin\release\netcoreapp1.0\win10-x64\publish" "Output\win10-x64" /E /I

xcopy "bin\release\netcoreapp1.0\osx.10.10-x64\publish" "Output\osx.10.10-x64" /E /I

xcopy "bin\release\netcoreapp1.0\linuxmint.17-x64\publish" "Output\linuxmint.17-x64" /E /I
xcopy "bin\release\netcoreapp1.0\ol.7-x64\publish" "Output\ol.7-x64" /E /I
xcopy "bin\release\netcoreapp1.0\opensuse.13.2-x64\publish" "Output\opensuse.13.2-x64" /E /I
xcopy "bin\release\netcoreapp1.0\fedora.23-x64\publish" "Output\fedora.23-x64" /E /I
xcopy "bin\release\netcoreapp1.0\centos.7-x64\publish" "Output\centos.7-x64" /E /I
xcopy "bin\release\netcoreapp1.0\ubuntu.14.04-x64\publish" "Output\ubuntu.14.04-x64" /E /I
xcopy "bin\release\netcoreapp1.0\rhel.7.0-x64\publish" "Output\rhel.7.0-x64" /E /I
xcopy "bin\release\netcoreapp1.0\debian.8-x64\publish" "Output\debian.8-x64" /E /I
