using ZondervanLibrary.SharedLibrary.Parsing.Fields;
using ZondervanLibrary.SharedLibrary.Parsing.Records;

namespace ZondervanLibrary.Harvester.Core.Operations.Demographics
{
    public enum DemographicGender
    {
        /// <summary>
        /// Male
        /// </summary>
        M,

        /// <summary>
        /// Female
        /// </summary>
        F 
    }

    public enum DemographicLevel
    {
        /// <summary>
        /// Undergraduate
        /// </summary>
        UG,

        /// <summary>
        /// Graduate?
        /// </summary>
        GR,
    }

    public enum DemographicStudentType
    {
        /// <summary>
        /// Undeclared
        /// </summary>
        _0,

        /// <summary>
        /// CCL - Correspondence
        /// </summary>
        _1,

        /// <summary>
        /// CCL - Online
        /// </summary>
        _2,

        /// <summary>
        /// CCL - AA
        /// </summary>
        _3,

        /// <summary>
        /// CLL (Cohort)
        /// </summary>
        _4,

        /// <summary>
        /// CLL - Mentored Student
        /// </summary>
        _5,

        /// <summary>
        /// Grad Transfer Student
        /// </summary>
        _6,
        
        /// <summary>
        /// CLL 2+2, First Time Freshman
        /// </summary>
        _7,

        /// <summary>
        /// CLL 2+2, Continuing Student
        /// </summary>
        _8,

        /// <summary>
        /// Graduate Guest
        /// </summary>
        A,

        /// <summary>
        /// Post Baccalaureate
        /// </summary>
        B,

        /// <summary>
        /// Continuing Student
        /// </summary>
        C,

        /// <summary>
        /// Re-admitted Student
        /// </summary>
        E,

        /// <summary>
        /// First Time Freshman
        /// </summary>
        F,

        /// <summary>
        /// Guest Student
        /// </summary>
        G,
        
        /// <summary>
        /// Graduate Guest, Financial Aid Eligible
        /// </summary>
        H,

        /// <summary>
        /// English Second Language
        /// </summary>
        I,

        /// <summary>
        /// International Guest
        /// </summary>
        J,

        /// <summary>
        /// Certificate Seeking Guest
        /// </summary>
        K,

        /// <summary>
        /// ESL International Guest
        /// </summary>
        L,

        /// <summary>
        /// First Time Graduate
        /// </summary>
        M,

        /// <summary>
        /// New Student Cancellation
        /// </summary>
        N,

        /// <summary>
        /// Continuing Graduate
        /// </summary>
        O,

        /// <summary>
        /// Pre-college Guest Student
        /// </summary>
        P,

        /// <summary>
        /// Re-Admitted Graduate
        /// </summary>
        Q,

        /// <summary>
        /// Returning Freshman
        /// </summary>
        R,

        /// <summary>
        /// Stopout
        /// </summary>
        S,

        /// <summary>
        /// Transfer
        /// </summary>
        T,

        /// <summary>
        /// Consortium Visitor
        /// </summary>
        V,

        /// <summary>
        /// Withdrawn
        /// </summary>
        W,

        /// <summary>
        /// Continuing Student - Forward Transition
        /// </summary>
        Z
    }

    /// <remarks>
    ///     <para>The best place I can find to map these codes to name is through the My TU Degree interface(Micah)</para>
    ///     <para>The myTaylor Roster tab has a hidden list of majors that can be shown if the page source is modified.(Tim)</para>
    /// </remarks>
    public enum DemographicMajor
    {
        /// <summary>
        /// Major Not Declared
        /// </summary>
        _0000,

        /// <summary>
        /// Associate Accounting
        /// </summary>
        AAC,

        /// <summary>
        /// Associate Business Admin
        /// </summary>
        ABA,

        /// <summary>
        /// Accounting
        /// </summary>
        ACC,

        /// <summary>
        /// Accounting Systems
        /// </summary>
        ACCS,

        /// <summary>
        /// Accounting and Info Applications
        /// </summary>
        AIA,

        /// <summary>
        /// Associate Music
        /// </summary>
        AMU,

        /// <summary>
        /// Art
        /// </summary>
        ART,

