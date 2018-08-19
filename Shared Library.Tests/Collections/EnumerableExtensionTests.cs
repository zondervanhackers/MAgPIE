namespace ZondervanLibrary.SharedLibrary.Tests.Collections
{
    public class EnumerableExtensionTests
    {
        //[Fact]
        //public void Fork_Output_Must_Be_The_Same_As_Input()
        //{
        //    // Arrange
        //    List<int> output = null;
        //    bool run = false;
        //    var list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        //    // Act
        //    list.Fork(l =>
        //    {
        //        run = true;
        //        output = l.ToList();
        //    });

        //    // Assert
        //    Assert.Equal(output, list);
        //}

        //[Fact]
        //public void Fork_Must_Allow_Partial_Enumeration()
        //{
        //    // Arrange
        //    List<int> output = null;
        //    var list = new List<int>() { 1, 2, 3, 4, 5, 6 };

        //    // Act
        //    list.Fork(l =>
        //    {
        //        output = l.Take(3).ToList();
        //    });

        //    // Assert
        //    Assert.Equal(new List<int>() { 1, 2, 3 }, output);
        //}

        //[Fact]
        //public void Fork_Multiple_Output_Must_All_Receive_The_Same_Enumerable()
        //{
        //    // Arrange
        //    List<int> output1 = null;
        //    List<int> output2 = null;
        //    var list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        //    // Act
        //    list.Fork(l =>
        //    {
        //        output1 = l.ToList();
        //    }, l =>
        //    {
        //        output2 = l.ToList();
        //    });

        //    // Assert
        //    Assert.Equal(list, output1);
        //    Assert.Equal(list, output2);
        //}

        //[Fact]
        //public void Fork_Multiple_Should_Be_Able_To_Handle_Random_Timings()
        //{
        //    // Arrange
        //    List<int> output1 = new List<int>();
        //    List<int> output2 = new List<int>();
        //    List<int> output3 = new List<int>();
        //    Random random = new Random();
        //    var list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

        //    // Act
        //    list.Fork(l =>
        //    {
        //        foreach (var x in l)
        //        {
        //            System.Threading.Thread.Sleep(random.Next(5, 100));
        //            output1.Add(x);
        //        }
        //    }, l =>
        //    {
        //        foreach (var x in l)
        //        {
        //            System.Threading.Thread.Sleep(random.Next(5, 100));
        //            output2.Add(x);
        //        }
        //    }, l =>
        //    {
        //        foreach (var x in l)
        //        {
        //            System.Threading.Thread.Sleep(random.Next(5, 100));
        //            output3.Add(x);
        //        }
        //    });

        //    // Assert
        //    Assert.Equal(list, output1);
        //    Assert.Equal(list, output2);
        //    Assert.Equal(list, output3);
        //}
    }
}
