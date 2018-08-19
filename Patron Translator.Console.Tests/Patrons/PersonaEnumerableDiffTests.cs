namespace ZondervanLibrary.PatronTranslator.Console.Tests.Patrons
{
    public class PersonaEnumerableDiffTests
    {
        ///// <summary>
        ///// The barcode should uniquely identify each Persona.
        ///// </summary>
        //[Fact]
        //public void PersonaEnumerableDiff_Should_Be_Keyed_By_Barcode()
        //{
        //    // Arrange
        //    var p1 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1" } };
        //    var p2 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1" } };
        //    var oldEnumerable = new List<Persona>() { p1, p2 };
        //    var newEnumerable = new List<Persona>();

        //    PersonaEnumerableDiff diff = new PersonaEnumerableDiff();

        //    // Act
        //    var exception = Record.Exception(() => diff.ComputeDiff(oldEnumerable, newEnumerable).ToArray());

        //    // Assert
        //    Assert.Equal(typeof(ArgumentException), exception.GetType());
        //}

        ///// <summary>
        ///// Oclc's input format specifies that having a record present means it should be added/updated.
        ///// </summary>
        //[Fact]
        //public void PersonaEnumerableDiff_Should_Include_Added_Records_In_Output()
        //{
        //    // Arrange
        //    var p1 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1" } };
        //    var oldEnumerable = new List<Persona>();
        //    var newEnumerable = new List<Persona>() { p1 };

        //    PersonaEnumerableDiff diff = new PersonaEnumerableDiff();

        //    // Act
        //    var result = diff.ComputeDiff(oldEnumerable, newEnumerable).ToArray();
            
        //    // Assert
        //    Assert.Equal(1, result.Count());
        //    Assert.Equal(p1, result.First());
        //}

        ///// <summary>
        ///// Oclc's input format specifies that having a record present with its exipiration date set to a date in the past flags the record for deletion.
        ///// </summary>
        //[Fact]
        //public void PersonaEnumerableDiff_Should_Include_Removed_Records_In_Output()
        //{
        //    // Arrange
        //    var p1 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1" } };
        //    var oldEnumerable = new List<Persona>() { p1 };
        //    var newEnumerable = new List<Persona>();

        //    PersonaEnumerableDiff diff = new PersonaEnumerableDiff();

        //    // Act
        //    var result = diff.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

        //    // Assert
        //    Assert.Equal(1, result.Count());
        //    Assert.True(result.First().wmsCircPatronInfo.circExpirationDateSpecified);
        //    Assert.True(result.First().wmsCircPatronInfo.circExpirationDate <= DateTime.Now);
        //}

        ///// <summary>
        ///// Oclc's input format specifies that having a record present means it should be added/updated.
        ///// </summary>
        //[Fact]
        //public void PersonaEnumerableDiff_Should_Include_Changed_Records_In_Output()
        //{
        //    // Arrange
        //    var p1 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1", barcodeStatus = "A" } };
        //    var p2 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1", barcodeStatus = "B" } };
        //    var oldEnumerable = new List<Persona>() { p1 };
        //    var newEnumerable = new List<Persona>() { p2 };

        //    PersonaEnumerableDiff diff = new PersonaEnumerableDiff();

        //    // Act
        //    var result = diff.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

        //    // Assert
        //    Assert.Equal(1, result.Count());
        //    Assert.Equal(p2.wmsCircPatronInfo.barcodeStatus, result.First().wmsCircPatronInfo.barcodeStatus);
        //}

        ///// <summary>
        ///// No need to include records that have not changed.
        ///// </summary>
        //[Fact]
        //public void PersonaEnumerableDiff_Should_Not_Include_Unchanged_Records_In_Output()
        //{
        //    // Arrange
        //    var p1 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1", barcodeStatus = "A" } };
        //    var p2 = new Persona() { wmsCircPatronInfo = new WmsCircPatronInfo() { barcode = "1", barcodeStatus = "A" } };
        //    var oldEnumerable = new List<Persona>() { p1 };
        //    var newEnumerable = new List<Persona>() { p2 };

        //    PersonaEnumerableDiff diff = new PersonaEnumerableDiff();

        //    // Act
        //    var result = diff.ComputeDiff(oldEnumerable, newEnumerable).ToArray();

        //    // Assert
        //    Assert.Equal(0, result.Count());
        //}
    }
}
