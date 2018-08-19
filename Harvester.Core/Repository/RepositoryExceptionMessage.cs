using System;

namespace ZondervanLibrary.Harvester.Core.Repository
{
    public static class RepositoryExceptionMessage
    {
        public static string HostNotFound_1 = "The host '{0}' could not be found.";
        public static string UnrecognizedException = "An unrecognized exception occured.";
        public static string UnrecognizedError = "An unrecognized error was return by the host.";
        public static string NetworkUnavailable_1 = "The host '{0}' could not be reached because the network is currently unavailable.";
        public static Int32  NetworkUnavailableWait = 900;
        public static string InternetUnavailable_1 = "The host '{0}' could not be reached because the internet is currently unavailable.";
        public static string AuthenticationError = "The authentication credentials are incorrect.";
        public static string DirectoryNotFound_1 = "The Directory {0} could not be found";
        public static string FileNotFound_1 = "The file '{0}' could not be found.";
        public static string FileLocked_1 = "The file '{0}' is being used and cannot be deleted.";
        public static string PermissionDenied_1 = "Permission was denied on file '{0}'.";
        public static string FileAlreadyExists_1 = "The file '{0}' already exists.";
        public static string Timeout = "The operation on the host timed out.";
        public static Int32  TimedOutWait = 900;
        public static string MechanismFailed_1 = "An error occured while accessing the file '{0}'.";
        public static string InvalidCharacter_1 = "The file name '{0}' contains an invalid character.";
        public static string InvalidFormat_1 = "Part of the fileName '{0}' is in an invalid format.";
        public static string TooLong_1 = "Part of the fileName '{0}' is too long.";
        public static string ConnectionClosed = "The connection to the host was unexpectedly closed.";
    }
}
