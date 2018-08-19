using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using ZondervanLibrary.PatronTranslator.Console.Patrons;

namespace ZondervanLibrary.PatronTranslator.Console.Tests.Patrons
{
    public class PersonasDifferentiatorTests
    {
        [Fact]
        public void ComputeDifference_Should_Return_Empty_List_For_Empty_List_Input()
        {
            // Arrange
            IEnumerable<Persona> list1 = new List<Persona>() { };
            IEnumerable<Persona> list2 = new List<Persona>() { };
            PersonasDifferentiator differentiator = new PersonasDifferentiator();

            // Act
            IEnumerable<Persona> result = differentiator.ComputeDifference(list1, list2);

            // Assert
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void ComputeDifference_Should_Add_Items_Only_In_New_List_To_Output()
        {
            // Arrange
            Persona p1 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1" } };
            Persona p2 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "2" } };
            IEnumerable<Persona> list1 = new List<Persona>() { p1 };
            IEnumerable<Persona> list2 = new List<Persona>() { p1, p2 };
            PersonasDifferentiator differentiator = new PersonasDifferentiator();

            // Act
            IEnumerable<Persona> result = differentiator.ComputeDifference(list1, list2);

            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal("2", result.First().wmsCircPatronInfo.barcode);
        }

        [Fact]
        public void ComputeDifference_Should_Mark_Items_Only_In_Old_List_For_Deletion()
        {
            // Arrange
            Persona p1 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1" } };
            Persona p2 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "2" } };
            IEnumerable<Persona> list1 = new List<Persona>() { p1, p2 };
            IEnumerable<Persona> list2 = new List<Persona>() { p1 };
            PersonasDifferentiator differentiator = new PersonasDifferentiator();
            DateTime preTime = DateTime.Now;

            // Act
            IEnumerable<Persona> result = differentiator.ComputeDifference(list1, list2);
            DateTime postTime = DateTime.Now;

            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal("2", result.First().wmsCircPatronInfo.barcode);

            // Expiration Date should be set
            Assert.Equal(true, result.First().wmsCircPatronInfo.circExpirationDate.Subtract(preTime).Ticks >= 0);
            Assert.Equal(true, postTime.Subtract(result.First().wmsCircPatronInfo.circExpirationDate).Ticks >= 0);

            // Expiration Date Specified should be true
            Assert.Equal(true, result.First().wmsCircPatronInfo.circExpirationDateSpecified);
        }

        [Fact]
        public void ComputeDifference_Should_Mark_For_Update_On_New_Persona_Updated()
        {
            // Arrange
            Persona p1 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1" } };
            Persona p2 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1" }, preferences = new Preferences() { preferredNotificationMethod = new NotificationMethod() { label = "ASDF" } } };
            IEnumerable<Persona> list1 = new List<Persona>() { p1 };
            IEnumerable<Persona> list2 = new List<Persona>() { p2 };
            PersonasDifferentiator differentiator = new PersonasDifferentiator();
            DateTime preTime = DateTime.Now;

            // Act
            IEnumerable<Persona> result = differentiator.ComputeDifference(list1, list2);
            DateTime postTime = DateTime.Now;

            // Assert
            Assert.Equal(1, result.Count());
            Assert.Equal("1", result.First().wmsCircPatronInfo.barcode);

            // Ensure nested element is updated
            Assert.Equal("ASDF", result.First().preferences.preferredNotificationMethod.label);
        }
    }
}
