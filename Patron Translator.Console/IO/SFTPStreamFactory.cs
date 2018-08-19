using System;
using System.IO;
using System.Net;

using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace ZondervanLibrary.PatronTranslator.Console.IO
{
    public class SFTPStream : Stream
    {
        private Uri _uri;
        private readonly SftpClient _client;
        private readonly SftpFileStream _stream;
        private Boolean _disposed = false;

        public SFTPStream(Uri uri, ConnectionInfo connectionInfo, StreamMode streamMode)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (connectionInfo == null)
                throw new ArgumentNullException(nameof(connectionInfo));

            _uri = uri;
            _client = new SftpClient(connectionInfo);

            _client.Connect();

            String adjustedLocalPath = uri.LocalPath.Substring(1);

            _stream = streamMode == StreamMode.Read ? _client.OpenRead(adjustedLocalPath) : _client.Create(adjustedLocalPath);
        }

        public override Boolean CanRead => _stream.CanRead;

        public override Boolean CanSeek => _stream.CanSeek;

        public override Boolean CanWrite => _stream.CanWrite;

        public override Int64 Length => _stream.Length;

        public override Int64 Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _stream.Dispose();

                _client.Disconnect();
                _client.Dispose();
            }

            _disposed = true;
        }

        ~SFTPStream()
        {
            Dispose(false);
        }
    }

    public class SFTPStreamFactory : IStreamFactory
    {
        private readonly NetworkCredential _credentials;
        private readonly Uri _uri;

        public SFTPStreamFactory(Uri uri, NetworkCredential credentials)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            _uri = uri;
        }

        public Stream CreateInstance(StreamMode streamMode)
        {
            if (!Enum.IsDefined(typeof(StreamMode), streamMode))
                throw new ArgumentOutOfRangeException(nameof(streamMode));

            PasswordAuthenticationMethod passwordMethod = new PasswordAuthenticationMethod(_credentials.UserName, _credentials.Password);

            KeyboardInteractiveAuthenticationMethod keyboardMethod = new KeyboardInteractiveAuthenticationMethod(_credentials.UserName);
            keyboardMethod.AuthenticationPrompt += (sender, e) =>
            {
                foreach (AuthenticationPrompt prompt in e.Prompts)
                {
                    if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        prompt.Response = _credentials.Password;
                    }
                }
            };

            ConnectionInfo connectionInfo = new ConnectionInfo(_uri.Host, _credentials.UserName, passwordMethod, keyboardMethod);

            return new SFTPStream(_uri, connectionInfo, streamMode);
        }
    }
}
