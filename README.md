# Round Robin

# Table of Contents
1. [Requirements](#requirements)
2. [Run application](#run-application)
3. [Simple API](#simple-aPI)
   * [Simple API throw exception](#simple-api-throw-exception)
   * [Simple API delay](#simple-api-delay)
4. [Round Robin API](#round-robin-api)
   * [Round Robin API Redis](#round-robin-api-redis)
   * [Round Robin API retry policy](#round-robin-api-retry-policy)
5. [Logs](#logs)
   * [Simple API receive request log](#simple-api-receive-request-log)

## Requirements

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

Simple API returns received JSON object with 200 OK.<br>
If JSON is in wrong format, returns 400 Bad Request.

### Simple API throw exception

It is possible to set API to throw exception on received request:
```
SimpleApiOptions__ThrowExceptions: true
```

### Simple API delay

It is possible to set API to delay response:
```
SimpleApiOptions__WaitTimeInSeconds: 10
```

## Round Robin API

Round Robin API redirects proper JSON request to the next Simple API endpoint.<br>
If JSON is in wrong format, returns 400 Bad Request.<br>
If JSON is empty, returns 400 Bad Request will proper validation error.<br>

### Round Robin API Redis
Round Robin API uses Redis distributed cache service to store current index of Simple API endpoint. The use of an external cache server was dictated by the possibility of running several instances of the Round Robin API.

### Round Robin API retry policy
Round Robin API uses retry policy. 

* <strong>MaxRetries</strong> defines how many times API should retry to send request.
* <strong>RetryDelayInSeconds</strong> defines retry delays in seconds array. Retry policy will loop through the array according to <strong>MaxRetries</strong>, after reached the end of array it will start from the beginning.
* <strong>RetryTimeoutInSeconds</strong> defines maximum retry policy timeout. Retry policy execution will be cancelled after this time. 

### Round Robin API endpoint status

After starting application executes health check for each defined simple api, every 30 seconds.<br>
Based on health check result endpoints cache is updated.<br>
Round robin algorithm will skip unhealthy endpoints.

## Logs

Simple API application extends logs with <strong>AppInstanceVersion</strong>:
![App Instance Version!](/readme_resources/images/app-instance-version.png)

### Simple API receive request log

The easiest way to see which Simple API instance receives request search for:
```
SourceContext = 'Coda.SimpleWebApi.Controllers.ApiController'
```
![Simple API receive!](/readme_resources/images/simple-api-receive.png)
