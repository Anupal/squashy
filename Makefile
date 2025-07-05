.PHONY: build


build:
	dotnet publish ./Squashy/Squashy.csproj -r linux-x64 -c Release --self-contained -p:PublishSingleFile=true -o publish
