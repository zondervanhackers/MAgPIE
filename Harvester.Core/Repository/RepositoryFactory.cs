using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Core.Repository.Email;
using ZondervanLibrary.Harvester.Core.ScholarlyIQSushi;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.SharedLibrary.Factory;
using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Core.Repository
{
    public static class RepositoryFactory
    {
        public static ICounterRepository CreateCounterRepository(RepositoryArgumentsBase arguments)
        {
            switch (arguments)
            {
                case SushiCounterRepositoryArguments repositoryArguments:
                {
                    BasicHttpBinding binding = new BasicHttpBinding
                    {
                        MaxBufferPoolSize = Int32.MaxValue,
                        MaxBufferSize = Int32.MaxValue,
                        MaxReceivedMessageSize = Int32.MaxValue,
                        SendTimeout = new TimeSpan(0, 6, 0),
                        ReceiveTimeout = new TimeSpan(0, 6, 0)
                    };
                    
                    LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress> factory;
                    switch (repositoryArguments.Version)
                    {
                        case "3.0":
                        case "3":
                            factory = new LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress>(address => new Journal.Sushi_3_0.SushiServiceInterfaceClient(binding, address));
                            break;
                        case "4.0":
                        case "4":
                            factory = new LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress>(address => new Journal.Sushi_4_0.SushiServiceInterfaceClient(binding, address));
                            break;
                        case "4.1":
                        case "":
                        case null:
                            factory = new LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress>(address => new SushiServiceInterfaceClient(binding, address));
                            break;
                        default:
                            throw new NotImplementedException(@"Sushi Version is not supported: {repositoryArguments.Version}. Try 4.1");
                    }

                    return new SushiCounterRepository(repositoryArguments, factory);
                    
                }
                case EbscoHostCounterRepositoryArguments ebscoRepositoryArguments:
                {
                    BasicHttpBinding binding = new BasicHttpBinding
                    {
                        MaxBufferPoolSize = Int32.MaxValue,
                        MaxBufferSize = Int32.MaxValue,
                        MaxReceivedMessageSize = Int32.MaxValue,
                        SendTimeout = new TimeSpan(0, 6, 0),
                        ReceiveTimeout = new TimeSpan(0, 6, 0),
                    };

                    var sushiFactory = new LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress>(address => new SushiServiceInterfaceClient(binding, address));
                    var sushiRepoFactory = LambdaFactory.Wrap<SushiCounterRepository, SushiCounterRepositoryArguments>(s => new SushiCounterRepository(s, sushiFactory));

                    return new EbscoHostCounterRepository(ebscoRepositoryArguments, sushiRepoFactory);
                }
                case EmailRepositoryArguments emailRepositoryArguments:
                    return new EmailRepository(emailRepositoryArguments);
                case SushiMuseCounterRepositoryArguments sushiArgs:
                {
                    SushiCounterRepositoryArguments counterArgumentsnew = SushiCounterRepositoryArguments(sushiArgs);

                    CustomBinding binding = new CustomBinding(
                        new CustomTextMessageBindingElement("iso-8859-1", "text/xml", MessageVersion.Soap11),
                        new HttpTransportBindingElement());

                    var factory = new LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress>(address => new Journal.Sushi_3_0.SushiServiceInterfaceClient(binding, address));
                    
                    return new SushiCounterRepository(counterArgumentsnew, factory);
                }
                case SushiMpsCounterRepositoryArguments sushiArgs:
                {
                    SushiCounterRepositoryArguments counterArgumentsnew = SushiCounterRepositoryArguments(sushiArgs);

                    BasicHttpBinding binding = new BasicHttpBinding
                    {
                        MaxBufferPoolSize = Int32.MaxValue,
                        MaxBufferSize = Int32.MaxValue,
                        MaxReceivedMessageSize = Int32.MaxValue,
                        SendTimeout = new TimeSpan(0, 6, 0),
                        ReceiveTimeout = new TimeSpan(0, 6, 0),
                    };
                    
                    var factory = new LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress>(address => new MPSInsight.SushiServiceInterfaceClient(binding, address));

                    return new SushiCounterRepository(counterArgumentsnew, factory);
                }
                case SushiGaleCounterRepositoryArguments sushiArgs:
                {
                    SushiCounterRepositoryArguments counterArgumentsnew = SushiCounterRepositoryArguments(sushiArgs);

                    var binding  = new CustomBinding(new BasicHttpBinding("SushiService4"));
                    binding.Elements.Remove<TextMessageEncodingBindingElement>();
                    binding.Elements.Insert(0, new CustomTextMessageBindingElement(null, "text/html", MessageVersion.Soap11));

                    var httpTransportBindingElement = binding.Elements.Find<HttpTransportBindingElement>();
                    httpTransportBindingElement.MaxReceivedMessageSize = Int32.MaxValue;
                    binding.Elements.Remove<HttpTransportBindingElement>();
                    binding.Elements.Add(httpTransportBindingElement);

                    var factory = new LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress>(address => new Gale.SushiServiceInterfaceClient(binding, address));

                    return new SushiCounterRepository(counterArgumentsnew, factory);
                }
                case SushiJstorCounterRepositoryArguments sushiArgs:
                {
                    SushiCounterRepositoryArguments counterArgumentsnew = SushiCounterRepositoryArguments(sushiArgs);
                    
                    var binding = new CustomBinding(new BasicHttpBinding(BasicHttpSecurityMode.Transport) { MaxReceivedMessageSize = Int32.MaxValue, SendTimeout = TimeSpan.FromMinutes(4) });

                    binding.Elements.Remove<TextMessageEncodingBindingElement>();
                    binding.Elements.Insert(0, new CustomTextMessageBindingElement(null, null, MessageVersion.Soap11));

                    var factory = new LambdaFactory<ISushiServiceInterfaceClient, EndpointAddress>(address => new Journal.Sushi_4_0.SushiServiceInterfaceClient(binding, address));

                    return new SushiCounterRepository(counterArgumentsnew, factory);
                }

                default:
                    throw new NotImplementedException();
            }
        }

        private static SushiCounterRepositoryArguments SushiCounterRepositoryArguments(ISushiCounterRepositoryArguments sushiArgs)
        {
            return new SushiCounterRepositoryArguments
            {
                Name = sushiArgs.Name,
                RequestorID = sushiArgs.RequestorID,
                CustomerID = sushiArgs.CustomerID,
                ReleaseVersion = sushiArgs.ReleaseVersion,
                AvailableReports = sushiArgs.AvailableReports,
                Url = sushiArgs.Url,
            };
        }

        public static IDatabaseRepository<IStatisticsDataContext> CreateStatisticsRepository(RepositoryArgumentsBase arguments)
        {
            
            if (arguments is SqlServerDatabaseRepositoryArguments repositoryArguments)
            {
                LambdaFactory<IStatisticsDataContext, String> contextFactory = LambdaFactory.Wrap<IStatisticsDataContext, String>(s => new StatisticsDataContext(s));
                return new SqlServerDatabaseRepository<IStatisticsDataContext>(repositoryArguments, contextFactory);
            }

            throw new NotImplementedException();
        }

        public static IDatabaseRepository<IHarvesterDataContext> CreateHarvesterRepository(RepositoryArgumentsBase arguments)
        {
            if (arguments.GetType() == typeof(SqlServerDatabaseRepositoryArguments))
            {
                LambdaFactory<IHarvesterDataContext, String> contextFactory = LambdaFactory.Wrap<IHarvesterDataContext, String>(s => new HarvesterDataContext(s));
                return new SqlServerDatabaseRepository<IHarvesterDataContext>(arguments as SqlServerDatabaseRepositoryArguments, contextFactory);
            }

            throw new NotImplementedException();
        }

        public static IDirectoryRepository CreateDirectoryRepository(RepositoryArgumentsBase arguments)
        {
            switch (arguments)
            {
                case FtpDirectoryRepositoryArguments ftpRepositoryArguments:
                    return new FtpDirectoryRepository(ftpRepositoryArguments);

                case FolderDirectoryRepositoryArguments folderRepositoryArguments:
                    return new FolderDirectoryRepository(folderRepositoryArguments);

                case SftpDirectoryRepositoryArguments sftpRepositoryArguments:
                    return new SftpDirectoryRepository(sftpRepositoryArguments);
            }

            throw new NotImplementedException();
        }
    }

    public class CustomTextMessageEncoder : MessageEncoder
    {
        private readonly CustomTextMessageEncoderFactory factory;
        private readonly XmlWriterSettings writerSettings;

        public CustomTextMessageEncoder(CustomTextMessageEncoderFactory factory)
        {
            this.factory = factory;

            if (!string.IsNullOrEmpty(factory.CharSet))
                writerSettings = new XmlWriterSettings { Encoding = new UTF8Encoding(false) };
            ContentType = $"{this.factory.MediaType}; charset={factory.CharSet}";
        }

        public override string ContentType { get; }

        public override string MediaType => factory.MediaType;

        public override MessageVersion MessageVersion => factory.MessageVersion;

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            byte[] msgContents = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
            bufferManager.ReturnBuffer(buffer.Array);

            MemoryStream stream = new MemoryStream(msgContents);
            return ReadMessage(stream, int.MaxValue);
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            XmlReader reader = XmlReader.Create(stream);
            return Message.CreateMessage(reader, maxSizeOfHeaders, MessageVersion);
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(stream, writerSettings);
            message.WriteMessage(writer);
            writer.Close();

            byte[] messageBytes = stream.GetBuffer();
            int messageLength = (int)stream.Position;
            stream.Close();

            int totalLength = messageLength + messageOffset;
            byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
            Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

            ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
            return byteArray;
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            XmlWriter writer = XmlWriter.Create(stream, writerSettings);
            message.WriteMessage(writer);
            writer.Close();
        }

        public override Boolean IsContentTypeSupported(String contentType)
        {
            return true;/*contentType == ContentType
                || contentType.Replace(" ", "") == ContentType.Replace(" ", "")
                || contentType == "text/xml; charset=utf-8"
                || contentType == "application/soap+xml; charset=utf-8";*/
        }
    }

    public class CustomTextMessageEncoderFactory : MessageEncoderFactory
    {
        internal CustomTextMessageEncoderFactory(string mediaType, string charSet, MessageVersion version)
        {
            MessageVersion = version;
            MediaType = mediaType;
            CharSet = charSet;
            Encoder = new CustomTextMessageEncoder(this);
        }

        public override MessageEncoder Encoder { get; }

        public override MessageVersion MessageVersion { get; }

        internal string MediaType { get; }

        internal string CharSet { get; }
    }

    public class CustomTextMessageBindingElement : MessageEncodingBindingElement
    {
        private MessageVersion msgVersion;
        private string mediaType;
        private string encoding;
        private readonly XmlDictionaryReaderQuotas readerQuotas;

        CustomTextMessageBindingElement(CustomTextMessageBindingElement binding)
            : this(binding.Encoding, binding.MediaType, binding.MessageVersion)
        {
            readerQuotas = new XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.CopyTo(readerQuotas);
        }

        public CustomTextMessageBindingElement(string encoding, string mediaType,
            MessageVersion msgVersion)
        {
            if (encoding == null)
                encoding = "utf-8";

            if (mediaType == null)
                mediaType = "application/soap+xml";

            this.msgVersion = msgVersion ?? throw new ArgumentNullException(nameof(msgVersion));
            this.mediaType = mediaType;
            this.encoding = encoding;
            readerQuotas = new XmlDictionaryReaderQuotas();
        }

        public override MessageVersion MessageVersion
        {
            get => msgVersion;
            set => msgVersion = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string MediaType => mediaType;

        public string Encoding => encoding;

        // This encoder does not enforces any quotas for the unsecure messages. The 
        // quotas are enforced for the secure portions of messages when this encoder
        // is used in a binding that is configured with security. 
        public XmlDictionaryReaderQuotas ReaderQuotas => readerQuotas;

        #region IMessageEncodingBindingElement Members
        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new CustomTextMessageEncoderFactory(MediaType,
                Encoding, MessageVersion);
        }

        #endregion

        public override BindingElement Clone()
        {
            return new CustomTextMessageBindingElement(this);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
                return (T)(object)readerQuotas;
            
            return base.GetProperty<T>(context);
        }
    }
}