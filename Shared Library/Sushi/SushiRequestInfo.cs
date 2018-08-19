using System;
using System.Net;
namespace ZondervanLibrary.SharedLibrary.Sushi
{
    public class SushiRequestInfo
    {
        public string url { get; set; }
        public string requestorID { get; set; }
        public string customerID { get; set; }
        public DateTime date { get; set; }
        public NetworkCredential credential { get; set; }
        public SushiRequestInfo(string URL, string RequestorID, string CustomerID, DateTime rundate,NetworkCredential cred)
        {
            url = URL;
            requestorID = RequestorID;
            customerID = CustomerID;
            date = rundate;
            credential = cred;
        }
    }
}
