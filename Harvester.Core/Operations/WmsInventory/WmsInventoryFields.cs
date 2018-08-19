// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace ZondervanLibrary.Harvester.Core.Operations.WmsInventory
{
    public enum CurrentStatus
    {
        Available,
        Withdrawn,
        ClaimedReturned,
        Recalled,
        Transit,
        OnHold,
        OnLoan,
        ClaimedNeverHad,
        Lost,
        Missing,
        Dispatched,
        Unavailable,
        InProcessing
    }

    public enum HoldingLocation
    {
        ITUM,
        ITUZ
    }

    public enum InstitutionSymbol
    {
        ITU
    }

    public enum ItemType
    {
        Volume,
        CircManaged
    }

    public enum MaterialFormat
    {
        Archv,
        ArtChap,
        ArtChapArtcl,
        ArtChapChptr,
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
        IntMMDigital,
        Jrnl,
        JrnlDigital,
        Kit,
        Map,
        MsScr,
        MsScrDigital,
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

    public enum ShelvingLocation
    {
        AskLibraryStaffForAssistance,
        AudioBookCollection,
        AyresCollection,
        BoardGamesCollection,
        BrownCollection,
        CdCollection,
        CheckInProcessing,
        CheckOutDeskReserves,
        CurriculumResourcesCollection,
        DvdCollection,
        FundingInformationNetwork,
        GeneralDevices,
        GraphicNovels,
        Juvenile,
        JuvenileCollection,
        Laptops3Day,
        Laptops3Hour,
        LewisFriendsCollection,
        Main,
        MainCollection,
        MaNaughtonCollection,
        McNaughtonCollection,
        MediaServicesReserves,
        MobileDevices,
        Periodicals,
        Reference,
        Reserves,
        Reserves1Hour,
        Storage,
        StorageEuler,
        StudyRoomKeys,
        VhsVideoCollection
    }
}