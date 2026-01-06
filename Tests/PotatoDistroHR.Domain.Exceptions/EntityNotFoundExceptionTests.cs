using Xunit;
using System;
using PotatoDistroHR.Domain.Exceptions;

namespace Tests.PotatoDistroHR.Domain.Exceptions
{
    /// <summary>
    /// Unit tests for EntityNotFoundException
    /// </summary>
    public class EntityNotFoundExceptionTests
    {
        [Fact]
        public void Constructor_WithEntityNameAndId_ShouldSetMessage()
        {
            // Arrange
            string entityName = "Employee";
            int id = 123;

            // Act
            var exception = new EntityNotFoundException(entityName, id);

            // Assert
            Assert.Equal("Employee with ID 123 was not found", exception.Message);
        }

        [Fact]
        public void Constructor_WithMessage_ShouldSetMessage()
        {
            // Arrange
            string expectedMessage = "Custom error message";

            // Act
            var exception = new EntityNotFoundException(expectedMessage);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void Constructor_WithMessageAndInnerException_ShouldSetBoth()
        {
            // Arrange
            string expectedMessage = "Outer exception message";
            var innerException = new InvalidOperationException("Inner exception");

            // Act
            var exception = new EntityNotFoundException(expectedMessage, innerException);

            // Assert
            Assert.Equal(expectedMessage, exception.Message);
            Assert.NotNull(exception.InnerException);
            Assert.Equal("Inner exception", exception.InnerException.Message);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public void Exception_ShouldBeOfTypeException()
        {
            // Arrange & Act
            var exception = new EntityNotFoundException("Test message");

            // Assert
            Assert.IsAssignableFrom<Exception>(exception);
        }

        [Fact]
        public void Constructor_WithEntityNameAndZeroId_ShouldFormatCorrectly()
        {
            // Arrange
            string entityName = "Department";
            int id = 0;

            // Act
            var exception = new EntityNotFoundException(entityName, id);

            // Assert
            Assert.Equal("Department with ID 0 was not found", exception.Message);
        }

        [Fact]
        public void Constructor_WithEntityNameAndNegativeId_ShouldFormatCorrectly()
        {
            // Arrange
            string entityName = "Account";
            int id = -1;

            // Act
            var exception = new EntityNotFoundException(entityName, id);

            // Assert
            Assert.Equal("Account with ID -1 was not found", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyMessage_ShouldAcceptEmptyString()
        {
            // Arrange & Act
            var exception = new EntityNotFoundException(string.Empty);

            // Assert
            Assert.Equal(string.Empty, exception.Message);
        }

        [Fact]
        public void Constructor_WithNullInnerException_ShouldAcceptNull()
        {
            // Arrange
            string message = "Test message";

            // Act
            var exception = new EntityNotFoundException(message, null!);

            // Assert
            Assert.Equal(message, exception.Message);
            Assert.Null(exception.InnerException);
        }

        [Fact]
        public void Exception_ShouldBeThrowable()
        {
            // Arrange
            var exception = new EntityNotFoundException("Test", 1);

            // Act
            Action act = () => throw exception;

            // Assert
            var thrownException = Assert.Throws<EntityNotFoundException>(act);
            Assert.Equal("Test with ID 1 was not found", thrownException.Message);
        }

        [Fact]
        public void Constructor_WithDifferentEntityTypes_ShouldFormatCorrectly()
        {
            // Arrange & Act
            var employeeException = new EntityNotFoundException("Employee", 100);
            var departmentException = new EntityNotFoundException("Department", 200);
            var accountException = new EntityNotFoundException("Account", 300);

            // Assert
            Assert.Contains("Employee", employeeException.Message);
            Assert.Contains("100", employeeException.Message);
            Assert.Contains("Department", departmentException.Message);
            Assert.Contains("200", departmentException.Message);
            Assert.Contains("Account", accountException.Message);
            Assert.Contains("300", accountException.Message);
        }
    }
}
