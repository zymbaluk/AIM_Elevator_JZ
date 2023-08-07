using Xunit;

namespace Elevator.Tests
{
    public class ElevatorTests
    {
        [Fact]
        public void AddStop_AddsFloorRequest()
        {
            // Arrange
            var elevator = new Elevator(1, 10);

            // Act
            elevator.AddStop(5);

            // Assert
            Assert.Contains(5, elevator.FloorRequests);
        }

        [Fact]
        public void AddStop_ThrowsExceptionForInvalidFloor()
        {
            // Arrange
            var elevator = new Elevator(1, 10);

            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => elevator.AddStop(11));
        }

        [Fact]
        public void NextStop_ReturnsCurrentFloorIfNoRequests()
        {
            // Arrange
            var elevator = new Elevator(1, 10);

            // Act
            var nextStop = elevator.NextStop();

            // Assert
            Assert.Equal(elevator.CurrentFloor, nextStop);
        }

        [Fact]
        public void NextStop_ReturnsNextFloorInDirection()
        {
            // Arrange
            var elevator = new Elevator(1, 10);
            elevator.AddStop(5);
            elevator.AddStop(7);

            // Act
            var nextStop = elevator.NextStop();

            // Assert
            Assert.Equal(5, nextStop);
        }

        [Fact]
        public void GoNextStop_MovesElevatorToNextStop()
        {
            // Arrange
            var elevator = new Elevator(1, 10);
            elevator.AddStop(5);

            // Act
            elevator.GoNextStop();

            // Assert
            Assert.Equal(5, elevator.CurrentFloor);
        }

        [Fact]
        public void GoNextStop_RemovesStopFromList()
        {
            // Arrange
            var elevator = new Elevator(1, 10);
            elevator.AddStop(5);

            // Act
            elevator.GoNextStop();

            // Assert
            Assert.DoesNotContain(5, elevator.FloorRequests);
        }

        [Fact]
        public void GoNextStop_ChangesDirectionAtTopFloor()
        {
            // Arrange
            var elevator = new Elevator(1, 10, 5, Direction.Up);
            elevator.AddStop(10);
            elevator.AddStop(3);

            // Act
            elevator.GoNextStop();

            // Assert
            Assert.Equal(Direction.Down, elevator.Direction);
        }

        [Fact]
        public void GoNextStop_ChangesDirectionAtBottomFloor()
        {
            // Arrange
            var elevator = new Elevator(1, 10, 5, Direction.Down);
            elevator.AddStop(0);
            elevator.AddStop(8);

            // Act
            elevator.GoNextStop();

            // Assert
            Assert.Equal(Direction.Up, elevator.Direction);
        }

        [Fact]
        public void FloorDistance_ReturnsZeroForCurrentFloor()
        {
            // Arrange
            var elevator = new Elevator(1, 10);

            // Act
            var distance = elevator.FloorDistance(elevator.CurrentFloor);

            // Assert
            Assert.Equal(0, distance);
        }

        [Fact]
        public void FloorDistance_ReturnsAbsoluteDistanceForStationaryElevator()
        {
            // Arrange
            var elevator = new Elevator(1, 10);
            elevator.CurrentFloor = 5;

            // Act
            var distance = elevator.FloorDistance(3);

            // Assert
            Assert.Equal(2, distance);
        }

        [Fact]
        public void FloorDistance_ReturnsDistanceToNextFloorInDirection()
        {
            // Arrange
            var elevator = new Elevator(1, 10);
            elevator.CurrentFloor = 5;
            elevator.Direction = Direction.Up;
            elevator.AddStop(7);

            // Act
            var distance = elevator.FloorDistance(6);

            // Assert
            Assert.Equal(1, distance);
        }

        [Fact]
        public void FloorDistance_ReturnsDistanceToPeakFloorAndRequestedFloor()
        {
            // Arrange
            var elevator = new Elevator(1, 10, 5, Direction.Up);
            elevator.AddStop(7);
            elevator.AddStop(3);

            // Act
            var distance = elevator.FloorDistance(2);

            // Assert
            Assert.Equal(7, distance);
        }
    }
}