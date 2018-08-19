namespace ZondervanLibrary.Harvester.Core.Tests.Repository.Counter
{
    public class SushiCounterRepositoryTests
    {
        //public static readonly IEnumerable<object[]> RequestMethods = new []
        //{
        //    new object[] { new Func<SushiCounterRepository, IEnumerable<object>>(r => r.RequestJournalRecords(DateTime.Now)) },
        //    new object[] { new Func<SushiCounterRepository, IEnumerable<object>>(r => r.RequestDatabaseRecords(DateTime.Now)) }
        //};

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryConfigurationException_When_Uri_Cannot_Be_Located(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var factory = LambdaFactory1.Wrap((EndpointAddress s) =>
        //    {
        //        var sushiInterfaceMock = new Mock<ISushiServiceInterfaceClient>();
        //        sushiInterfaceMock.Setup(c => c.GetReport(It.IsAny<ReportRequest>())).Throws(new FaultException());
        //        return sushiInterfaceMock.Object;
        //    });

        //    var args = new SushiJournalRepositoryArguments() { Url = "http://bad.com", CustomerID = "1", RequestorID = "1" };
        //    var repository = new SushiJournalRepository(args, factory);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryConfigurationException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryIOException_When_Network_Unavailable(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var factory = LambdaFactory1.Wrap((EndpointAddress s) =>
        //    {
        //        var sushiInterfaceMock = new Mock<ISushiServiceInterfaceClient>();
        //        sushiInterfaceMock.Setup(c => c.GetReport(It.IsAny<ReportRequest>())).Throws(new EndpointNotFoundException());
        //        return sushiInterfaceMock.Object;
        //    });

        //    var args = new SushiJournalRepositoryArguments() { Url = "http://bad.com", CustomerID = "1", RequestorID = "1" };
        //    var repository = new SushiJournalRepository(args, factory);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryIOException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryIOException_When_Request_Timed_Out(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var factory = LambdaFactory1.Wrap((EndpointAddress s) =>
        //    {
        //        var sushiInterfaceMock = new Mock<ISushiServiceInterfaceClient>();
        //        sushiInterfaceMock.Setup(c => c.GetReport(It.IsAny<ReportRequest>())).Throws(new TimeoutException());
        //        return sushiInterfaceMock.Object;
        //    });

        //    var args = new SushiJournalRepositoryArguments() { Url = "http://bad.com", CustomerID = "1", RequestorID = "1" };
        //    var repository = new SushiJournalRepository(args, factory);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryIOException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryIOException_When_Server_Too_Busy(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var factory = LambdaFactory.Wrap((EndpointAddress s) =>
        //    {
        //        var sushiInterfaceMock = new Mock<ISushiServiceInterfaceClient>();
        //        sushiInterfaceMock.Setup(c => c.GetReport(It.IsAny<ReportRequest>())).Throws(new ServerTooBusyException());
        //        return sushiInterfaceMock.Object;
        //    });

        //    var args = new SushiJournalRepositoryArguments() { Url = "http://bad.com", CustomerID = "1", RequestorID = "1" };
        //    var repository = new SushiJournalRepository(args, factory);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryIOException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //private SushiJournalRepository CreateErrorResponse(String message, Int32 number)
        //{
        //    var factory = LambdaFactory1.Wrap((EndpointAddress s) =>
        //    {
        //        var sushiInterfaceMock = new Mock<ISushiServiceInterfaceClient>();
        //        sushiInterfaceMock.Setup(c => c.GetReport(It.IsAny<ReportRequest>())).Returns(new CounterReportResponse()
        //        {
        //            Exception = new Sushi.Exception[] { new Sushi.Exception()
        //            {
        //                Created = DateTime.Now,
        //                Message = message,
        //                Number = number,
        //                Severity = ExceptionSeverity.Error
        //            }}
        //        });
        //        return sushiInterfaceMock.Object;
        //    });

        //    var args = new SushiJournalRepositoryArguments() { Url = "http://bad.com", CustomerID = "1", RequestorID = "1" };
        //    return new SushiJournalRepository(args, factory);
        //}

        //private SushiJournalRepository CreateStandardResponse(ReportItem reportItem)
        //{
        //    var factory = LambdaFactory1.Wrap((EndpointAddress s) =>
        //    {
        //        var sushiInterfaceMock = new Mock<ISushiServiceInterfaceClient>();
        //        sushiInterfaceMock.Setup(c => c.GetReport(It.IsAny<ReportRequest>())).Returns(new CounterReportResponse()
        //        {
        //            Report = new Report[] { new Report()
        //            {
        //                Customer = new ReportCustomer[] { new ReportCustomer() 
        //                {
        //                    ReportItems = new ReportItem[] { reportItem }
        //                }}
        //            }}
        //        });
        //        return sushiInterfaceMock.Object;
        //    });

        //    var args = new SushiJournalRepositoryArguments() { Url = "http://bad.com", CustomerID = "1", RequestorID = "1" };
        //    return new SushiJournalRepository(args, factory);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryConfigurationException_When_RequestorID_Is_Not_Valid(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var repository = CreateErrorResponse("Requestor Not Authorized to Access Service", 2000);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryConfigurationException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryConfigurationException_When_CustomerID_Is_Not_Valid(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var repository = CreateErrorResponse("Requestor is Not Authorized to Access Usage for Institution", 2010);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryConfigurationException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryConfigurationException_When_Report_Not_Supported(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var repository = CreateErrorResponse("Report Not Supported", 3000);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryConfigurationException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryConfigurationException_When_Report_Version_Not_Supported(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var repository = CreateErrorResponse("Report Version Not Supported", 3010);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryConfigurationException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryIOException_When_Server_Is_Too_Busy(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var repository = CreateErrorResponse("Server is too busy to handle request. Try Later.", 1010);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryIOException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryIOException_When_Client_Has_Made_Too_Many_Requests(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var repository = CreateErrorResponse("Client has made too many requests.", 1020);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryIOException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Throw_RepositoryImplementationException_When_Error_Unrecognized(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var repository = CreateErrorResponse("Client has made too many requests.", 9999);

        //    // Act
        //    var record = Record.Exception(() => method(repository).ToArray()) as RepositoryImplementationException;

        //    // Assert
        //    Assert.NotNull(record);
        //}

        //[Theory]
        //[MemberData("RequestMethods")]
        //public void X_Should_Return_Empty_Enumerable_When_Exception_No_Data_For_Date_Range_Occurs(Func<SushiJournalRepository, IEnumerable<object>> method)
        //{
        //    // Arrange
        //    var repository = CreateErrorResponse("No Data for this Date Range.", 3030);

        //    // Act
        //    var result = method(repository).ToArray();

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(0, result.Count());
        //}

        //[Fact]
        //public void RequestJournalRecords_Should_Return_Null_For_PrintOrOnline_ISSN_If_Not_Present()
        //{
        //    // Arrange
        //    var repository = CreateStandardResponse(new ReportItem()
        //    {
        //        ItemIdentifier = new Identifier[] { },
        //        ItemName = "JournalName",
        //        ItemPlatform = "DatabaseName",
        //        ItemPerformance = new Metric[] { new Metric()
        //        {
        //            Instance = new PerformanceCounter[] { new PerformanceCounter() { MetricType = MetricType.ft_total, Count = "1" }}
        //        }}
        //    });

        //    // Act
        //    var response = repository.RequestJournalRecords(DateTime.Now).ToArray();

        //    // Assert
        //    Assert.NotNull(response);
        //    Assert.True(response.Length > 0);
        //    Assert.Null(response.First().PrintIssn);
        //    Assert.Null(response.First().OnlineIssn);
        //}

        //[Fact]
        //public void RequestDatabaseRecords_Should_Be_Able_To_Handle_Metrics_Not_Being_In_Same_ItemPerformance()
        //{
        //    // Arrange
        //    var searches = 1;
        //    var recordViews = 2;
        //    var resultClicks = 3;

        //    var repository = CreateStandardResponse(new ReportItem()
        //    {
        //        ItemIdentifier = new Identifier[] { },
        //        ItemName = "JournalName",
        //        ItemPlatform = "DatabaseName",
        //        ItemPerformance = new Metric[] { new Metric()
        //        {
        //            Instance = new PerformanceCounter[] { new PerformanceCounter() { MetricType = MetricType.record_view, Count = recordViews.ToString() }, new PerformanceCounter() { MetricType = MetricType.result_click, Count = resultClicks.ToString() } }
        //        }, new Metric()
        //        {
        //            Instance = new PerformanceCounter[] { new PerformanceCounter() { MetricType = MetricType.search_reg, Count = searches.ToString() }}
        //        }}
        //    });

        //    // Act
        //    var response = repository.RequestDatabaseRecords(DateTime.Now).ToArray();

        //    // Assert
        //    Assert.NotNull(response);
        //    Assert.True(response.Length > 0);
        //    Assert.Equal(searches, response.First().SearchCount);
        //    Assert.Equal(recordViews, response.First().RecordViewCount);
        //    Assert.Equal(resultClicks, response.First().ResultClickCount);
        //}

        //[Fact]
        //public void RequestDatabaseRecords_Should_Return_Null_For_Metrics_Not_Present_In_Source()
        //{
        //    // Arrange
        //    var repository = CreateStandardResponse(new ReportItem()
        //    {
        //        ItemIdentifier = new Identifier[] { },
        //        ItemName = "JournalName",
        //        ItemPlatform = "DatabaseName",
        //        ItemPerformance = new Metric[] { new Metric()
        //        {
        //            Instance = new PerformanceCounter[] { }
        //        }, new Metric()
        //        {
        //            Instance = new PerformanceCounter[] { }
        //        }}
        //    });

        //    // Act
        //    var response = repository.RequestDatabaseRecords(DateTime.Now).ToArray();

        //    // Assert
        //    Assert.NotNull(response);
        //    Assert.Equal(1, response.Count());
        //    Assert.Null(response.First().SearchCount);
        //    Assert.Null(response.First().RecordViewCount);
        //    Assert.Null(response.First().ResultClickCount);
        //}
    }
}
