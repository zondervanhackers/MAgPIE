using System;
using System.Data.Linq.Mapping;

using ZondervanLibrary.SharedLibrary;
using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.Harvester.Repository.Statistics.Circulation
{
    public enum MaterialFormat
    {
        Archv,
        ArtChapArtcl,
        Audiobook,
        AudiobookCassette,
        AudiobookCd,
        AudiobookLp,
        Book,
        BookBraille,
        BookContinuing,
        BookDigital,
        BookLargePrint,
        BookMic,
        BookThsis,
        CompFile,
        CompFileDigital,
        Game,
        Image2D,
        IntMM,
        Jrnl,
        JrnlDigital,
        Kit,
        Map,
        MsScr,
        Music,
        MusicCassette,
        MusicCd,
        MusicDigital,
        MusicLp,
        News,
        Object,
        Otr,
        Video,
        VideoBluray,
        VideoDvd,
        VideoFilm,
        VideoVhs,
        Vis,
        WebDigital
    }

    [Table(Name = "Circulation.BibliographicRecords")]
    public class BibliographicRecord : ViewModelBase, IDataModel
    {
        private Guid _bibliographicRecordID;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid BibliographicRecordID
        {
            get => _bibliographicRecordID;
            set => RaiseAndSetIfPropertyChanged(ref _bibliographicRecordID, value);
        }

        private String _oclcNumber;

        [Column]
        public String OclcNumber
        {
            get => _oclcNumber;
            set => RaiseAndSetIfPropertyChanged(ref _oclcNumber, value);
        }

        private String _title;

        [Column]
        public String Title
        {
            get => _title;
            set => RaiseAndSetIfPropertyChanged(ref _title, value);
        }

        private MaterialFormat _materialFormat;

        [Column(DbType = "NVarChar(32)")]
        public MaterialFormat MaterialFormat
        {
            get => _materialFormat;
            set => RaiseAndSetIfPropertyChanged(ref _materialFormat, value);
        }

        private String _author;

        [Column]
        public String Author
        {
            get => _author;
            set => RaiseAndSetIfPropertyChanged(ref _author, value);
        }

        private DateTime _runDate;

        [Column]
        public DateTime RunDate
        {
            get => _runDate;
            set => RaiseAndSetIfPropertyChanged(ref _runDate, value);
        }

        private DateTime _creationDate;

        [Column(IsDbGenerated = true)]
        public DateTime CreationDate
        {
            get => _creationDate;
            set => RaiseAndSetIfPropertyChanged(ref _creationDate, value);
        }

        private DateTime _modifiedDate;

        [Column(IsDbGenerated = true)]
        public DateTime ModifiedDate
        {
            get => _modifiedDate;
            set => RaiseAndSetIfPropertyChanged(ref _modifiedDate, value);
        }
    }
}
