using CasinoLibrary;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Sockets;

namespace CasinoLibraryTests
{
    [TestClass]
    public class WheelTests
    {
        [TestMethod]
        [Description("Ensures that the Wheel constructor correctly assigns the resultNum and initializes the outcome with the corresponding number.")]
        public void TestWheelConstructorAssignment()
        {
            // Arrange
            int expectedNumber = 5; // Example number for testing

            // Act
            Wheel wheel = new Wheel(expectedNumber);

            // Assert
            Assert.AreEqual(expectedNumber, wheel.resultNum, "Constructor should assign result number correctly.");
            Assert.AreEqual(expectedNumber, wheel.outcome.resultNum, "Constructor should initialize outcome with the correct result number.");
        }

        // Test ID: SERVER-TEST-022
        [TestMethod]
        [Description("Validates that the spin() method correctly sets the outcome to the pre-assigned resultNum.")]
        public void TestSpinOutcomeConsistency()
        {
            // Arrange
            int expectedNumber = 5; // Example number for testing
            Wheel wheel = new Wheel(expectedNumber);

            // Act
            wheel.spin(); // Assuming Outcome class has a method setOutcome(int num) that sets its internal state

            // Assert
            Assert.AreEqual(expectedNumber, wheel.outcome.resultNum, "Spin should set outcome result number to the wheel's result number.");
        }


    }
}