        /// <summary>
        /// Art Education
        /// </summary>
        ARTE,

        /// <summary>
        /// Art Systems
        /// </summary>
        ARTS,

        /// <summary>
        /// Athletic Training
        /// </summary>
        ATR,

        /// <summary>
        /// Bachelor Business Administration
        /// </summary>
        BBA,

        /// <summary>
        /// Biblical Studies
        /// </summary>
        BBS,

        /// <summary>
        /// Biochemistry
        /// </summary>
        BCH,

        /// <summary>
        /// Business and Info Applications
        /// </summary>
        BIA,

        /// <summary>
        /// Biblical Literature
        /// </summary>
        BIB,

        /// <summary>
        /// Biblical Literature Systems
        /// </summary>
        BIBS,

        /// <summary>
        /// Biology
        /// </summary>
        BIO,

        /// <summary>
        /// Biology Science Education
        /// </summary>
        BIOE,

        /// <summary>
        /// Biology PreMed
        /// </summary>
        BIOM,

        /// <summary>
        /// Biology Systems
        /// </summary>
        BIOS,

        /// <summary>
        /// Business Admin
        /// </summary>
        BUA,

        /// <summary>
        /// Business Admin Systems
        /// </summary>
        BUAS,

        /// <summary>
        /// Commication Arts Education
        /// </summary>
        CAE,

        /// <summary>
        /// Mass Communication
        /// </summary>
        CAM,

        /// <summary>
        /// Mass Communication Systems
        /// </summary>
        CAMS,

        /// <summary>
        /// Communications Studies
        /// </summary>
        CAS,

        /// <summary>
        /// Communications Studies Environmental Science
        /// </summary>
        CASN,

        /// <summary>
        /// Communications Studies Systems
        /// </summary>
        CASS,

        /// <summary>
        /// Theatre Arts
        /// </summary>
        CAT,

        /// <summary>
        /// Theatre Arts Systems
        /// </summary>
        CATS,

        /// <summary>
        /// Computer Science Digital Media
        /// </summary>
        CDM,

        /// <summary>
        /// Computer Science Digital Media Systems
        /// </summary>
        CDMS,

        /// <summary>
        /// Christian Education
        /// </summary>
        CED,

        /// <summary>
        /// Christian Education Systems
        /// </summary>
        CEDS,

        /// <summary>
        /// Christian Education Ministries
        /// </summary>
        CEM,

        /// <summary>
        /// Christian Education Ministries Systems
        /// </summary>
        CEMS,

        /// <summary>
        /// Computer Engineering
        /// </summary>
        CEN,

        /// <summary>
        /// Computer Engineering Systems
        /// </summary>
        CENS,

        /// <summary>
        /// Chemistry Environmental Science
        /// </summary>
        CES,

        /// <summary>
        /// Chemistry Environmental Science Systems
        /// </summary>
        CESS,

        /// <summary>
        /// Computer Graphic Arts
        /// </summary>
        CGA,

        /// <summary>
        /// Computer Graphic Arts Systems
        /// </summary>
        CGAS,

        /// <summary>
        /// Chemistry
        /// </summary>
        CHE,

        /// <summary>
        /// Chemistry Science Education
        /// </summary>
        CHEE,

        /// <summary>
        /// Chemistry Systems
        /// </summary>
        CHES,

        /// <summary>
        /// Computing and Information Applications
        /// </summary>
        CIA,

        /// <summary>
        /// Computer Information Systems
        /// </summary>
        CIS,

        /// <summary>
        /// Computer Information Systems Systems
        /// </summary>
        CISS,

        /// <summary>
        /// Christian Ministries Cross Cultural
        /// </summary>
        CMCC,

        /// <summary>
        /// Christian Ministries Christian Education
        /// </summary>
        CMCE,

        /// <summary>
        /// Christian Ministries
        /// </summary>
        CMI,

        /// <summary>
        /// Mass Communication Journalism
        /// </summary>
        CMJ,

        /// <summary>
        /// Mass Communication Journalism Systems
        /// </summary>
        CMJS,

        /// <summary>
        /// Christian Ministries Music
        /// </summary>
        CMMM,

        /// <summary>
        /// Christian Ministries Pastoral
        /// </summary>
        CMPM,

