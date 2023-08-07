using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => {
    options.ListenLocalhost(8080);
    });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

int ElevatorCount = builder.Configuration["ElevatorCount"] != null ? int.Parse(builder.Configuration["ElevatorCount"]) : 1;
if(ElevatorCount < 1) throw new Exception("ElevatorCount must be greater than 0");

int FloorCount = builder.Configuration["FloorCount"] != null ? int.Parse(builder.Configuration["FloorCount"]) : 20;
if(FloorCount < 2) throw new Exception("FloorCount must be greater than 1");

var elevators = new Dictionary<int, Elevator.Elevator>(ElevatorCount);

for(int i = 0; i < ElevatorCount; i++){
    elevators.Add(i, new Elevator.Elevator(i, FloorCount));
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();



// return a string representation of the elevator's current state
app.MapGet("/", () => {
    return Results.Ok(new { ElevatorCount, FloorCount, Elevators = elevators.Values});
});

// return a string representation of the elevator's current state without moving the elevator
app.MapGet("/elevator/{id}/nextFloor", (int id) => {
    if(elevators.TryGetValue(id, out Elevator.Elevator? value))
    {
        return Results.Ok(value.NextStop());
    }
    else{
        return Results.BadRequest($"Invalid elevator id\n\t requested elevator id: {id}\n\t valid elevator ids: 0-{ElevatorCount - 1}");
    }
});

// moves the elevator to its next scheduled floor
app.MapGet("/elevator/{id}/goNextFloor", (int id) => {
    if(elevators.TryGetValue(id, out Elevator.Elevator? value))
    {
        value.GoNextStop();
        return Results.Ok();
    }
    else{
        return Results.BadRequest($"Invalid elevator id\n\t requested elevator id: {id}\n\t valid elevator ids: 0-{ElevatorCount - 1}");
    }
});

// returns the elevator's current floor
app.MapGet("/elevator/{id}/currentFloor", (int id) => {
    if(elevators.TryGetValue(id, out Elevator.Elevator? value))
    {
        return Results.Ok(value.CurrentFloor);
    }
    else{
        return Results.BadRequest($"Invalid elevator id\n\t requested elevator id: {id}\n\t valid elevator ids: 0-{ElevatorCount - 1}");
    }
});

// returns a list of all the floors the elevator is scheduled to stop at
app.MapGet("/elevator/{id}/allRequests", (int id) => {
    if(elevators.TryGetValue(id, out Elevator.Elevator? value))
    {
        return Results.Ok(value.FloorRequests);
    }
    else{
        return Results.BadRequest($"Invalid elevator id\n\t requested elevator id: {id}\n\t valid elevator ids: 0-{ElevatorCount - 1}");
    }
});

// adds a stop to a specific elevator's list of scheduled stops
app.MapPost("/elevator/{id}/floor/{floor}", (int id, int floor) => {
    if(elevators.TryGetValue(id, out Elevator.Elevator? value))
    {
        value.AddStop(floor);
        return Results.Ok();
    }
    else{
        return Results.BadRequest($"Invalid elevator id\n\t requested elevator id: {id}\n\t valid elevator ids: 0-{ElevatorCount - 1}");
    }
}); 

// requests a stop at a specific floor from the closest elevator, determined by the elevator's Floor distance,
//  which takes into account the elevator's current direction and the elevator's scheduled stops
app.MapPost("/elevator/floor/{floor}", (int floor) => {
    var closestElevator = elevators.Values.OrderBy(elevator => elevator.FloorDistance(floor)).First();
    closestElevator.AddStop(floor);
    return Results.Ok(closestElevator.Id);
});

app.Run();
