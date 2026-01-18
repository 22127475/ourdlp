set SRC_CSPROJ=%1
set SW_OUT_PATH=%2
dotnet publish %SRC_CSPROJ% -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true -o %SW_OUT_PATH%