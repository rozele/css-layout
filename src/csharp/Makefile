MSB=msbuild.exe /m /verbosity:m /nologo
NUGET=nuget.exe
VER=0.1.0.0
NAME=Facebook.CSSLayout

.PHONY: distribute
distribute: package release

.PHONY: package
package:
	cd ${NAME} && ${MSB} ${NAME}.csproj /p:Configuration=Release  /t:"Rebuild"
	cd ${NAME} && ${NUGET} pack ${NAME}.csproj -Version ${VER} -Prop Configuration=Release

.PHONY: release
release:
	cd ${NAME} && nuget push ${NAME}.${VER}.nupkg