        /// <summary>
        /// Communication New Media
        /// </summary>
        CNM,

        /// <summary>
        /// Communication New Media Systems
        /// </summary>
        CNMS,

        /// <summary>
        /// Computer Science
        /// </summary>
        COS,

        /// <summary>
        /// Computer Science Systems
        /// </summary>
        COSS,

        /// <summary>
        /// Criminal Justice
        /// </summary>
        CRJ,

        /// <summary>
        /// Criminal Justice Systems
        /// </summary>
        CRJS,

        /// <summary>
        /// Computer Science
        /// </summary>
        CSC,

        /// <summary>
        /// Counseling
        /// </summary>
        CSG,

        /// <summary>
        /// Computer Science New Media
        /// </summary>
        CSM,

        /// <summary>
        /// Computer Science New Media Systems
        /// </summary>
        CSMS,

        /// <summary>
        /// Developmental Economics
        /// </summary>
        DEC,

        /// <summary>
        /// Developmental Economics Systems
        /// </summary>
        DECS,

        /// <summary>
        /// Early Childhood Education
        /// </summary>
        ECE,

        /// <summary>
        /// Economics
        /// </summary>
        ECO,

        /// <summary>
        /// Economics Systems
        /// </summary>
        ECOS,

        /// <summary>
        /// Educational Studies
        /// </summary>
        EDS,

        /// <summary>
        /// Elementary Education
        /// </summary>
        EED,

        /// <summary>
        /// Environmental Engineering
        /// </summary>
        EEN,

        /// <summary>
        /// Environmental Geology
        /// </summary>
        EGE,

        /// <summary>
        /// Environmental Biology
        /// </summary>
        ENB,

        /// <summary>
        /// Environmental Biology Systems
        /// </summary>
        ENBS,

        /// <summary>
        /// English
        /// </summary>
        ENG,

        /// <summary>
        /// English Education
        /// </summary>
        ENGE,

        /// <summary>
        /// English Literature
        /// </summary>
        ENGL,

        /// <summary>
        /// English Systems
        /// </summary>
        ENGS,

        /// <summary>
        /// English Writing
        /// </summary>
        ENGW,

        /// <summary>
        /// Environmental Management
        /// </summary>
        ENM,

        /// <summary>
        /// Environmental Management Systems
        /// </summary>
        ENMS,

        /// <summary>
        /// Engineering
        /// </summary>
        ENR,

        /// <summary>
        /// Environmental Science
        /// </summary>
        ENS,

        /// <summary>
        /// Engineering Physics
        /// </summary>
        EPH,

        /// <summary>
        /// Engineering Physics Systems
        /// </summary>
        EPHS,

        /// <summary>
        /// English Studies
        /// </summary>
        EST,

        /// <summary>
        /// Exercise Science
        /// </summary>
        EXS,

        /// <summary>
        /// Finance
        /// </summary>
        FIN,

        /// <summary>
        /// Film and Media Production
        /// </summary>
        FMP,

        /// <summary>
        /// French
        /// </summary>
        FRE,

        /// <summary>
        /// French Education
        /// </summary>
        FREE,

        /// <summary>
        /// Graphic Art
        /// </summary>
        GAT,

        /// <summary>
        /// Graphic Art Systems
        /// </summary>
        GATS,

        /// <summary>
        /// Graduate Business Administration
        /// </summary>
        GBUA,

        /// <summary>
        /// Graduate Environmental Science
        /// </summary>
        GENS,

        /// <summary>
        /// Geography
        /// </summary>
        GEO,

        /// <summary>
        /// Graduate Higher Education and Student Development
        /// </summary>
        GHES,

        /// <summary>
        /// Graduate Religious Studies
        /// </summary>
        GRST,

        /// <summary>
        /// Healthcare Management
        /// </summary>
        HCM,

        /// <summary>
        /// Higher Education And Student Development
        /// </summary>
        HES,

        /// <summary>
        /// History
        /// </summary>
        HIS,

        /// <summary>
        /// History Environmental Science
        /// </summary>
        HISN,

        /// <summary>
        /// History Systems
        /// </summary>
        HISS,

        /// <summary>
        /// Human Services
        /// </summary>
        HMS,

