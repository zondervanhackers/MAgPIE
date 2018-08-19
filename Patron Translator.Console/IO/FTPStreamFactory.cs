using System;
using System.IO;
using System.Net;

namespace ZondervanLibrary.PatronTranslator.Console.IO
{
    /// <summary>
    /// A factory to create a stream over FTP (File Transfer Protocol).
    /// </summary>
    public class FTPStreamFactory : IStreamFactory
    {
        private readonly String _uri;
        private readonly NetworkCredential _credentials;
        private readonly Boolean _enableSsl;

        public FTPStreamFactory(String uri, NetworkCredential credentials, Boolean enableSsl)
        {
            _uri = uri;
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            _enableSsl = enableSsl;
        }

        public Stream CreateInstance(StreamMode streamMode)
        {
            if (!Enum.IsDefined(typeof(StreamMode), streamMode))
                throw new ArgumentOutOfRangeException(nameof(streamMode));

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_uri);
            request.Credentials = _credentials;
            request.EnableSsl = _enableSsl;

            switch (streamMode)
            {
                case StreamMode.Read:
                    request.Method = WebRequestMethods.Ftp.DownloadFile;
                    break;
                case StreamMode.Write:
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    break;
            }

            if (streamMode == StreamMode.Write)
            {
                return request.GetRequestStream();
            }
            else
            {
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return response.GetResponseStream();
            }
        }
    }
}
