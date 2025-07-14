.PHONY: build


build:
	dotnet publish ./Squashy/Squashy.csproj -r linux-x64 -c Release --self-contained -p:PublishSingleFile=true -o publish
gentest:
	bash setup-test-repo.sh
run:
	./publish/Squashy -d ./test-repo log -n 10
clean:
	dotnet clean -c Release
	rm -rf publish
	rm -rf ./Squashy/bin
	rm -rf ./Squashy/obj
squash-dry-run:
	./publish/Squashy -d ./test-repo squash a35c0aa 6eeab88 "this is the squashed commit" -x
test:
	dotnet test
