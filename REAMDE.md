# Squashy
Simple commandline git squasher.

- Use `make build` to publish the binary into a `./publish` directory
- Can only display commits in a directory right now.
  ```
  ./publish/Squashy list -d </path/to/git-repo> -n <num-commits>
  ```