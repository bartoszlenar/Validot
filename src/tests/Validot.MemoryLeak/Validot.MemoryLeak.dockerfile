FROM mcr.microsoft.com/dotnet/sdk:5.0

WORKDIR /usr/app/validot

COPY tests/Validot.MemoryLeak Validot.MemoryLeak
COPY src/Validot Validot

RUN find . -iname "bin" | xargs rm -rf
RUN find . -iname "obj" | xargs rm -rf

RUN dotnet remove Validot.MemoryLeak/Validot.MemoryLeak.csproj reference ../../src/Validot/Validot.csproj
RUN dotnet add Validot.MemoryLeak/Validot.MemoryLeak.csproj reference Validot/Validot.csproj

RUN dotnet clean Validot.MemoryLeak/Validot.MemoryLeak.csproj
RUN dotnet clean Validot/Validot.csproj
RUN dotnet build Validot.MemoryLeak/Validot.MemoryLeak.csproj

ENTRYPOINT [ "dotnet", "run", "--project", "Validot.MemoryLeak/Validot.MemoryLeak.csproj" ]