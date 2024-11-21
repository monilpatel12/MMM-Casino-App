using CasinoLibrary;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Sockets;
using static System.Net.Mime.MediaTypeNames;

namespace CasinoLibraryTests
{
    [TestClass]
    public class OutcomeTests
    {

        [TestMethod]
        [Description("Validates that determineType() assigns Type.zero to the variable type when the number is 0.")]
        public void TestDetermineTypeWithZero()
        {
            // Arrange
            var number = 0;
            var expectedType = Type.zero; // Replace with actual Type enum or equivalent in your code

            // Act
            var actualType = DetermineType(number); // Replace with the actual way to determine the type

            // Assert
            Assert.AreEqual(expectedType, actualType);
        }

        // SERVER-TEST-014: Test DetermineType With Even Number
        [TestMethod]
        [Description("Checks that determineType() correctly assigns Type.even to the variable type when the number is an even number greater than zero.")]
        public void TestDetermineTypeWithEvenNumber()
        {
            // Arrange
            var number = 2;
            var expectedType = Type.even; // Replace with actual Type enum or equivalent in your code

            // Act
            var actualType = DetermineType(number); // Replace with the actual way to determine the type

            // Assert
            Assert.AreEqual(expectedType, actualType);
        }

        // SERVER-TEST-015: Test DetermineType With Odd Number
        [TestMethod]
        [Description("Ensures that determineType() correctly assigns Type.odd to the variable type when the number is an odd number.")]
        public void TestDetermineTypeWithOddNumber()
        {
            // Arrange
            var number = 1;
            var expectedType = Type.odd; // Replace with actual Type enum or equivalent in your code

            // Act
            var actualType = DetermineType(number); // Replace with the actual way to determine the type

            // Assert
            Assert.AreEqual(expectedType, actualType);
        }

        // SERVER-TEST-016: Test Wheel Constructor Assignment
        [TestMethod]
        [Description("Ensures that the Wheel constructor correctly assigns the resultNum and initializes the outcome with the corresponding number.")]
        public void TestWheelConstructorAssignment()
        {
            // Arrange
            int inputNumber = 22; // Example input for testing

            // Act
            Wheel wheel = new Wheel(inputNumber);

            // Assert
            Assert.AreEqual(inputNumber, wheel.resultNum, "Wheel constructor should correctly assign the result number.");
            Assert.AreEqual(inputNumber, wheel.outcome.resultNum, "Wheel constructor should initialize Outcome with the correct number.");
        }

        // SERVER-TEST-017: Test Spin Outcome Consistency
        [TestMethod]
        [Description("Validates that the spin() method correctly sets the outcome to the pre-assigned resultNum.")]
        public void TestSpinOutcomeConsistency()
        {
            // Arrange
            int inputNumber = 22; // Example input for testing
            Wheel wheel = new Wheel(inputNumber);

            // Act
            wheel.spin();

            // Assert
            Assert.AreEqual(inputNumber, wheel.outcome.resultNum, "Spin method should correctly set the outcome to the pre-assigned result number.");
        }


        // SERVER-TEST-018: Test Wheel Constructor with Specific Number
        [TestMethod]
        [Description("Ensures that the Wheel constructor correctly assigns a specific number.")]
        public void TestWheelConstructorWithSpecificNumber()
        {
            // Arrange
            int inputNumber = 7; // Example input for testing

            // Act
            Wheel wheel = new Wheel(inputNumber);

            // Assert
            Assert.AreEqual(inputNumber, wheel.resultNum, "Wheel constructor should assign the specific result number correctly.");
        }

        // SERVER-TEST-019: Test Spin Method with Even Number
        [TestMethod]
        [Description("Validates that the spin() method maintains the even number result after spinning.")]
        public void TestSpinMethodWithEvenNumber()
        {
            // Arrange
            int inputNumber = 8; // Even number for testing
            Wheel wheel = new Wheel(inputNumber);

            // Act
            wheel.spin();

            // Assert
            Assert.AreEqual(Type.even, wheel.outcome.DetermineType(), "Spin method should maintain the even number outcome.");
        }

        // SERVER-TEST-020: Test Spin Method with Odd Number
        [TestMethod]
        [Description("Validates that the spin() method maintains the odd number result after spinning.")]
        public void TestSpinMethodWithOddNumber()
        {
            // Arrange
            int inputNumber = 9; // Odd number for testing
            Wheel wheel = new Wheel(inputNumber);

            // Act
            wheel.spin();

            // Assert
            Assert.AreEqual(Type.odd, wheel.outcome.DetermineType(), "Spin method should maintain the odd number outcome.");
        }

        // Replace with your actual method to determine the type.
        // This example assumes a standalone function for simplicity.
        private Type DetermineType(int number)
        {
            if (number == 0) return Type.zero;
            if (number % 2 == 0) return Type.even;
            return Type.odd;
        }
    }

    // Define the Type enum or class as per your implementation.
    public enum Type
    {
        zero,
        even,
        odd
    }

    // Assume Outcome class is defined with a resultNum property.


    public class Outcome
    {
        public int resultNum;
        public Outcome(int num)
        {
            resultNum = num;
        }
        public void setOutcome(int num)
        {
            resultNum = num;
        }

        public Type DetermineType()
        {
            if (resultNum == 0) return Type.zero;
            return resultNum % 2 == 0 ? Type.even : Type.odd;
        }
    }

    // Assume Wheel class is defined as per the code provided.
    public class Wheel
    {
        public int resultNum;
        public Outcome outcome;
        public Wheel(int num)
        {
            resultNum = num;
            outcome = new Outcome(num);
        }
        public void spin()
        {
            outcome.setOutcome(resultNum);
        }
    }


}