        /// <summary>
        /// Health And Physical Education
        /// </summary>
        HPEE,

        /// <summary>
        /// Physical Education
        /// </summary>
        HPR,

        /// <summary>
        /// Physical Education
        /// </summary>
        HPRE,

        /// <summary>
        /// Physical Education Systems
        /// </summary>
        HPRS,

        /// <summary>
        /// Intercultural Studies
        /// </summary>
        ICS,

        /// <summary>
        /// Interdisciplinary Systems
        /// </summary>
        IDS,

        /// <summary>
        /// Individual Goal Oriented
        /// </summary>
        ING,

        /// <summary>
        /// Individual Goal Oriented Systems
        /// </summary>
        INGS,

        /// <summary>
        /// International Business
        /// </summary>
        ITB,

        /// <summary>
        /// International Business Systems
        /// </summary>
        ITBS,

        /// <summary>
        /// International Studies
        /// </summary>
        ITS,

        /// <summary>
        /// International Studies Systems Was not listed in most recent file
        /// </summary>
        ITSS,

        /// <summary>
        /// Justice Administration
        /// </summary>
        JAD,

        /// <summary>
        /// Justice and Ministry
        /// </summary>
        JMI,

        /// <summary>
        /// Journalism Media Writing
        /// </summary>
        JMW,

        /// <summary>
        /// Journalism
        /// </summary>
        JRN,

        /// <summary>
        /// Liberal Arts
        /// </summary>
        LBA,

        /// <summary>
        /// Law and Justice
        /// </summary>
        LJU,

        /// <summary>
        /// Mathematics Education
        /// </summary>
        MAED,

        /// <summary>
        /// Mathematics Interdisciplinary
        /// </summary>
        MAI,

        /// <summary>
        /// Mathematics
        /// </summary>
        MAT,

        /// <summary>
        /// Mathematics Science Education
        /// </summary>
        MATE,

        /// <summary>
        /// Mathematics Systems
        /// </summary>
        MATS,

        /// <summary>
        /// Media Communication
        /// </summary>
        MCM,

        /// <summary>
        /// Media Communication Systems
        /// </summary>
        MCMS,

        /// <summary>
        /// Mathematics Environmental Science
        /// </summary>
        MES,

        /// <summary>
        /// Mathematics Environmental Science Systems
        /// </summary>
        MESS,

        /// <summary>
        /// Business Management
        /// </summary>
        MGT,

        /// <summary>
        /// Business Management Systems
        /// </summary>
        MGTS,

        /// <summary>
        /// Management Information Systems
        /// </summary>
        MIS,

        /// <summary>
        /// Marketing Communication
        /// </summary>
        MKC,

        /// <summary>
        /// Marketing
        /// </summary>
        MKT,

        /// <summary>
        /// Marketing Systems
        /// </summary>
        MKTS,

        /// <summary>
        /// Film and Media Production
        /// </summary>
        MPF,

        /// <summary>
        /// Film and Media Production Systems
        /// </summary>
        MPFS,

        /// <summary>
        /// Media Production
        /// </summary>
        MPR,

        /// <summary>
        /// Media Production Systems
        /// </summary>
        MPRS,

        /// <summary>
        /// Musical Threatre
        /// </summary>
        MTR,

        /// <summary>
        /// Music Theory and Composition
        /// </summary>
        MUCP,

        /// <summary>
        /// Music Vocal Performance
        /// </summary>
        MUPF,

        /// <summary>
        /// Music
        /// </summary>
        MUS,

        /// <summary>
        /// Music Systems
        /// </summary>
        MUSS,

        /// <summary>
        /// Music Education
        /// </summary>
        MUSE,

        /// <summary>
        /// Media Writing
        /// </summary>
        MWR,

        /// <summary>
        /// Media Writing Systems
        /// </summary>
        MWRS,

        /// <summary>
        /// Natural Science
        /// </summary>
        NAS,

        /// <summary>
        /// Natural Science Systems
        /// </summary>
        NASS,

        /// <summary>
        /// Organizational Management
        /// </summary>
        OMG,

        /// <summary>
        /// Pre-Art Therapy
        /// </summary>
        PAT,

