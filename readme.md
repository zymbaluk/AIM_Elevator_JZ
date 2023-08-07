# IN PROGRESS


# Elevator Control System

This is a simple elevator control system that can be used to control a set of elevators in a building.

## Building and running the project

* Download dotnet core 6 SDK from https://dotnet.microsoft.com/en-us/download
* run `dotnet build` from project root to build the project
* run `dotnet run --project .\Elevator\Elevator.csproj` from project root to run the project

---or---

* run `buildAndRun.bat` or `buildAndRun.sh` or `buildAndRun.ps1` from project root to build and run the project

http://localhost:8080 will be the default url for the project

swagger documentation is enabled and can be found at http://localhost:8080/swagger/index.html, the swagger UI includes a test client that can be used to test the API

## Configuration

The project uses the `appsettings.json` file to configure the number of elevators and floors in the building. The default configuration is 2 elevators and 20 floors. The configuration can be changed by editing the `appsettings.json` file and rebuilding the project.

## Design

The original problem statement had the following requirements:

1. A person requests an elevator be sent to their current floor
1. A person requests that they be brought to a floor
1. An elevator car requests all floors that it’s current passengers are servicing (e.g. to light up the buttons that show which floors the car is going to)
1. An elevator car requests the next floor it needs to service

The API endpoints that satisfy these requirements are:

###  1. A person requests an elevator be sent to their current floor

`POST /elevator/floor/{floor}` this API sends an elevator to the floor specified in the path parameter. If multiple elevators exist, the optimal elevator will be selected using a total travel distance algorithm which takes into account the current floor of the elevator, the direction it is travelling in, and the floors it is already servicing.

This API could be improved by adding a `direction` parameter to the request body. This would allow the customer to specify which direction they want to travel in. This would be useful in a building with multiple elevators where the customer wants to travel in a specific direction.

###  2. A person requests that they be brought to a floor

`POST /elevator/{id}/floor/{floor}` This API sends the elevator specified in the path parameter to the floor specified in the path parameter. It's meant to simulate being inside an elevator and pressing a button to go to a floor. The elevator has already been chosen and will now service the customer's request.

###  3. An elevator car requests all floors that it’s current passengers are servicing (e.g. to light up the buttons that show which floors the car is going to)

`GET /elevator/{id}/allRequests` This API returns a list of all the floors that the elevator specified in the path parameter is servicing. 

###  4. An elevator car requests the next floor it needs to service

`GET /elevator/{id}/nextFloor` This API returns the next floor that the elevator specified in the path parameter will visit. This API does NOT actually send the elevator to the next floor. It's meant to simulate the elevator's control system requesting the next floor that the elevator will visit.

`GET /elevator/{id}/goNextFloor` This API sends the elevator specified in the path parameter to the next floor that it will visit. This API is meant to simulate the elevator's control system sending the elevator to the next floor that it will visit.

### Additional APIs

`GET /` This API returns a list of all the elevators in the building, as well as the number of floors and number of elevators. This API is meant to be used for testing purposes.

` GET /elevator/{id}/currentFloor` This API returns the current floor of the elevator specified in the path parameter. This API is meant to be used for testing purposes.