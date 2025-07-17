.PHONY: build


build:
	dotnet publish ./Squashy/Squashy.csproj -r linux-x64 -c Release --self-contained -p:PublishSingleFile=true -p:InvariantGlobalization=true -o build/bin
package-deb:
	bash package.sh all 1.0.0
gentest:
	bash setup-test-repo.sh
run:
	./publish/Squashy -d ./test-repo log -n 10
clean:
	dotnet clean -c Release
	rm -rf build/bin
	rm -rf ./Squashy/bin
	rm -rf ./Squashy/obj
test:
	dotnet test
