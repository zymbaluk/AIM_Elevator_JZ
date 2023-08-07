namespace Elevator
{
    public class Elevator
    {
        public int Id { get; }
        public int CurrentFloor { get; set; }

        public int MaxFloor { get; set; }
        public Direction Direction { get; set; }

        public SortedSet<int> FloorRequests { get; set; } = new SortedSet<int>();

        public Elevator(int Id, int maxFloor)
        {
            this.MaxFloor = maxFloor;
            this.Id = Id;
            CurrentFloor = 0; 
            Direction = Direction.AtRest;
        }

        public Elevator(int Id, int maxFloor, int floor, Direction direction)
        {
            this.Id = Id;
            this.MaxFloor = maxFloor;
            CurrentFloor = floor;
            Direction = direction;
        }

        public void AddStop(int floor)
        {
            if(floor > MaxFloor || floor < 0)
            {
                throw new IndexOutOfRangeException($"Invalid floor request: {floor} - must be between 0 and {MaxFloor}");
            }

            if(floor > CurrentFloor && Direction == Direction.AtRest)
            {
                Direction = Direction.Up;
            }
            if(floor < CurrentFloor && Direction == Direction.AtRest)
            {
                Direction = Direction.Down;
            }
            if(floor == CurrentFloor)
            {
                return;
            }
            FloorRequests.Add(floor);
        }

        /// <summary>
        /// returns the next floor the elevator will stop at, without actually moving the elevator
        /// </summary>
        /// <returns>the next floor the elevator stops at</returns>
        public int NextStop()
        {
            // if there are no requests, the elevator is at rest
            if(FloorRequests.Count == 0)
            {
                if(Direction != Direction.AtRest)
                {
                    throw new Exception("Elevator has zero stops but is not at rest - something went wrong");
                }
                return CurrentFloor;
            }

            // if the elevator is moving up, find the next requested floor above the current floor, if there is one
            // if not, turn around
            if(Direction == Direction.Up)
            {
                var nextFloor = FloorRequests.GetViewBetween(CurrentFloor, int.MaxValue).Min;
                if(nextFloor == default)
                {
                    // sortedset.Max returns default (which is 0) if the set is empty, but we know it's not empty because we 
                    // just checked so we can return whatever we get here
                    return FloorRequests.GetViewBetween(0, CurrentFloor).Max;

                }
                else
                {
                    return nextFloor;
                }
            }

            // if the elevator is moving down, find the next requested floor below the current floor, if there is one
            // if not, turn around
            if(Direction == Direction.Down)
            {
                var nextFloor = FloorRequests.GetViewBetween(int.MinValue, CurrentFloor).Max;
                if(nextFloor == default && !FloorRequests.Contains(0))
                {
                    // sortedset.Min returns default (which is 0) if the set is empty, but we know it's not empty because we 
                    // checked above so we can return whatever we get here
                    return FloorRequests.GetViewBetween(CurrentFloor, int.MaxValue).Min;
                }
                else
                {
                    return nextFloor;
                }
            }

            throw new Exception("This shouldn't happen - something went wrong");
        }

        /// <summary>
        /// Moves the elevator to the next stop and removes that stop from the list of requests
        /// Also updates the direction of the elevator if necessary
        /// </summary>
        public void GoNextStop()
        {
            CurrentFloor = NextStop();
            FloorRequests.Remove(CurrentFloor);

            if(CurrentFloor == MaxFloor)
            {
                Direction = Direction.Down;
            }
            if(CurrentFloor == 0)
            {
                Direction = Direction.Up;
            }
            if(FloorRequests.Count == 0)
            {
                Direction = Direction.AtRest;
            }
        }

        /// <summary>
        /// Calculates the distance between the current floor and the requested floor, taking into account the direction
        /// </summary>
        /// <param name="requestedFloor">The floor requested by the user</param>
        /// <returns>the number of floors the elevator will have to travel to service this request</returns>
        /// <exception cref="Exception">Should never happen</exception>
        public int FloorDistance(int requestedFloor)
        {
            // the distance between the current floor and the requested floor is zero
            if(requestedFloor == CurrentFloor)
            {
                return 0;
            }

            // If the elevator is not in motion, the distance is the difference between the current floor 
            // and the requested floor
            if(Direction == Direction.AtRest)
            {
                return Math.Abs(requestedFloor - CurrentFloor);
            }

            // if the elevator is moving, it wants to continue in the same direction until it reaches the furthest 
            // requested floor in that direction before turning around
            if(Direction == Direction.Up)
            {
                // if the requested floor is above the current floor, the distance is the difference between the two
                if(requestedFloor > CurrentFloor)
                {
                    return requestedFloor - CurrentFloor;
                }
                // if the requested floor is below the current floor, the distance is the difference between the 
                // current floor and the peak requested floor plus the difference between the peak and requested floor
                else
                {
                    var peakDistance = FloorRequests.Max - CurrentFloor;
                    return peakDistance + (FloorRequests.Max - requestedFloor);
                }
            }
            if(Direction == Direction.Down)
            {
                // if the requested floor is below the current floor, the distance is the difference between the two
                if(requestedFloor < CurrentFloor)
                {
                    return CurrentFloor - requestedFloor;
                }
                // if the requested floor is above the current floor, the distance is the difference between the
                // current floor and the trough requested floor plus the difference between the trough and requested floor
                else
                {
                    var troughDistance = CurrentFloor - FloorRequests.Min;
                    return troughDistance + (requestedFloor - FloorRequests.Min);
                }
            }
            
            throw new Exception("This shouldn't happen - something went wrong");
        }

        public override string ToString()
        {
            return $"Elevator {Id} is on floor {CurrentFloor} and is moving {Direction} Currently servicing requests for floors: [{string.Join(", ", FloorRequests)}]";
        }
    }

    public enum Direction
    {
        Up,
        Down,
        AtRest
    }
}