        /// <summary>
        /// Public Health
        /// </summary>
        PBH,

        /// <summary>
        /// Public Relations
        /// </summary>
        PBR,

        /// <summary>
        /// Public Relations Systems
        /// </summary>
        PBRS,

        /// <summary>
        /// Prepatory ESL
        /// </summary>
        PESL,

        /// <summary>
        /// Physics Engineering Physics
        /// </summary>
        PHEP,

        /// <summary>
        /// Philosophy
        /// </summary>
        PHI,

        /// <summary>
        /// Philosophy Systems
        /// </summary>
        PHIS,

        /// <summary>
        /// Physics Mathematics Education
        /// </summary>
        PHME,

        /// <summary>
        /// Physics
        /// </summary>
        PHY,

        /// <summary>
        /// Physics Science Education
        /// </summary>
        PHYE,

        /// <summary>
        /// Physics Environmental Science
        /// </summary>
        PHYN,

        /// <summary>
        /// Physics Systems
        /// </summary>
        PHYS,

        /// <summary>
        /// Pastoral Ministries
        /// </summary>
        PMI,

        /// <summary>
        /// Pastoral Ministries
        /// </summary>
        PMIN,

        /// <summary>
        /// Political Science
        /// </summary>
        POS,

        /// <summary>
        /// Political Science Systems
        /// </summary>
        POSS,

        /// <summary>
        /// Political Science Philosophy and Economics
        /// </summary>
        PPE,

        /// <summary>
        /// Political Science Philosophy and Economics Systems
        /// </summary>
        PPES,

        /// <summary>
        /// Pre Major
        /// </summary>
        PRE,

        /// <summary>
        /// Pre Major Preparatory
        /// </summary>
        PREP,

        /// <summary>
        /// Physical Science Education
        /// </summary>
        PSCE,

        /// <summary>
        /// Psychology
        /// </summary>
        PSY,

        /// <summary>
        /// Psychology Environmental Science
        /// </summary>
        PSYN,

        /// <summary>
        /// Psychology Systems
        /// </summary>
        PSYS,

        /// <summary>
        /// Professional Writing
        /// </summary>
        PWR,

        /// <summary>
        /// Professional Writing Systems
        /// </summary>
        PWRS,

        /// <summary>
        /// Religious Studies
        /// </summary>
        RES,

        /// <summary>
        /// Recreational Leadership
        /// </summary>
        RLE,

        /// <summary>
        /// Studio Art
        /// </summary>
        SAR,

        /// <summary>
        /// Systems Engineering
        /// </summary>
        SEN,

        /// <summary>
        /// Sport Management
        /// </summary>
        SMA,

        /// <summary>
        /// Sport Management Systems
        /// </summary>
        SMAS,

        /// <summary>
        /// Sociology
        /// </summary>
        SOC,

        /// <summary>
        /// Sociology Systems
        /// </summary>
        SOCS,

        /// <summary>
        /// Social Studies
        /// </summary>
        SOS,

        /// <summary>
        /// Social Studies Education
        /// </summary>
        SOSE,

        /// <summary>
        /// Spanish
        /// </summary>
        SPA,

        /// <summary>
        /// Spanish Education
        /// </summary>
        SPAE,

        /// <summary>
        /// Spanish Systems
        /// </summary>
        SPAS,

        /// <summary>
        /// Social Studies Geography
        /// </summary>
        SSGE,

        /// <summary>
        /// Social Studies Government
        /// </summary>
        SSGO,

        /// <summary>
        /// Social Studies Psychology
        /// </summary>
        SSPS,

        /// <summary>
        /// Social Studies Sociology
        /// </summary>
        SSSO,

        /// <summary>
        /// Social Studies US History
        /// </summary>
        SSUS,

        /// <summary>
        /// Social Studies Education World Civilations
        /// </summary>
        SSWC,

        /// <summary>
        /// Sustainable Development
        /// </summary>
        SUS,

        /// <summary>
        /// Social Work
        /// </summary>
        SWK,

        /// <summary>
        /// Social Work Systems
        /// </summary>
        SWKS,

        /// <summary>
        /// Theatre Arts
        /// </summary>
        THR,

        /// <summary>
        /// Technology Management
        /// </summary>
        TMT,

