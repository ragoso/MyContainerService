# MyContainerService
This is an simple abstraction of Docker (For now) Services API to easily and safely manage services from anywhere.

:warning: Be very careful with production use. **Credentials not implemented yet**.

# How to use

Build and deploy:

```
docker build -t mcs:latest . && docker run -d -p 80:80 -v /var/run/docker.sock:/var/run/docker.sock mcs:latest
```

Deployments of apps with Console.dll:

```
$ dotnet Console/bin/Debug/net5.0/Console.dll -h
Usage: Console ...
Interact com MyContainerService

Options:
  -u, --url=VALUE            The url of grpc endpoints
  -f, --file=VALUE           The yaml or json of service or tar of image to be
                               handled
  -t, --tag=VALUE            The image tag to build and update.
  -p, --param=VALUE          The param to build. Ex: foo=bar
  -s, --stream               Use streaming transfer to build images.
  -b, --buffer=VALUE         The buffer size to streaming image build
  -a, --action=VALUE         The action to perform (create,update,remove)
  -w, --write                Write yaml example
  -h, --help                 Print help
```

# ToDo
- [ ] Tests, tests, tests and more tests.
- [x] GRPC Endpoint.
- [x] CLI that recives MyService json to pass on Endpoint writed in Golang.
- [ ] Load docker-compose.yaml.
- [ ] GRPC Authentication.
- [ ] Integrate with Kubernetes.
- [ ] UI prefered in VueJS.
- [ ] Increment this ToDo.
