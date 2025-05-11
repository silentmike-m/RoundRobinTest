# Round Robin

## Req

* .NET 8
* Docker

## Run application

```console
docker-compose build
docker-compose up
```

It will run
* Seq for logs at http://localhost:5341
* * user name: admin
* * user password: P@ssw0rd
* Simple API 1 at http://localhost:5081
* * Simple API 2 at http://localhost:5082
* * Simple API 3 at http://localhost:5083
* Round Robin API at http://localhost:5080

## Simple API

Simple API return received JSON object with 200 OK.
If JSON will be in wrong format then it will return 400 Bad Request.

### Simple API throw exception

It is possible to set API to throw exception on received request
```
SimpleApiOptions__ThrowExceptions: true
```

### Simple API delay

It is possible to set API to delay response
```
SimpleApiOptions__WaitTimeInSeconds: 10
```

## Round Robin API

Round Robin API will redirect proper JSON request to the next Simple API endpoint.
If JSON will be in wrong format then it will return 400 Bad Request.
If JSON will be empty it will return 400 Bad Request will proper validation error.

### Round Robin API Redis
Round Robin API uses Redis distributed cache service to store current index of Simple API endpoint. The use of an external cache server was dictated by the possibility of running several instances of the Round Robin API.

### Round Robin API retry policy
Round Robin API uses retry policy. 

<strong>MaxRetries</strong> defines how many times API should retry to send request.
<strong>RetryDelayInSeconds</strong> defines retry delays in seconds array. Retry policy will loop through the array according to <strong>MaxRetries</strong>, after reached the end of array it will start from the beginning.
<strong>RetryTimeoutInSeconds</strong> defines maximum retry policy timeout. Retry policy execution will be cancelled after this time. 


## Logs

Simple API application extends logs with <strong>AppInstanceVersion</strong>:W
![App Instance Version!](/readme_resources/images/app-instance-version.png)

### Simple API receive request log

The easiest way to see which Simple API instance receives request search for:
```
SourceContext = 'Coda.SimpleWebApi.Controllers.ApiController'
```
![Simple API receive!](/readme_resources/images/simple-api-receive.png)