        /// <summary>
        /// Special Education Lic? Program
        /// </summary>
        TSED,

        /// <summary>
        /// Transition to Teaching Elementary
        /// </summary>
        TTE,

        /// <summary>
        /// Transition to Teaching Secondary
        /// </summary>
        TTS,

        /// <summary>
        /// Undeclared Business
        /// </summary>
        UBUS,

        /// <summary>
        /// Undeclared Secondary Education
        /// </summary>
        UEDU,

        /// <summary>
        /// Undeclared Media Communication 
        /// </summary>
        UMCM,

        /// <summary>
        /// Urban Ministries
        /// </summary>
        UMI,

        /// <summary>
        /// Urban Ministries
        /// </summary>
        UMIN,

        /// <summary>
        /// Visual Arts New Media
        /// </summary>
        VAM,

        /// <summary>
        /// Visual Arts New Media Systems
        /// </summary>
        VAMS,

        /// <summary>
        /// Graphic Design
        /// </summary>
        VAR,

        /// <summary>
        /// Graphic Design Systems
        /// </summary>
        VARS,

        /// <summary>
        /// Vocational Ministry
        /// </summary>
        VMI,

        /// <summary>
        /// Web Communication
        /// </summary>
        WBC,

        /// <summary>
        /// Web Communication Systems
        /// </summary>
        WBCS,

        /// <summary>
        /// Wellness
        /// </summary>
        WEL,

        /// <summary>
        /// Bible
        /// </summary>
        YBIB,

        /// <summary>
        /// Youth Ministries
        /// </summary>
        YMI,

        /// <summary>
        /// Youth Ministries Systems
        /// </summary>
        YMIS,
    }

    public enum DemographicClass
    {
        /// <summary>
        /// Freshman
        /// </summary>
        FR,

        /// <summary>
        /// Graduate
        /// </summary>
        GR,

        /// <summary>
        /// Junior
        /// </summary>
        JR,

        /// <summary>
        /// Sophomore
        /// </summary>
        SO,

        /// <summary>
        /// Senior
        /// </summary>
        SR,

        /// <summary>
        /// Unclassified
        /// </summary>
        UN
    }

    public enum DemographicResidence
    {
        /// <summary>
        /// Anderson Apartment
        /// </summary>
        AA,

        /// <summary>
        /// Away From Campus Upland
        /// </summary>
        AFC,

        /// <summary>
        /// Allen House
        /// </summary>
        AH,

        /// <summary>
        /// Brandle Apartment/OC
        /// </summary>
        BA,

        /// <summary>
        /// Bacon House/OC
        /// </summary>
        BC,

        /// <summary>
        /// Brandle House/OC
        /// </summary>
        BD,

        /// <summary>
        /// Bethany Hall
        /// </summary>
        BE,

        /// <summary>
        /// Bell House/OC
        /// </summary>
        BEL,

        /// <summary>
        /// Bergwall Hall
        /// </summary>
        BH,

        /// <summary>
        /// Balkema House/OC
        /// </summary>
        BK,

        /// <summary>
        /// Benjamin House/OC
        /// </summary>
        BN,

        /// <summary>
        /// Briarwood Apartments/OC
        /// </summary>
        BR,

        /// <summary>
        /// Beers Apartment/OC
        /// </summary>
        BS,

        /// <summary>
        /// Brown House/OC
        /// </summary>
        BW,

        /// <summary>
        /// Carriage House/OC
        /// </summary>
        CA,

        /// <summary>
        /// Callison House/OC
        /// </summary>
        CAL,

        /// <summary>
        /// Codding House/OC
        /// </summary>
        CD,

        /// <summary>
        /// Crane House/OC
        /// </summary>
        CH,

        /// <summary>
        /// Chance House
        /// </summary>
        CHA,

        /// <summary>
        /// The Church/OC
        /// </summary>
        CHU,

        /// <summary>
        /// Craig Luthy Houses/OC
        /// </summary>
        CL,

        /// <summary>
        /// Commuter Student
        /// </summary>
        CM,

        /// <summary>
        /// Commuter Married
        /// </summary>
        CMM,

        /// <summary>
        /// Commuter Single
        /// </summary>
        CMS,

