# Squashy
Simple commandline git squasher.
- squash a sequence of commits (including the edges)
- supports view commit log and dry-run before squashing

### Commands
#### Display commit log
```
./Squashy -d <path/to/git/repo> log -n <num-of-commits>
```
#### Squash commits
- use `-x` for dry-run
```
./Squashy -d <path/to/git/repo> squash <first-commit> <second-commit> <commit-message> [-x]
```

### Building
- Use `make build` to publish the binary into a `./publish` directory
- Generate a test repo using `make gentest`
- Use `make test` to run tests
