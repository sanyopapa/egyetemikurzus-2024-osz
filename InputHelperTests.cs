using System;
using System.IO;
using Xunit;
using BFAM77;

namespace BFAM77.Tests
{
    public class InputHelperTests
    {
        [Fact]
        public void GetStringInput_ReturnsInput()
        {
            // Arrange
            var input = "Test Input";
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            // Act
            var result = InputHelper.GetStringInput("Prompt: ");

            // Assert
            Assert.Equal(input, result);
        }

        [Fact]
        public void GetIntInput_ReturnsInput()
        {
            // Arrange
            var input = "123";
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            // Act
            var result = InputHelper.GetIntInput("Prompt: ");

            // Assert
            Assert.Equal(123, result);
        }

        [Fact]
        public void GetBoolInput_ReturnsTrueForIgen()
        {
            // Arrange
            var input = "igen";
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            // Act
            var result = InputHelper.GetBoolInput("Prompt: ");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetBoolInput_ReturnsFalseForNem()
        {
            // Arrange
            var input = "nem";
            var stringReader = new StringReader(input);
            Console.SetIn(stringReader);

            // Act
            var result = InputHelper.GetBoolInput("Prompt: ");

            // Assert
            Assert.False(result);
        }
    }
}