        /// <summary>
        /// Casa Patricia Apts/OC
        /// </summary>
        CP,

        /// <summary>
        /// Cramer House
        /// </summary>
        CRH,

        /// <summary>
        /// Clyde Taylor House
        /// </summary>
        CT,

        /// <summary>
        /// Davenport House/OC
        /// </summary>
        DA,

        /// <summary>
        /// Davenport/Tittle House/OC
        /// </summary>
        DDT,

        /// <summary>
        /// Deck House
        /// </summary>
        DH,

        /// <summary>
        /// Diller House/OC
        /// </summary>
        DI,

        /// <summary>
        /// Delong House
        /// </summary>
        DL,

        /// <summary>
        /// Dooley House/OC
        /// </summary>
        DO,

        /// <summary>
        /// Delta Apts/OC
        /// </summary>
        DT,

        /// <summary>
        /// Eastus House
        /// </summary>
        EAH,

        /// <summary>
        /// English Hall
        /// </summary>
        EH,

        /// <summary>
        /// Eric Hayes Apt/OC
        /// </summary>
        EK,

        /// <summary>
        /// Fairlane Apartments/OC
        /// </summary>
        FA,

        /// <summary>
        /// Away From Campus FW
        /// </summary>
        FAFC,

        /// <summary>
        /// Founder's Hall
        /// </summary>
        FD,

        /// <summary>
        /// Fode House/OC
        /// </summary>
        FO,

        /// <summary>
        /// Oakwood Apartments
        /// </summary>
        FOC,

        /// <summary>
        /// Temporary Housing - Fort Wayne
        /// </summary>
        FTMP,

        /// <summary>
        /// Fyffe House
        /// </summary>
        FYH,

        /// <summary>
        /// Gas City Apartments
        /// </summary>
        GCA,

        /// <summary>
        /// Gerig Hall
        /// </summary>
        GH,

        /// <summary>
        /// Gifford House/OC
        /// </summary>
        GI,

        /// <summary>
        /// Glass House/OC
        /// </summary>
        GL,

        /// <summary>
        /// Gillenwater House
        /// </summary>
        GLH,

        /// <summary>
        /// Gortner House/OC
        /// </summary>
        GR,

        /// <summary>
        /// Gross House/OC
        /// </summary>
        GRS,

        /// <summary>
        /// Graduate Student Housing
        /// </summary>
        GSH,

        /// <summary>
        /// Hill Apartment
        /// </summary>
        HA,

        /// <summary>
        /// Haire House
        /// </summary>
        HAI,

        /// <summary>
        /// Hayes Apartment
        /// </summary>
        HAY,

        /// <summary>
        /// Hausser Hall
        /// </summary>
        HH,

        /// <summary>
        /// Holloway House/OC
        /// </summary>
        HL,

        /// <summary>
        /// Horner House/OC
        /// </summary>
        HN,

        /// <summary>
        /// Harmon House
        /// </summary>
        HR,

        /// <summary>
        /// Hunter House/OC
        /// </summary>
        HU,

        /// <summary>
        /// International House/OC
        /// </summary>
        IH,

        /// <summary>
        /// Jefferson St Cottages
        /// </summary>
        JC,

        /// <summary>
        /// Jordan Carriage Apartment
        /// </summary>
        JCA,

        /// <summary>
        /// Jordan Mobile/OC
        /// </summary>
        JM,

        /// <summary>
        /// Korfmacher House/OC
        /// </summary>
        KRF,

        /// <summary>
        /// Leightner Hall
        /// </summary>
        LE,

        /// <summary>
        /// Luthy (Fred) Houses
        /// </summary>
        LH,

        /// <summary>
        /// Leffingwell (John) House/OC
        /// </summary>
        LJ,

        /// <summary>
        /// Lillian Rolf House
        /// </summary>
        LL,

        /// <summary>
        /// Lawvere House
        /// </summary>
        LW,

        /// <summary>
        /// Muncie Apartments
        /// </summary>
        MA,

        /// <summary>
        /// Miller Apartment/OC
        /// </summary>
        MI,

        /// <summary>
        /// Moore (Chuck) House/OC
        /// </summary>
        MO,

        /// <summary>
        /// Marion Apartments
        /// </summary>
        MRA,

        /// <summary>
        /// Muthiah House/OC
        /// </summary>
        MT,

        /// <summary>
        /// Nitsche House
        /// </summary>
        NTZ,

        /// <summary>
        /// Off-campus Housing
        /// </summary>
        OC,

        /// <summary>
        /// Olson Hall
        /// </summary>
        OH,

        /// <summary>
        /// Parkville Apartments/OC
        /// </summary>
        PA,

        /// <summary>
        /// Pat Moore House/OC
        /// </summary>
        PM,

        /// <summary>
        /// Raikes House/OC
        /// </summary>
        RAI,

        /// <summary>
        /// Breuninger Hall
        /// </summary>
        RBH,

        /// <summary>
        /// Reese House/OC
        /// </summary>
        RE,

        /// <summary>
        /// Rivera House/OC
        /// </summary>
        RI,

        /// <summary>
        /// Rice House
        /// </summary>
        RIC,

        /// <summary>
        /// Ramseyer Hall
        /// </summary>
        RM,

        /// <summary>
        /// Rohrs House
        /// </summary>
        RR,

        /// <summary>
        /// Ross Apartments
        /// </summary>
        RS,

        /// <summary>
        /// Schultz House
        /// </summary>
        SC,

        /// <summary>
        /// Songer House/OC
        /// </summary>
        SG,

        /// <summary>
        /// Sammy Morris Hall
        /// </summary>
        SM,

        /// <summary>
        /// Speicher House
        /// </summary>
        SPC,

        /// <summary>
        /// Spencer House/OC
        /// </summary>
        SPN,

        /// <summary>
        /// Swallow Robin Hall
        /// </summary>
        SR,

        /// <summary>
        /// Stevens House
        /// </summary>
        ST,

        /// <summary>
        /// Schweichart House/OC
        /// </summary>
        SW,

        /// <summary>
        /// Bowers House/OC
        /// </summary>
        TBH,

        /// <summary>
        /// Tedder (Jack) House/OC
        /// </summary>
        TD,

        /// <summary>
        /// True House/OC
        /// </summary>
        TH,

        /// <summary>
        /// Taylot Street House/OC
        /// </summary>
        TS,

        /// <summary>
        /// Cambell Hall
        /// </summary>
        U1A,

        /// <summary>
        /// Wolgemuth Hall
        /// </summary>
        U2A,

        /// <summary>
        /// Haakonsen Hall
        /// </summary>
        UHH,

        /// <summary>
        /// TemporaryHousing
        /// </summary>
        UTMP,

        /// <summary>
        /// Voss Apartment
        /// </summary>
        VA,

        /// <summary>
        /// Wiebke House
        /// </summary>
        WB,

        /// <summary>
        /// Wengatz Hall
        /// </summary>
        WH,

        /// <summary>
        /// Wren House/OC
        /// </summary>
        WR,

        /// <summary>
        /// Williams Unit A/OC
        /// </summary>
        WUA,

        /// <summary>
        /// Williams Unit C/OC
        /// </summary>
        WUC,

        /// <summary>
        /// Williams Unit D/OC
        /// </summary>
        WUD,

        /// <summary>
        /// Wandering Wheels Apt/OC
        /// </summary>
        WW,

        /// <summary>
        /// Young House/OC
        /// </summary>
        YH,
    }

    [DelimitedRecord("\t", IgnoreFirstLine = true)]
    public class DemographicRecord
    {
        [DelimitedField(IsRequired = true, ValidationPattern = "^[0-9]{14}$")]
        public string Barcode { get; set; }

        [DelimitedField(IsRequired = true)]
        public DemographicGender Gender { get; set; }

        [DelimitedField(IsRequired = true)]
        public DemographicLevel Level { get; set; }

        [DelimitedField(IsRequired = true)]
        public DemographicStudentType StudentType { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^0$")]
        public DemographicMajor? Major1 { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^$")]
        public DemographicMajor? Major2 { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^.00$")]
        public decimal? Gpa { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^UN$")]
        public DemographicClass? Class { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^$")]
        public DemographicResidence? Residence { get; set; }
    }
}