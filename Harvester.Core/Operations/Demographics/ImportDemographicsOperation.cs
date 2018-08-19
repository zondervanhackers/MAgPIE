using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.SharedLibrary.Binding;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.SharedLibrary.Parsing;
using ZondervanLibrary.SharedLibrary.Pipeline;
using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Core.Operations.Demographics
{
    public class ImportDemographicsOperation : OperationBase
    {
        private readonly RepositoryArgumentsBase _harvesterArgs;
        private readonly RepositoryArgumentsBase _directoryArgs;
        private readonly RepositoryArgumentsBase _statisticsArgs;

        public ImportDemographicsOperation(RepositoryArgumentsBase HarvesterDatabase, RepositoryArgumentsBase DestinationDatabase, RepositoryArgumentsBase SourceDirectory)
        {
            Contract.Requires(SourceDirectory != null);
            Contract.Requires(HarvesterDatabase != null);
            Contract.Requires(DestinationDatabase != null);

            _directoryArgs = SourceDirectory;
            _harvesterArgs = HarvesterDatabase;
            _statisticsArgs = DestinationDatabase;
        }

        public enum Gender
        {
            Male,
            Female
        }

        public enum StudentLevel
        {
            Undergraduate,
            Graduate
        }

        public enum StudentType
        {
            Undeclared,
            CclCorrespondence,
            CclOnline,
            CclAa,
            CclCohort,
            CclMentoredLearning,
            GradTransferStudent,
            CclFirstTimeFreshman,
            CclContinuingStudent,
            GraduateGuest,
            PostBaccalaureate,
            ContinuingStudent,
            ReadmittedStudent,
            FirstTimeFreshman,
            GuestStudent,
            GraduateGuestFinancialAidEligible,
            EnglishSecondLanguage,
            InternationalGuest,
            CertificateSeekingStudent,
            EslInternationalGuest,
            FirstTimeGraduate,
            NewStudentCancellation,
            ContinuingGraduate,
            PrecollegeStudent,
            Re_AdmittedStudent,
            ReturningFreshman,
            Stopout,
            Transfer,
            ConsortiumVisitor,
            Withdrawn,
            ContinuingStudentForwardTransition
        }

        public enum StudentClass
        {
            Freshman,
            Sophomore,
            Junior,
            Senior,
            Graduate,
            Unclassified
        }

        public enum ResidenceName
        {
            /// <summary>
            /// Likely studying abroad.
            /// </summary>
            AllenHouse,
            AndersonApartment,
            AwayFromCampusFW,
            AwayFromCampusUpland,
            BaconHouse,
            BalkemaHouse,
            BeersApartment,
            BellHouse,
            BenjaminHouse,
            BergwallHall,
            BethanyHouse,
            BowersHouse,
            BrandleApartment,
            BrandleHouse,
            BreuningerHall,
            BriarwoodApartments,
            BrownHouse,
            CallisonHouse,
            CampbellHall,
            CarriageHouse,
            CasaPatriciaApartments,
            ChanceHouse,
            ClydeTaylorHouse,
            CoddingHouse,
            CommuterMarried,
            CommuterSingle,
            CommuterStudent,
            CraigLuthyHouses,
            CramerHouse,
            CraneHouse,
            DavenportHouse,
            DavenportTittleHouse,
            DeckHouse,
            DelongHouse,
            DeltaApartments,
            DillerHouse,
            DooleyHouse,
            EastusHouse,
            EnglishHall,
            EricHayesApartments,
            FairlaneApartments,
            FodeHouse,
            FoundersHall,
            FyffeHouse,
            GasCityApartments,
            GerigHall,
            GiffordHouse,
            GillenwaterHouse,
            GlassHouse,
            GortnerHouse,
            GraduateStudentHousing,
            GrossHouse,
            HaakonsenHall,
            HaireHouse,
            HarmonHouse,
            HausserHall,
            HayesApartment,
            HillApartment,
            HollowayHouse,
            HornerApartment,
            HunterHouse,
            InternationalHouse,
            JeffersonStCottages,
            JordanCarriageApartment,
            JordanMobile,
            KorfmacherHouse,
            LawvereHouse,
            LeffingwellJohnHouse,
            LeightnerHall,
            LillianRolfHouse,
            LuthyFredHouses,
            MarionApartments,
            MillerApartments,
            MooreChuckHouse,
            MuncieApartments,
            MuthiahHouse,
            NitzscheHouse,
            OakwoodApartments,
            OffCampusHousing,
            OlsonHall,
            ParkvilleApartments,
            PatMooreHouse,
            RaikesHouse,
            RamseyerHall,
            ReeseHouse,
            RiceHouse,
            RiveraHouse,
            RohrsHouse,
            RossApartments,
            SammyMorrisHall,
            SchultzHall,
            SchweichartHouse,
            SongerHouse,
            SpeicherHouse,
            SpencerHouse,
            StevensHouse,
            SwallowRobinHall,
            TaylorStreetHouse,
            TedderJackHouse,
            TemporaryHousingFortWayne,
            TemporaryHousingUpland,
            TheChurch,
            TrueHouse,
            VossApartment,
            WanderingWheelsApartments,
            WengatzHall,
            WiebkeHouse,
            WilliamsUnitA,
            WilliamsUnitC,
            WilliamsUnitD,
            WolgemuthHall,
            WrenHouse,
            YoungHouse,
            
        }

        public enum ResidenceCategory
        {
            Commuter,
            OnCampus,
            OffCampus,
            AwayFromCampus
        }

        public enum MajorName
        {
            Accounting,
            AccountingInfoApplications,
            AccountingSystems,
            Art,
            ArtEducation,
            ArtSystems,
            AssociateAccounting,
            AssociatebusinessAdmin,
            AssociateMusic,
            AthleticTraining,
            BachelorBusinessAdministration,
            Bible,
            BiblicalLiterature,
            BiblicalLiteratureSystems,
            BiblicalStudies,
            Biochemistry,
            Biology,
            BiologyScienceEducation,
            BiologyPremed,
            BiologySystems,
            BusinessInfoApplications,
            BusinessAdministration,
            BusinessAdministrationSystems,
            Chemistry,
            ChemistryScienceEducation,
            ChemistrySystems,
            ChemistryEnvironmentalScience,
            ChemistryEnvironmentalScienceSystems,
            ChristianEducation,
            ChristianEducationSystems,
            ChristianEducationMinistries,
            ChristianEducationMinistriesSystems,
            ChristianMinistries,
            ChristianMinistriesChristianEducation,
            ChristianMinistriesCrossCultural,
            ChristianMinistriesMusic,
            ChristianMinistriesPastoral,
            CommunicationArtsEducation,
            CommunicationStudies,
            CommunicationStudiesEnvSci,
            CommunicationStudiesSystems,
            CommunicationNewMediaSystems,
            ComputerEngineering,
            ComputerEngineeringSystems,
            ComputerGraphicArts,
            ComputerGraphicArtsSystems,
            ComputerInformationSystems,
            ComputerInformationSystemsSystems,
            ComputerScience,
            ComputerScienceSystems,
            ComputerScienceDigitalMedia,
            ComputerScienceDigitalMediaSystems,
            ComputerScienceNewMedia,
            ComputerScienceNewMediaSystems,
            ComputingAndInfoApplications,
            Counseling,
            CriminalJustice,
            CriminalJusticeSystems,
            DevelopmentalEconomics,
            DevelopmentalEconomicsSystems,
            EarlyChildhoodEducation,
            Economics,
            EconomicsSystems,
            EducationalStudies,
            ElementaryEducation,
            Engineering,
            EngineeringPhysics,
            EngineeringPhysicsSystems,
            English,
            EnglishEducation,
            EnglishLiterature,
            EnglishStudies,
            EnglishSystems,
            EnglishWriting,
            EnvironmentalBiology,
            EnvironmentalBiologySystems,
            EnvironmentalEngineering,
            EnvironmentalGeology,
            EnvironmentalManagement,
            EnvironmentalManagementSystems,
            EnvironmentalScience,
            ExerciseScience,
            FilmAndMediaProduction,
            FilmAndMediaProductionSystems,
            Finance,
            French,
            FrenchEducation,
            Geography,
            GraduateReligiousStudies,
            GraphicArt,
            GraphicArtSystems,
            HealthPhysicalEducation,
            HealthcareManagement,
            HigherEducationStudentDevelopment,
            History,
            HistoryEnvironmentalScience,
            HistorySystems,
            HumanServices,
            IndividualGoalOriented,
            IndividualGoalOrientedSystems,
            InterculturalStudies,
            InterdisciplinaryStudies,
            InternationalBusiness,
            InternationalBusinessSystems,
            InternationalStudies,
            InternationalStudiesSystems,
            Journalism,
            JournalismMediaWriting,
            JusticeAdministration,
            JusticeAndMinistry,
            LawAndJustice,
            LiberalArts,
            NotDeclared,
            Management,
            ManagementInformationSystems,
            ManagementSystems,
            Marketing,
            MarketingCommunication,
            MarketingSystems,
            MassCommunication,
            MassCommunicationJournalism,
            MassCommunicationJournalismSystems,
            MassCommunicationSystems,
            Mathematics,
            MathematicsEducation,
            MathematicsScienceEducation,
            MathematicsSystems,
            MathematicsEnvironmentalScience,
            MathematicsEnivornmentalScienceSystems,
            MathematicsInterdisciplinary,
            MediaCommunication,
            MediaCommunicationSystems,
            MediaProduction,
            MediaProductionSystems,
            MediaWriting,
            MediaWritingSystems,
            Music,
            MusicEducation,
            MusicSystems,
            MusicalTheatre,
            NaturalScience,
            NaturalScienceSystems,
            OrganizationalManagement,
            PastoralMinistries,
            MusicVocalPerformance,
            Philosophy,
            PhilosophySystems,
            PhysicalEducation,
            PhysicalEducationSystems,
            PhysicalScienceEducation,
            Physics,
            PhysicsScienceEducation,
            PhysicsEngineeringPhysics,
            PhysicsEnvironmentalScience,
            PhysicsMathematicsEducation,
            PhysicsSystems,
            PoliticalSciencePhilosophyEconomics,
            PoliticalSciencePhilosophyEconomicsSystems,
            PoliticalScience,
            PoliticalScienceSystems,
            PreArtTherapy,
            PreMajor,
            PreMajorPreparatory,
            PrepatoryESL,
            ProfessionalWriting,
            ProfessionalWritingSystems,
            Psychology,
            PsychologyEnvironmentalScience,
            PsychologySystems,
            PublicHealth,
            PublicRelations,
            PublicRelationsSystems,
            RecreationalLeadership,
            ReligiousStudies,
            SocialStudies,
            SocialStudiesEducationGeography,
            SocialStudiesEducationGovernment,
            SocialStudiesEducationPsychology,
            SocialStudiesEducationSociology,
            SocialStudiesEducation,
            SocialStudiesEducationUSHistory,
            SocialStudiesEducationWorldCiv,
            SocialWork,
            SocialWorkSystems,
            Sociology,
            SociologySystems,
            Spanish,
            SpanishEducation,
            SpanishSystems,
            SpecialEducationLicProg,
            SportManagement,
            SportManagementSystems,
            StudioArt,
            SustainableDevelopment,
            SystemsEngineering,
            TechnologyManagement,
            TheatreArts,
            TheatreArtsSystems,
            TransitionTeachingElementary,
            TransitionTeachingSecondary,
            UndeclaredBusiness,
            UndeclaredMediaCommunication,
            UndeclaredSecondaryEducation,
            UrbanMinistries,
            VisualArts,
            VisualArtsSystems,
            VisualArtsNewMedia,
            VisualArtsNewMediaSystems,
            VocationalMinistry,
            WebCommunication,
            WebCommunicationSystems,
            Wellness,
            YouthMinistries,
            YouthMinistriesSystems,

            GraduateBusinessAdministration,
            GraduateEnvironmentalScience,
            GraduateHigherEducationStudentDevelopment,
            MusicTheoryComposition,

            //BusinessManagement,
            //BusinessManagementSystems,
            

        }

        public enum MajorBase
        {
            Accounting,
            Art,
            BiblicalLiterature,
            Biology,
            Business,
            Communications,
            Theatre,
            ComputerScience,
            Chemistry,
            Economics,
            Education,
            Engineering,
            English,
            EnvironmentalScience,
            Finance,
            French,
            Geography,
            Health,
            History,
            InternationalStudies,
            LiberalArts,
            Mathematics,
            Marketing,
            Music,
            Philosophy,
            Physics,
            Politics,
            Psychology,
            Writing,
            Sports,
            SocialStudies,
            Sociology,
            Spanish,
            SocialWork,
            GoalOriented,
            NotDeclared
        }

        public enum MajorType
        {
            Major1,
            Major2
        }

        /*
        private enum DegreeType
        {
            AssociateArts,
            BachelorArts,
            BachelorBusinessAdministration,
            BachelorMusic,
            BachelorScience,
            MasterArts,
            MasterBusinessAdministration,
            MasterEnvironmentalScience
        }
	    */

        private enum School
        {
            NaturalAppliedSciences,
            SocialSciencesEducationBusiness,
            HumanitiesArtsBiblicalStudies,
            GraduateStudies
        }

        public enum Department
        {
            Art,
            BiblicalStudiesChristianEducationPhilosophy,
            Biology,
            Business,
            ChemistryBiochemistry,
            Communication,
            ComputerScienceEngineering,
            EarthEnvironmentalSciences,
            Education,
            English,
            HistoryInternationalStudiesSocialStudies,
            Independent,
            Kinesiology,
            LiberalArts,
            MasterArtsHigherEducationStudentDevelopment,
            //MasterBusinessAdministration,
            MasterEnvironmentalScience,
            Mathematics,
            MediaCommunication,
            ModernLanguages,
            Music,
            PhysicsEngineering,
            PoliticalScienceInternationalRelations,
            ProfessionalWriting,
            Psychology,
            SocialWork,
            Sociology,
            Undeclared
        }

        public static Gender GetGender(DemographicGender gender)
        {
            switch (gender)
            {
                case DemographicGender.M:
                    return Gender.Male;
                case DemographicGender.F:
                    return Gender.Female;
            }

            throw new NotImplementedException(gender.ToString());
        }

        public static StudentLevel GetStudentLevel(DemographicLevel level)
        {
            switch (level)
            {
                case DemographicLevel.UG:
                    return StudentLevel.Undergraduate;
                case DemographicLevel.GR:
                    return StudentLevel.Graduate;
            }

            throw new NotImplementedException(level.ToString());
        }

        public static StudentType GetStudentType(DemographicStudentType studentType)
        {
            switch (studentType)
            {
                case DemographicStudentType._0:
                    return StudentType.Undeclared;
                case DemographicStudentType._1:
                    return StudentType.CclCorrespondence;
                case DemographicStudentType._2:
                    return StudentType.CclOnline;
                case DemographicStudentType._3:
                    return StudentType.CclAa;
                case DemographicStudentType._4:
                    return StudentType.CclCohort;
                case DemographicStudentType._5:
                    return StudentType.CclMentoredLearning;
                case DemographicStudentType._6:
                    return StudentType.GradTransferStudent;
                case DemographicStudentType._7:
                    return StudentType.CclFirstTimeFreshman;
                case DemographicStudentType._8:
                    return StudentType.CclContinuingStudent;
                case DemographicStudentType.A:
                    return StudentType.GraduateGuest;
                case DemographicStudentType.B:
                    return StudentType.PostBaccalaureate;
                case DemographicStudentType.C:
                    return StudentType.ContinuingStudent;
                case DemographicStudentType.E:
                    return StudentType.ReadmittedStudent;
                case DemographicStudentType.F:
                    return StudentType.FirstTimeFreshman;
                case DemographicStudentType.G:
                    return StudentType.GuestStudent;
                case DemographicStudentType.H:
                    return StudentType.GraduateGuestFinancialAidEligible;
                case DemographicStudentType.I:
                    return StudentType.EnglishSecondLanguage;
                case DemographicStudentType.J:
                    return StudentType.InternationalGuest;
                case DemographicStudentType.K:
                    return StudentType.CertificateSeekingStudent;
                case DemographicStudentType.L:
                    return StudentType.EslInternationalGuest;
                case DemographicStudentType.M:
                    return StudentType.FirstTimeGraduate;
                case DemographicStudentType.N:
                    return StudentType.NewStudentCancellation;
                case DemographicStudentType.O:
                    return StudentType.ContinuingGraduate;
                case DemographicStudentType.P:
                    return StudentType.PrecollegeStudent;
                case DemographicStudentType.Q:
                    return StudentType.Re_AdmittedStudent;
                case DemographicStudentType.R:
                    return StudentType.ReturningFreshman;
                case DemographicStudentType.S:
                    return StudentType.Stopout;
                case DemographicStudentType.T:
                    return StudentType.Transfer;
                case DemographicStudentType.V:
                    return StudentType.ConsortiumVisitor;
                case DemographicStudentType.W:
                    return StudentType.Withdrawn;
                case DemographicStudentType.Z:
                    return StudentType.ContinuingStudentForwardTransition;
            }

            throw new NotImplementedException(studentType.ToString());
        }

        public static StudentClass? GetStudentClass(DemographicClass? studentClass)
        {
            if (studentClass == null)
                return null;

            switch (studentClass.Value)
            {
                case DemographicClass.FR:
                    return StudentClass.Freshman;
                case DemographicClass.SO:
                    return StudentClass.Sophomore;
                case DemographicClass.JR:
                    return StudentClass.Junior;
                case DemographicClass.SR:
                    return StudentClass.Senior;
                case DemographicClass.GR:
                    return StudentClass.Graduate;
                case DemographicClass.UN:
                    return StudentClass.Unclassified;
            }

            throw new NotImplementedException();
        }

        public static ResidenceName GetResidenceName(DemographicResidence residence)
        {
            switch (residence)
            {
                case DemographicResidence.AA:
                    return ResidenceName.AndersonApartment;
                case DemographicResidence.AFC:
                    return ResidenceName.AwayFromCampusUpland;
                case DemographicResidence.AH:
                    return ResidenceName.AllenHouse;
                case DemographicResidence.BA:
                    return ResidenceName.BrandleApartment;
                case DemographicResidence.BC:
                    return ResidenceName.BaconHouse;
                case DemographicResidence.BD:
                    return ResidenceName.BrandleHouse;
                case DemographicResidence.BE:
                    return ResidenceName.BethanyHouse;
                case DemographicResidence.BEL:
                    return ResidenceName.BellHouse;
                case DemographicResidence.BH:
                    return ResidenceName.BergwallHall;
                case DemographicResidence.BK:
                    return ResidenceName.BalkemaHouse;
                case DemographicResidence.BN:
                    return ResidenceName.BenjaminHouse;
                case DemographicResidence.BR:
                    return ResidenceName.BriarwoodApartments;
                case DemographicResidence.BS:
                    return ResidenceName.BeersApartment;
                case DemographicResidence.BW:
                    return ResidenceName.BrownHouse;
                case DemographicResidence.CA:
                    return ResidenceName.CarriageHouse;
                case DemographicResidence.CAL:
                    return ResidenceName.CallisonHouse;
                case DemographicResidence.CD:
                    return ResidenceName.CoddingHouse;
                case DemographicResidence.CH:
                    return ResidenceName.CraneHouse;
                case DemographicResidence.CHA:
                    return ResidenceName.ChanceHouse;
                case DemographicResidence.CHU:
                    return ResidenceName.TheChurch;
                case DemographicResidence.CL:
                    return ResidenceName.CraigLuthyHouses;
                case DemographicResidence.CM:
                    return ResidenceName.CommuterStudent;
                case DemographicResidence.CMM:
                    return ResidenceName.CommuterMarried;
                case DemographicResidence.CMS:
                    return ResidenceName.CommuterSingle;
                case DemographicResidence.CP:
                    return ResidenceName.CasaPatriciaApartments;
                case DemographicResidence.CRH:
                    return ResidenceName.CramerHouse;
                case DemographicResidence.CT:
                    return ResidenceName.ClydeTaylorHouse;
                case DemographicResidence.DA:
                    return ResidenceName.DavenportHouse;
                case DemographicResidence.DDT:
                    return ResidenceName.DavenportTittleHouse;
                case DemographicResidence.DH:
                    return ResidenceName.DeckHouse;
                case DemographicResidence.DI:
                    return ResidenceName.DillerHouse;
                case DemographicResidence.DL:
                    return ResidenceName.DelongHouse;
                case DemographicResidence.DO:
                    return ResidenceName.DooleyHouse;
                case DemographicResidence.DT:
                    return ResidenceName.DeltaApartments;
                case DemographicResidence.EAH:
                    return ResidenceName.EastusHouse;
                case DemographicResidence.EH:
                    return ResidenceName.EnglishHall;
                case DemographicResidence.EK:
                    return ResidenceName.EricHayesApartments;
                case DemographicResidence.FA:
                    return ResidenceName.FairlaneApartments;
                case DemographicResidence.FAFC:
                    return ResidenceName.AwayFromCampusFW;
                case DemographicResidence.FD:
                    return ResidenceName.FoundersHall;
                case DemographicResidence.FO:
                    return ResidenceName.FodeHouse;
                case DemographicResidence.FOC:
                    return ResidenceName.OakwoodApartments;
                case DemographicResidence.FTMP:
                    return ResidenceName.TemporaryHousingFortWayne;
                case DemographicResidence.FYH:
                    return ResidenceName.FyffeHouse;
                case DemographicResidence.GCA:
                    return ResidenceName.GasCityApartments;
                case DemographicResidence.GH:
                    return ResidenceName.GerigHall;
                case DemographicResidence.GI:
                    return ResidenceName.GiffordHouse;
                case DemographicResidence.GL:
                    return ResidenceName.GlassHouse;
                case DemographicResidence.GLH:
                    return ResidenceName.GillenwaterHouse;
                case DemographicResidence.GR:
                    return ResidenceName.GortnerHouse;
                case DemographicResidence.GRS:
                    return ResidenceName.GrossHouse;
                case DemographicResidence.GSH:
                    return ResidenceName.GraduateStudentHousing;
                case DemographicResidence.HA:
                    return ResidenceName.HillApartment;
                case DemographicResidence.HAI:
                    return ResidenceName.HaireHouse;
                case DemographicResidence.HAY:
                    return ResidenceName.HayesApartment;
                case DemographicResidence.HH:
                    return ResidenceName.HausserHall;
                case DemographicResidence.HL:
                    return ResidenceName.HollowayHouse;
                case DemographicResidence.HN:
                    return ResidenceName.HornerApartment;
                case DemographicResidence.HR:
                    return ResidenceName.HarmonHouse;
                case DemographicResidence.HU:
                    return ResidenceName.HunterHouse;
                case DemographicResidence.IH:
                    return ResidenceName.InternationalHouse;
                case DemographicResidence.JC:
                    return ResidenceName.JeffersonStCottages;
                case DemographicResidence.JCA:
                    return ResidenceName.JordanCarriageApartment;
                case DemographicResidence.JM:
                    return ResidenceName.JordanMobile;
                case DemographicResidence.KRF:
                    return ResidenceName.KorfmacherHouse;
                case DemographicResidence.LE:
                    return ResidenceName.LeightnerHall;
                case DemographicResidence.LH:
                    return ResidenceName.LuthyFredHouses;
                case DemographicResidence.LJ:
                    return ResidenceName.LeffingwellJohnHouse;
                case DemographicResidence.LL:
                    return ResidenceName.LillianRolfHouse;
                case DemographicResidence.LW:
                    return ResidenceName.LawvereHouse;
                case DemographicResidence.MA:
                    return ResidenceName.MuncieApartments;
                case DemographicResidence.MI:
                    return ResidenceName.MillerApartments;
                case DemographicResidence.MO:
                    return ResidenceName.MooreChuckHouse;
                case DemographicResidence.MRA:
                    return ResidenceName.MarionApartments;
                case DemographicResidence.MT:
                    return ResidenceName.MuthiahHouse;
                case DemographicResidence.NTZ:
                    return ResidenceName.NitzscheHouse;
                case DemographicResidence.OC:
                    return ResidenceName.OffCampusHousing;
                case DemographicResidence.OH:
                    return ResidenceName.OlsonHall;
                case DemographicResidence.PA:
                    return ResidenceName.ParkvilleApartments;
                case DemographicResidence.PM:
                    return ResidenceName.PatMooreHouse;
                case DemographicResidence.RAI:
                    return ResidenceName.RaikesHouse;
                case DemographicResidence.RBH:
                    return ResidenceName.BreuningerHall;
                case DemographicResidence.RE:
                    return ResidenceName.ReeseHouse;
                case DemographicResidence.RI:
                    return ResidenceName.RiveraHouse;
                case DemographicResidence.RIC:
                    return ResidenceName.RiceHouse;
                case DemographicResidence.RM:
                    return ResidenceName.RamseyerHall;
                case DemographicResidence.RR:
                    return ResidenceName.RohrsHouse;
                case DemographicResidence.RS:
                    return ResidenceName.RossApartments;
                case DemographicResidence.SC:
                    return ResidenceName.SchultzHall;
                case DemographicResidence.SG:
                    return ResidenceName.SongerHouse;
                case DemographicResidence.SM:
                    return ResidenceName.SammyMorrisHall;
                case DemographicResidence.SPC:
                    return ResidenceName.SpeicherHouse;
                case DemographicResidence.SPN:
                    return ResidenceName.SpencerHouse;
                case DemographicResidence.SR:
                    return ResidenceName.SwallowRobinHall;
                case DemographicResidence.ST:
                    return ResidenceName.StevensHouse;
                case DemographicResidence.SW:
                    return ResidenceName.SchweichartHouse;
                case DemographicResidence.TBH:
                    return ResidenceName.BowersHouse;
                case DemographicResidence.TD:
                    return ResidenceName.TedderJackHouse;
                case DemographicResidence.TH:
                    return ResidenceName.TrueHouse;
                case DemographicResidence.TS:
                    return ResidenceName.TaylorStreetHouse;
                case DemographicResidence.U1A:
                    return ResidenceName.CampbellHall;
                case DemographicResidence.U2A:
                    return ResidenceName.WolgemuthHall;
                case DemographicResidence.UHH:
                    return ResidenceName.HaakonsenHall;
                case DemographicResidence.UTMP:
                    return ResidenceName.TemporaryHousingUpland;
                case DemographicResidence.VA:
                    return ResidenceName.VossApartment;
                case DemographicResidence.WB:
                    return ResidenceName.WiebkeHouse;
                case DemographicResidence.WH:
                    return ResidenceName.WengatzHall;
                case DemographicResidence.WR:
                    return ResidenceName.WrenHouse;
                case DemographicResidence.WUA:
                    return ResidenceName.WilliamsUnitA;
                case DemographicResidence.WUC:
                    return ResidenceName.WilliamsUnitC;
                case DemographicResidence.WUD:
                    return ResidenceName.WilliamsUnitD;
                case DemographicResidence.WW:
                    return ResidenceName.WanderingWheelsApartments;
                case DemographicResidence.YH:
                    return ResidenceName.YoungHouse;
            }

            throw new NotImplementedException(residence.ToString());
        }

        public static ResidenceCategory GetResidenceCategory(DemographicResidence residence)
        {
            switch (residence)
            {
                case DemographicResidence.AFC:
                case DemographicResidence.FAFC:
                    return ResidenceCategory.AwayFromCampus;

                case DemographicResidence.AA:
                case DemographicResidence.AH:
                case DemographicResidence.BE:
                case DemographicResidence.BH:
                case DemographicResidence.CHA:
                case DemographicResidence.CRH:
                case DemographicResidence.CT:
                case DemographicResidence.DH:
                case DemographicResidence.DL:
                case DemographicResidence.EAH:
                case DemographicResidence.EH:
                case DemographicResidence.FD:
                case DemographicResidence.FOC:
                case DemographicResidence.FTMP:
                case DemographicResidence.FYH:
                case DemographicResidence.GCA:
                case DemographicResidence.GH:
                case DemographicResidence.GLH:
                case DemographicResidence.GSH:
                case DemographicResidence.HA:
                case DemographicResidence.HAI:
                case DemographicResidence.HAY:
                case DemographicResidence.HH:
                case DemographicResidence.HN:
                case DemographicResidence.JCA:
                case DemographicResidence.LE:
                case DemographicResidence.LL:
                case DemographicResidence.MA:
                case DemographicResidence.MRA:
                case DemographicResidence.NTZ:
                case DemographicResidence.OC:
                case DemographicResidence.OH:
                case DemographicResidence.RBH:
                case DemographicResidence.RIC:
                case DemographicResidence.RM:
                case DemographicResidence.SC:
                case DemographicResidence.SM:
                case DemographicResidence.SPC:
                case DemographicResidence.SR:
                case DemographicResidence.U1A:
                case DemographicResidence.U2A:
                case DemographicResidence.UHH:
                case DemographicResidence.UTMP:
                case DemographicResidence.VA:
                case DemographicResidence.WB:
                case DemographicResidence.WH:
                    return ResidenceCategory.OnCampus;

                case DemographicResidence.BA:
                case DemographicResidence.BC:
                case DemographicResidence.BD:
                case DemographicResidence.BEL:
                case DemographicResidence.BK:
                case DemographicResidence.BN:
                case DemographicResidence.BR:
                case DemographicResidence.BS:
                case DemographicResidence.BW:
                case DemographicResidence.CA:
                case DemographicResidence.CAL:
                case DemographicResidence.CD:
                case DemographicResidence.CH:
                case DemographicResidence.CHU:
                case DemographicResidence.CL:
                case DemographicResidence.CP:
                case DemographicResidence.DA:
                case DemographicResidence.DDT:
                case DemographicResidence.DI:
                case DemographicResidence.DO:
                case DemographicResidence.DT:
                case DemographicResidence.EK:
                case DemographicResidence.FA:
                case DemographicResidence.FO:
                case DemographicResidence.GI:
                case DemographicResidence.GL:
                case DemographicResidence.GR:
                case DemographicResidence.GRS:
                case DemographicResidence.HL:
                case DemographicResidence.HR:
                case DemographicResidence.HU:
                case DemographicResidence.IH:
                case DemographicResidence.JC:
                case DemographicResidence.JM:
                case DemographicResidence.KRF:
                case DemographicResidence.LH:
                case DemographicResidence.LJ:
                case DemographicResidence.LW:
                case DemographicResidence.MI:
                case DemographicResidence.MO:
                case DemographicResidence.MT:
                case DemographicResidence.PA:
                case DemographicResidence.PM:
                case DemographicResidence.RAI:
                case DemographicResidence.RE:
                case DemographicResidence.RI:
                case DemographicResidence.RR:
                case DemographicResidence.RS:
                case DemographicResidence.SG:
                case DemographicResidence.SPN:
                case DemographicResidence.ST:
                case DemographicResidence.SW:
                case DemographicResidence.TBH:
                case DemographicResidence.TD:
                case DemographicResidence.TH:
                case DemographicResidence.TS:
                case DemographicResidence.WR:
                case DemographicResidence.WUA:
                case DemographicResidence.WUC:
                case DemographicResidence.WUD:
                case DemographicResidence.WW:
                case DemographicResidence.YH:
                    return ResidenceCategory.OffCampus;

                case DemographicResidence.CMM:
                case DemographicResidence.CMS:
                case DemographicResidence.CM:
                    return ResidenceCategory.Commuter;
            }

            throw new NotImplementedException(residence.ToString());
        }

        public static MajorName GetMajorName(DemographicMajor major)
        {
            switch (major)
            {
                case DemographicMajor._0000:
                    return MajorName.NotDeclared;
                case DemographicMajor.AAC:
                    return MajorName.AssociateAccounting;
                case DemographicMajor.ABA:
                    return MajorName.AssociatebusinessAdmin;
                case DemographicMajor.ACC:
                    return MajorName.Accounting;
                case DemographicMajor.ACCS:
                    return MajorName.AccountingSystems;
                case DemographicMajor.AIA:
                    return MajorName.AccountingInfoApplications;
                case DemographicMajor.AMU:
                    return MajorName.AssociateMusic;
                case DemographicMajor.ART:
                    return MajorName.Art;
                case DemographicMajor.ARTE:
                    return MajorName.ArtEducation;
                case DemographicMajor.ARTS:
                    return MajorName.ArtSystems;
                case DemographicMajor.ATR:
                    return MajorName.AthleticTraining;
                case DemographicMajor.BBA:
                    return MajorName.BachelorBusinessAdministration;
                case DemographicMajor.BBS:
                    return MajorName.BiblicalStudies;
                case DemographicMajor.BCH:
                    return MajorName.Biochemistry;
                case DemographicMajor.BIA:
                    return MajorName.BusinessInfoApplications;
                case DemographicMajor.BIB:
                    return MajorName.BiblicalLiterature;
                case DemographicMajor.BIBS:
                    return MajorName.BiblicalLiteratureSystems;
                case DemographicMajor.BIO:
                    return MajorName.Biology;
                case DemographicMajor.BIOE:
                    return MajorName.BiologyScienceEducation;
                case DemographicMajor.BIOM:
                    return MajorName.BiologyPremed;
                case DemographicMajor.BIOS:
                    return MajorName.BiologySystems;
                case DemographicMajor.BUA:
                    return MajorName.BusinessAdministration;
                case DemographicMajor.BUAS:
                    return MajorName.BusinessAdministrationSystems;
                case DemographicMajor.CAE:
                    return MajorName.CommunicationArtsEducation;
                case DemographicMajor.CAM:
                    return MajorName.MassCommunication;
                case DemographicMajor.CAMS:
                    return MajorName.MassCommunicationSystems;
                case DemographicMajor.CAS:
                    return MajorName.CommunicationStudies;
                case DemographicMajor.CASN:
                    return MajorName.CommunicationStudiesEnvSci;
                case DemographicMajor.CASS:
                    return MajorName.CommunicationStudiesSystems;
                case DemographicMajor.CAT:
                    return MajorName.TheatreArts;
                case DemographicMajor.CATS:
                    return MajorName.TheatreArtsSystems;
                case DemographicMajor.CDM:
                    return MajorName.ComputerScienceDigitalMedia;
                case DemographicMajor.CDMS:
                    return MajorName.ComputerScienceDigitalMediaSystems;
                case DemographicMajor.CED:
                    return MajorName.ChristianEducation;
                case DemographicMajor.CEDS:
                    return MajorName.ChristianEducationSystems;
                case DemographicMajor.CEM:
                    return MajorName.ChristianEducationMinistries;
                case DemographicMajor.CEMS:
                    return MajorName.ChristianEducationMinistriesSystems;
                case DemographicMajor.CEN:
                    return MajorName.ComputerEngineering;
                case DemographicMajor.CENS:
                    return MajorName.ComputerEngineeringSystems;
                case DemographicMajor.CES:
                    return MajorName.ChemistryEnvironmentalScience;
                case DemographicMajor.CESS:
                    return MajorName.ChemistryEnvironmentalScienceSystems;
                case DemographicMajor.CGA:
                    return MajorName.ComputerGraphicArts;
                case DemographicMajor.CGAS:
                    return MajorName.ComputerGraphicArtsSystems;
                case DemographicMajor.CHE:
                    return MajorName.Chemistry;
                case DemographicMajor.CHEE:
                    return MajorName.ChemistryScienceEducation;
                case DemographicMajor.CHES:
                    return MajorName.ChemistrySystems;
                case DemographicMajor.CIA:
                    return MajorName.ComputingAndInfoApplications;
                case DemographicMajor.CIS:
                    return MajorName.ComputerInformationSystems;
                case DemographicMajor.CISS:
                    return MajorName.ComputerInformationSystemsSystems;
                case DemographicMajor.CMCC:
                    return MajorName.ChristianMinistriesCrossCultural;
                case DemographicMajor.CMCE:
                    return MajorName.ChristianMinistriesChristianEducation;
                case DemographicMajor.CMI:
                    return MajorName.ChristianMinistries;
                case DemographicMajor.CMJ:
                    return MajorName.MassCommunicationJournalism;
                case DemographicMajor.CMJS:
                    return MajorName.MassCommunicationJournalismSystems;
                case DemographicMajor.CMMM:
                    return MajorName.ChristianMinistriesMusic;
                case DemographicMajor.CMPM:
                    return MajorName.ChristianMinistriesPastoral;
                case DemographicMajor.CNM:
                    return MajorName.MediaCommunication;
                case DemographicMajor.CNMS:
                    return MajorName.CommunicationNewMediaSystems;
                case DemographicMajor.COS:
                case DemographicMajor.CSC:
                    return MajorName.ComputerScience;
                case DemographicMajor.COSS:
                    return MajorName.ComputerScienceSystems;
                case DemographicMajor.CRJ:
                    return MajorName.CriminalJustice;
                case DemographicMajor.CRJS:
                    return MajorName.CriminalJusticeSystems;
                case DemographicMajor.CSG:
                    return MajorName.Counseling;
                case DemographicMajor.CSM:
                    return MajorName.ComputerScienceNewMedia;
                case DemographicMajor.CSMS:
                    return MajorName.ComputerScienceNewMediaSystems;
                case DemographicMajor.DEC:
                    return MajorName.DevelopmentalEconomics;
                case DemographicMajor.DECS:
                    return MajorName.DevelopmentalEconomicsSystems;
                case DemographicMajor.ECE:
                    return MajorName.EarlyChildhoodEducation;
                case DemographicMajor.ECO:
                    return MajorName.Economics;
                case DemographicMajor.ECOS:
                    return MajorName.EconomicsSystems;
                case DemographicMajor.EDS:
                    return MajorName.EducationalStudies;
                case DemographicMajor.EED:
                    return MajorName.ElementaryEducation;
                case DemographicMajor.EEN:
                    return MajorName.EnvironmentalEngineering;
                case DemographicMajor.EGE:
                    return MajorName.EnvironmentalGeology;
                case DemographicMajor.ENB:
                    return MajorName.EnvironmentalBiology;
                case DemographicMajor.ENBS:
                    return MajorName.EnvironmentalBiologySystems;
                case DemographicMajor.ENG:
                    return MajorName.English;
                case DemographicMajor.ENGE:
                    return MajorName.EnglishEducation;
                case DemographicMajor.ENGL:
                    return MajorName.EnglishLiterature;
                case DemographicMajor.ENGS:
                    return MajorName.EnglishSystems;
                case DemographicMajor.ENGW:
                    return MajorName.EnglishWriting;
                case DemographicMajor.ENM:
                    return MajorName.EnvironmentalManagement;
                case DemographicMajor.ENMS:
                    return MajorName.EnvironmentalManagementSystems;
                case DemographicMajor.ENR:
                    return MajorName.Engineering;
                case DemographicMajor.ENS:
                    return MajorName.EnvironmentalScience;
                case DemographicMajor.EPH:
                    return MajorName.EngineeringPhysics;
                case DemographicMajor.EPHS:
                    return MajorName.EngineeringPhysicsSystems;
                case DemographicMajor.EST:
                    return MajorName.EnglishStudies;
                case DemographicMajor.EXS:
                    return MajorName.ExerciseScience;
                case DemographicMajor.FIN:
                    return MajorName.Finance;
                case DemographicMajor.FMP:
                    return MajorName.FilmAndMediaProduction;
                case DemographicMajor.FRE:
                    return MajorName.French;
                case DemographicMajor.FREE:
                    return MajorName.FrenchEducation;
                case DemographicMajor.GAT:
                    return MajorName.GraphicArt;
                case DemographicMajor.GATS:
                    return MajorName.GraphicArtSystems;
                case DemographicMajor.GBUA:
                    return MajorName.GraduateBusinessAdministration;
                case DemographicMajor.GENS:
                    return MajorName.GraduateEnvironmentalScience;
                case DemographicMajor.GEO:
                    return MajorName.Geography;
                case DemographicMajor.GHES:
                    return MajorName.GraduateHigherEducationStudentDevelopment;
                case DemographicMajor.GRST:
                    return MajorName.GraduateReligiousStudies;
                case DemographicMajor.HCM:
                    return MajorName.HealthcareManagement;
                case DemographicMajor.HES:
                    return MajorName.HigherEducationStudentDevelopment;
                case DemographicMajor.HIS:
                    return MajorName.History;
                case DemographicMajor.HISN:
                    return MajorName.HistoryEnvironmentalScience;
                case DemographicMajor.HISS:
                    return MajorName.HistorySystems;
                case DemographicMajor.HMS:
                    return MajorName.HumanServices;
                case DemographicMajor.HPEE:
                    return MajorName.HealthPhysicalEducation;
                case DemographicMajor.HPR:
                case DemographicMajor.HPRE:
                    return MajorName.PhysicalEducation;
                case DemographicMajor.HPRS:
                    return MajorName.PhysicalEducationSystems;
                case DemographicMajor.ICS:
                    return MajorName.InterculturalStudies;
                case DemographicMajor.IDS:
                    return MajorName.InterdisciplinaryStudies;
                case DemographicMajor.ING:
                    return MajorName.IndividualGoalOriented;
                case DemographicMajor.INGS:
                    return MajorName.IndividualGoalOrientedSystems;
                case DemographicMajor.ITB:
                    return MajorName.InternationalBusiness;
                case DemographicMajor.ITBS:
                    return MajorName.InternationalBusinessSystems;
                case DemographicMajor.ITS:
                    return MajorName.InternationalStudies;
                case DemographicMajor.ITSS:
                    return MajorName.InternationalStudiesSystems; // Not in file but still included.
                case DemographicMajor.JAD:
                    return MajorName.JusticeAdministration;
                case DemographicMajor.JMI:
                    return MajorName.JusticeAndMinistry;
                case DemographicMajor.JMW:
                    return MajorName.JournalismMediaWriting;
                case DemographicMajor.JRN:
                    return MajorName.Journalism;
                case DemographicMajor.LBA:
                    return MajorName.LiberalArts;
                case DemographicMajor.LJU:
                    return MajorName.LawAndJustice;
                case DemographicMajor.MAED:
                    return MajorName.MathematicsEducation;
                case DemographicMajor.MAI:
                    return MajorName.MathematicsInterdisciplinary;
                case DemographicMajor.MAT:
                    return MajorName.Mathematics;
                case DemographicMajor.MATE:
                    return MajorName.MathematicsScienceEducation;
                case DemographicMajor.MATS:
                    return MajorName.MathematicsSystems;
                case DemographicMajor.MCM:
                    return MajorName.MediaCommunication;
                case DemographicMajor.MCMS:
                    return MajorName.MediaCommunicationSystems;
                case DemographicMajor.MES:
                    return MajorName.MathematicsEnvironmentalScience;
                case DemographicMajor.MESS:
                    return MajorName.MathematicsEnivornmentalScienceSystems;
                case DemographicMajor.MGT:
                    return MajorName.Management;
                case DemographicMajor.MGTS:
                    return MajorName.ManagementSystems;
                case DemographicMajor.MIS:
                    return MajorName.ManagementInformationSystems;
                case DemographicMajor.MKC:
                    return MajorName.MarketingCommunication;
                case DemographicMajor.MKT:
                    return MajorName.Marketing;
                case DemographicMajor.MKTS:
                    return MajorName.MarketingSystems;
                case DemographicMajor.MPF:
                    return MajorName.FilmAndMediaProduction;
                case DemographicMajor.MPFS:
                    return MajorName.FilmAndMediaProductionSystems;
                case DemographicMajor.MPR:
                    return MajorName.MediaProduction;
                case DemographicMajor.MPRS:
                    return MajorName.MediaProductionSystems;
                case DemographicMajor.MTR:
                    return MajorName.MusicalTheatre;
                case DemographicMajor.MUCP:
                    return MajorName.MusicTheoryComposition;
                case DemographicMajor.MUPF:
                    return MajorName.MusicVocalPerformance;
                case DemographicMajor.MUS:
                    return MajorName.Music;
                case DemographicMajor.MUSE:
                    return MajorName.MusicEducation;
                case DemographicMajor.MUSS:
                    return MajorName.MusicSystems;
                case DemographicMajor.MWR:
                    return MajorName.MediaWriting;
                case DemographicMajor.MWRS:
                    return MajorName.MediaWritingSystems;
                case DemographicMajor.NAS:
                    return MajorName.NaturalScience;
                case DemographicMajor.NASS:
                    return MajorName.NaturalScienceSystems;
                case DemographicMajor.OMG:
                    return MajorName.OrganizationalManagement;
                case DemographicMajor.PAT:
                    return MajorName.PreArtTherapy;
                case DemographicMajor.PBH:
                    return MajorName.PublicHealth;
                case DemographicMajor.PBR:
                    return MajorName.PublicRelations;
                case DemographicMajor.PBRS:
                    return MajorName.PublicRelationsSystems;
                case DemographicMajor.PESL:
                    return MajorName.PrepatoryESL;
                case DemographicMajor.PHEP:
                    return MajorName.PhysicsEngineeringPhysics;
                case DemographicMajor.PHI:
                    return MajorName.Philosophy;
                case DemographicMajor.PHIS:
                    return MajorName.PhilosophySystems;
                case DemographicMajor.PHME:
                    return MajorName.PhysicsMathematicsEducation;
                case DemographicMajor.PHY:
                    return MajorName.Physics;
                case DemographicMajor.PHYE:
                    return MajorName.PhysicsScienceEducation;
                case DemographicMajor.PHYN:
                    return MajorName.PhysicsEnvironmentalScience;
                case DemographicMajor.PHYS:
                    return MajorName.PhysicsSystems;
                case DemographicMajor.PMI:
                case DemographicMajor.PMIN:
                    return MajorName.PastoralMinistries;
                case DemographicMajor.POS:
                    return MajorName.PoliticalScience;
                case DemographicMajor.POSS:
                    return MajorName.PoliticalScienceSystems;
                case DemographicMajor.PPE:
                    return MajorName.PoliticalSciencePhilosophyEconomics;
                case DemographicMajor.PPES:
                    return MajorName.PoliticalSciencePhilosophyEconomicsSystems;
                case DemographicMajor.PRE:
                    return MajorName.PreMajor;
                case DemographicMajor.PREP:
                    return MajorName.PreMajorPreparatory;
                case DemographicMajor.PSCE:
                    return MajorName.PhysicalScienceEducation;
                case DemographicMajor.PSY:
                    return MajorName.Psychology;
                case DemographicMajor.PSYN:
                    return MajorName.PsychologyEnvironmentalScience;
                case DemographicMajor.PSYS:
                    return MajorName.PsychologySystems;
                case DemographicMajor.PWR:
                    return MajorName.ProfessionalWriting;
                case DemographicMajor.PWRS:
                    return MajorName.ProfessionalWritingSystems;
                case DemographicMajor.RES:
                    return MajorName.ReligiousStudies;
                case DemographicMajor.RLE:
                    return MajorName.RecreationalLeadership;
                case DemographicMajor.SAR:
                    return MajorName.StudioArt;
                case DemographicMajor.SEN:
                    return MajorName.SystemsEngineering;
                case DemographicMajor.SMA:
                    return MajorName.SportManagement;
                case DemographicMajor.SMAS:
                    return MajorName.SportManagementSystems;
                case DemographicMajor.SOC:
                    return MajorName.Sociology;
                case DemographicMajor.SOCS:
                    return MajorName.SociologySystems;
                case DemographicMajor.SOS:
                    return MajorName.SocialStudies;
                case DemographicMajor.SOSE:
                    return MajorName.SocialStudiesEducation;
                case DemographicMajor.SPA:
                    return MajorName.Spanish;
                case DemographicMajor.SPAE:
                    return MajorName.SpanishEducation;
                case DemographicMajor.SPAS:
                    return MajorName.SpanishSystems;
                case DemographicMajor.SSGE:
                    return MajorName.SocialStudiesEducationGeography;
                case DemographicMajor.SSGO:
                    return MajorName.SocialStudiesEducationGovernment;
                case DemographicMajor.SSPS:
                    return MajorName.SocialStudiesEducationPsychology;
                case DemographicMajor.SSSO:
                    return MajorName.SocialStudiesEducationSociology;
                case DemographicMajor.SSUS:
                    return MajorName.SocialStudiesEducationUSHistory;
                case DemographicMajor.SSWC:
                    return MajorName.SocialStudiesEducationWorldCiv;
                case DemographicMajor.SUS:
                    return MajorName.SustainableDevelopment;
                case DemographicMajor.SWK:
                    return MajorName.SocialWork;
                case DemographicMajor.SWKS:
                    return MajorName.SocialWorkSystems;
                case DemographicMajor.THR:
                    return MajorName.TheatreArts;
                case DemographicMajor.TMT:
                    return MajorName.TechnologyManagement;
                case DemographicMajor.TSED:
                    return MajorName.SpecialEducationLicProg;
                case DemographicMajor.TTE:
                    return MajorName.TransitionTeachingElementary;
                case DemographicMajor.TTS:
                    return MajorName.TransitionTeachingSecondary;
                case DemographicMajor.UBUS:
                    return MajorName.UndeclaredBusiness;
                case DemographicMajor.UEDU:
                    return MajorName.UndeclaredSecondaryEducation;
                case DemographicMajor.UMCM:
                    return MajorName.UndeclaredMediaCommunication;
                case DemographicMajor.UMI:
                case DemographicMajor.UMIN:
                    return MajorName.UrbanMinistries;
                case DemographicMajor.VAM:
                    return MajorName.VisualArtsNewMedia;
                case DemographicMajor.VAMS:
                    return MajorName.VisualArtsNewMediaSystems;
                case DemographicMajor.VAR:
                    return MajorName.VisualArts;
                case DemographicMajor.VARS:
                    return MajorName.VisualArtsSystems;
                case DemographicMajor.VMI:
                    return MajorName.VocationalMinistry;
                case DemographicMajor.WBC:
                    return MajorName.WebCommunication;
                case DemographicMajor.WBCS:
                    return MajorName.WebCommunicationSystems;
                case DemographicMajor.WEL:
                    return MajorName.Wellness;
                case DemographicMajor.YBIB:
                    return MajorName.Bible;
                case DemographicMajor.YMI:
                    return MajorName.YouthMinistries;
                case DemographicMajor.YMIS:
                    return MajorName.YouthMinistriesSystems;
            }

            throw new NotImplementedException(major.ToString());
        }

        //Not Needed for our data.
        /*public static MajorBase GetMajorBase(DemographicMajor major)
        {
            switch (major)
            {
                case DemographicMajor._0000:
                case DemographicMajor.PRE:
                case DemographicMajor.PREP:
                    return MajorBase.NotDeclared;

                case DemographicMajor.ACC:
                case DemographicMajor.ACCS:
                    return MajorBase.Accounting;

                case DemographicMajor.ART:
                case DemographicMajor.ARTE:
                case DemographicMajor.ARTS:
                case DemographicMajor.GAT:
                case DemographicMajor.PAT:
                case DemographicMajor.VAR:
                case DemographicMajor.VARS:
                    return MajorBase.Art;

                case DemographicMajor.BBA:
                case DemographicMajor.BUA:
                case DemographicMajor.ITB:
                case DemographicMajor.ITBS:
                case DemographicMajor.MGT:
                case DemographicMajor.MGTS:
                case DemographicMajor.UBUS:
                case DemographicMajor.GBUA:
                    return MajorBase.Business;

                case DemographicMajor.BBS:
                case DemographicMajor.BIB:
                case DemographicMajor.BIBS:
                case DemographicMajor.CED:
                case DemographicMajor.CEM:
                case DemographicMajor.CEMS:
                case DemographicMajor.CMI:
                    return MajorBase.BiblicalLiterature;

                case DemographicMajor.BCH:
                case DemographicMajor.CHE:
                case DemographicMajor.CHEE:
                case DemographicMajor.CHES:
                    return MajorBase.Chemistry;

                case DemographicMajor.BIO:
                case DemographicMajor.BIOE:
                case DemographicMajor.BIOS:
                    return MajorBase.Biology;

                case DemographicMajor.CAS:
                case DemographicMajor.CASS:
                case DemographicMajor.MCM:
                case DemographicMajor.MCMS:
                case DemographicMajor.MPF:
                case DemographicMajor.MPFS:
                case DemographicMajor.MPR:
                case DemographicMajor.MPRS:
                case DemographicMajor.PBR:
                case DemographicMajor.PBRS:
                case DemographicMajor.UMCM:
                case DemographicMajor.WBC:
                case DemographicMajor.WBCS:
                    return MajorBase.Communications;

                case DemographicMajor.CAT:
                case DemographicMajor.CATS:
                case DemographicMajor.THR:
                    return MajorBase.Theatre;

                case DemographicMajor.CDM:
                case DemographicMajor.CDMS:
                case DemographicMajor.COS:
                case DemographicMajor.COSS:
                case DemographicMajor.CSC:
                case DemographicMajor.CSM:
                case DemographicMajor.CSMS:
                    return MajorBase.ComputerScience;

                case DemographicMajor.CEN:
                case DemographicMajor.EPH:
                case DemographicMajor.SEN:
                    return MajorBase.Engineering;

                case DemographicMajor.CES:
                case DemographicMajor.ENS:
                case DemographicMajor.EEN:
                    return MajorBase.EnvironmentalScience;

                case DemographicMajor.DEC:
                case DemographicMajor.DECS:
                case DemographicMajor.ECO:
                case DemographicMajor.ECOS:
                    return MajorBase.Economics;

                case DemographicMajor.ECE:
                case DemographicMajor.EDS:
                case DemographicMajor.EED:
                case DemographicMajor.GHES:
                case DemographicMajor.PESL:
                case DemographicMajor.TTE:
                case DemographicMajor.TTS:
                case DemographicMajor.UEDU:
                    return MajorBase.Education;

                case DemographicMajor.ENG:
                case DemographicMajor.ENGE:
                case DemographicMajor.ENGS:
                    return MajorBase.English;

                case DemographicMajor.EXS:
                case DemographicMajor.HPEE:
                case DemographicMajor.PBH:
                case DemographicMajor.PSCE:
                    return MajorBase.Health;

                case DemographicMajor.FIN:
                    return MajorBase.Finance;

                case DemographicMajor.FRE:
                    return MajorBase.French;

                case DemographicMajor.GENS:
                case DemographicMajor.SUS:
                    return MajorBase.EnvironmentalScience;

                case DemographicMajor.GEO:
                    return MajorBase.Geography;

                case DemographicMajor.HIS:
                case DemographicMajor.HISS:
                    return MajorBase.History;

                case DemographicMajor.ING:
                    return MajorBase.GoalOriented;

                case DemographicMajor.ITS:
                case DemographicMajor.ITSS:
                    return MajorBase.InternationalStudies;

                case DemographicMajor.JMW:
                case DemographicMajor.MWR:
                case DemographicMajor.MWRS:
                case DemographicMajor.PWR:
                    return MajorBase.Writing;

                case DemographicMajor.LBA:
                    return MajorBase.LiberalArts;

                case DemographicMajor.MAED:
                case DemographicMajor.MAI:
                case DemographicMajor.MAT:
                case DemographicMajor.MATS:
                    return MajorBase.Mathematics;

                case DemographicMajor.MKT:
                case DemographicMajor.MKTS:
                    return MajorBase.Marketing;

                case DemographicMajor.MUCP:
                case DemographicMajor.MUPF:
                case DemographicMajor.MUS:
                case DemographicMajor.MUSE:
                    return MajorBase.Music;

                case DemographicMajor.PHI:
                case DemographicMajor.PHIS:
                    return MajorBase.Philosophy;

                case DemographicMajor.PHME:
                case DemographicMajor.PHY:
                case DemographicMajor.PHYE:
                case DemographicMajor.PHYS:
                    return MajorBase.Physics;

                case DemographicMajor.POS:
                case DemographicMajor.POSS:
                case DemographicMajor.PPE:
                case DemographicMajor.PPES:
                    return MajorBase.Politics;

                case DemographicMajor.PSY:
                case DemographicMajor.PSYS:
                    return MajorBase.Psychology;

                case DemographicMajor.SMA:
                case DemographicMajor.SMAS:
                    return MajorBase.Sports;

                case DemographicMajor.SOC:
                case DemographicMajor.SOCS:
                    return MajorBase.Sociology;

                case DemographicMajor.SOSE:
                    return MajorBase.SocialStudies;

                case DemographicMajor.SPA:
                case DemographicMajor.SPAE:
                case DemographicMajor.SPAS:
                    return MajorBase.Spanish;

                case DemographicMajor.SWK:
                    return MajorBase.SocialWork;
            }

            throw new NotImplementedException(major.ToString());
        }*/

        //Not Needed for our data.
        /*public static Department GetDepartment(DemographicMajor major)
        {
            switch (major)
            {
                case DemographicMajor.ART:
                case DemographicMajor.ARTE:
                case DemographicMajor.ARTS:
                case DemographicMajor.GAT:
                case DemographicMajor.PAT:
                case DemographicMajor.VAR:
                case DemographicMajor.VARS:
                    return Department.Art;

                case DemographicMajor.CED:
                case DemographicMajor.CEM:
                case DemographicMajor.CEMS:
                case DemographicMajor.CMI:
                case DemographicMajor.PHI:
                case DemographicMajor.PHIS:
                case DemographicMajor.BIB:
                case DemographicMajor.BIBS:
                case DemographicMajor.BBS:
                    return Department.BiblicalStudiesChristianEducationPhilosophy;

                case DemographicMajor.BIO:
                case DemographicMajor.BIOE:
                case DemographicMajor.BIOS:
                case DemographicMajor.PBH:
                    return Department.Biology;

                case DemographicMajor.MKT:
                case DemographicMajor.MKTS:
                case DemographicMajor.ITB:
                case DemographicMajor.ITBS:
                case DemographicMajor.MGT:
                case DemographicMajor.MGTS:
                case DemographicMajor.DEC:
                case DemographicMajor.DECS:
                case DemographicMajor.ACC:
                case DemographicMajor.ACCS:
                case DemographicMajor.ECO:
                case DemographicMajor.ECOS:
                case DemographicMajor.FIN:
                case DemographicMajor.BBA:
                case DemographicMajor.BUA:
                case DemographicMajor.UBUS:
                case DemographicMajor.GBUA:
                    return Department.Business;

                case DemographicMajor.CHE:
                case DemographicMajor.CHEE:
                case DemographicMajor.CHES:
                case DemographicMajor.BCH:
                case DemographicMajor.CES:
                    return Department.ChemistryBiochemistry;

                case DemographicMajor.CAS:
                case DemographicMajor.CASS:
                case DemographicMajor.CAT:
                case DemographicMajor.CATS:
                case DemographicMajor.THR:
                    return Department.Communication;

                case DemographicMajor.COS:
                case DemographicMajor.COSS:
                case DemographicMajor.CSC:
                case DemographicMajor.CSM:
                case DemographicMajor.CSMS:
                case DemographicMajor.CDM:
                case DemographicMajor.CDMS:
                case DemographicMajor.CEN:
                    return Department.ComputerScienceEngineering;

                case DemographicMajor.ENS:
                case DemographicMajor.EEN:
                case DemographicMajor.GEO:
                case DemographicMajor.SUS:
                    return Department.EarthEnvironmentalSciences;

                case DemographicMajor.ECE:
                case DemographicMajor.EED:
                case DemographicMajor.EDS:
                case DemographicMajor.PESL:
                case DemographicMajor.TTE:
                case DemographicMajor.TTS:
                case DemographicMajor.UEDU:
                    return Department.Education;

                case DemographicMajor.ENG:
                case DemographicMajor.ENGS:
                case DemographicMajor.ENGE:
                    return Department.English;

                case DemographicMajor.HIS:
                case DemographicMajor.HISS:
                case DemographicMajor.ITS:
                case DemographicMajor.ITSS:
                case DemographicMajor.SOSE:
                    return Department.HistoryInternationalStudiesSocialStudies;

                case DemographicMajor.ING:
                    return Department.Independent;

                case DemographicMajor.SMA:
                case DemographicMajor.SMAS:
                case DemographicMajor.EXS:
                case DemographicMajor.HPEE:
                case DemographicMajor.PSCE:
                    return Department.Kinesiology;

                case DemographicMajor.LBA:
                    return Department.LiberalArts;

                case DemographicMajor.MAT:
                case DemographicMajor.MATS:
                case DemographicMajor.MAI:
                case DemographicMajor.MAED:
                    return Department.Mathematics;

                case DemographicMajor.GHES:
                    return Department.MasterArtsHigherEducationStudentDevelopment;

                case DemographicMajor.GENS:
                    return Department.MasterEnvironmentalScience;

                case DemographicMajor.JMW:
                case DemographicMajor.MPF:
                case DemographicMajor.MPFS:
                case DemographicMajor.PBR:
                case DemographicMajor.PBRS:
                case DemographicMajor.WBC:
                case DemographicMajor.WBCS:
                case DemographicMajor.MWR:
                case DemographicMajor.MWRS:
                case DemographicMajor.MCM:
                case DemographicMajor.MCMS:
                case DemographicMajor.MPR:
                case DemographicMajor.MPRS:
                case DemographicMajor.UMCM:
                    return Department.MediaCommunication;

                case DemographicMajor.FRE:
                case DemographicMajor.SPA:
                case DemographicMajor.SPAE:
                case DemographicMajor.SPAS:
                    return Department.ModernLanguages;

                case DemographicMajor.MUS:
                case DemographicMajor.MUSE:
                case DemographicMajor.MUCP:
                case DemographicMajor.MUPF:
                    return Department.Music;

                case DemographicMajor.PHY:
                case DemographicMajor.EPH:
                case DemographicMajor.PHYS:
                case DemographicMajor.PHME:
                case DemographicMajor.PHYE:
                case DemographicMajor.SEN:
                    return Department.PhysicsEngineering;

                case DemographicMajor.POS:
                case DemographicMajor.POSS:
                case DemographicMajor.PPE:
                case DemographicMajor.PPES:
                    return Department.PoliticalScienceInternationalRelations;

                case DemographicMajor.PWR:
                    return Department.ProfessionalWriting;

                case DemographicMajor.PSY:
                case DemographicMajor.PSYS:
                    return Department.Psychology;

                case DemographicMajor.SWK:
                    return Department.SocialWork;

                case DemographicMajor.SOC:
                case DemographicMajor.SOCS:
                    return Department.Sociology;

                case DemographicMajor._0000:
                case DemographicMajor.PRE:
                case DemographicMajor.PREP:
                    return Department.Undeclared;
            }

            throw new NotImplementedException(major.ToString());
        }*/

        // Cannot be determined with the level of information we are getting.
        //private DegreeType Convert(DemographicMajor major)
        //{
        //    switch (major)
        //    {
        //        case DemographicMajor.LBA:
        //            return DegreeType.AssociateArts;

        //        case DemographicMajor.ACC:
        //        case DemographicMajor.ART:
        //        case DemographicMajor.ARTE:
        //        case DemographicMajor.BIB:
        //        case DemographicMajor.BIO:
        //        case DemographicMajor.BIOE:
        //        case DemographicMajor.CHE:
        //        case DemographicMajor.CHEE:
        //        case DemographicMajor.CEM:
        //        case DemographicMajor.CAS:
        //        case DemographicMajor.COS:
        //        case DemographicMajor.CDM:
        //        case DemographicMajor.CSM:
        //        case DemographicMajor.DEC:
        //        case DemographicMajor.ECO:
        //        case DemographicMajor.EDS:
        //        case DemographicMajor.EED:
        //        case DemographicMajor.ENG:
        //        case DemographicMajor.ENGE:
        //        case DemographicMajor.EXS:
        //        case DemographicMajor.MPF:
        //        case DemographicMajor.GEO:
        //        case DemographicMajor.HPEE:
        //        case DemographicMajor.HIS:
        //        case DemographicMajor.ITB:
        //        case DemographicMajor.ITS:
        //        case DemographicMajor.MGT:
        //        case DemographicMajor.MKT:
        //        case DemographicMajor.MAT:
        //        case DemographicMajor.MAED:
        //        case DemographicMajor.MWR:
        //        case DemographicMajor.MUS:
        //        case DemographicMajor.PHI:
        //        case DemographicMajor.PHY:
        //        case DemographicMajor.PHYE:
        //        case DemographicMajor.PHME:
        //        case DemographicMajor.PPE:
        //        case DemographicMajor.POS:
        //        case DemographicMajor.PSY:
        //        case DemographicMajor.PBH:
        //        case DemographicMajor.PBR:
        //        case DemographicMajor.SOSE:
        //        case DemographicMajor.SWK:
        //        case DemographicMajor.SOC:
        //        case DemographicMajor.SPA:
        //        case DemographicMajor.SPAE:
        //        case DemographicMajor.SMA:
        //        case DemographicMajor.CAT:
        //        case DemographicMajor.WBC:
        //            return DegreeType.BachelorArts;

        //        case DemographicMajor.MUSE:
        //            return DegreeType.BachelorMusic;

        //        case DemographicMajor.ACC:
        //        case DemographicMajor.ACCS:
        //        case DemographicMajor.ARTE:
        //        case DemographicMajor.ARTS:
        //        case DemographicMajor.BIBS:
        //        case DemographicMajor.BIO:
        //        case DemographicMajor.BIOE:
        //        case DemographicMajor.BIOS:
        //        case DemographicMajor.CHE:
        //        case DemographicMajor.CHEE:
        //        case DemographicMajor.CES:
        //        case DemographicMajor.CEMS:
        //        case DemographicMajor.CSC:
        //        case DemographicMajor.CDMS:
        //        case DemographicMajor.COSS:
        //        case DemographicMajor.DECS:
        //        case DemographicMajor.ECOS:
        //        case DemographicMajor.EDS:
        //        case DemographicMajor.EED:
        //        case DemographicMajor.EPH:
        //        case DemographicMajor.ENGE:
        //        case DemographicMajor.ENGS:
        //        case DemographicMajor.EEN:
        //        case DemographicMajor.ENS:
        //        case DemographicMajor.EXS:
        //        case DemographicMajor.MPFS:
        //        case DemographicMajor.FIN:
        //        case DemographicMajor.HPEE:
        //        case DemographicMajor.HISS:
        //        case DemographicMajor.ITBS:
        //        case DemographicMajor.ITSS:
        //        case DemographicMajor.MGTS:
        //        case DemographicMajor.MKTS:
        //        case DemographicMajor.MAED:
        //        case DemographicMajor.MAI:
        //        case DemographicMajor.MATS:
        //        case DemographicMajor.MWRS:
        //        case DemographicMajor.MUS:
        //        case DemographicMajor.PHIS:
        //        case DemographicMajor.PHY:
        //        case DemographicMajor.PHYE:
        //        case DemographicMajor.PHME:
        //        case DemographicMajor.PPES:
        //        case DemographicMajor.POSS:
        //        case DemographicMajor.PWR:
        //        case DemographicMajor.PSYS:
        //        case DemographicMajor.PBH:
        //        case DemographicMajor.PBRS:
        //        case DemographicMajor.SOSE:
        //        case DemographicMajor.SWK:
        //        case DemographicMajor.SOCS:
        //        case DemographicMajor.SPAE:
        //        case DemographicMajor.SPAS:
        //        case DemographicMajor.SMA:
        //        case DemographicMajor.SEN:
        //        case DemographicMajor.CATS:
        //        case DemographicMajor.WBCS:
        //            return DegreeType.BachelorScience;

        //        case DemographicMajor.GENS:
        //            return DegreeType.MasterEnvironmentalScience;

        //        case DemographicMajor.GHES:
        //            return DegreeType.MasterArts;

        //        case DemographicMajor.BBA:
        //        case DemographicMajor.BBS:
        //        case DemographicMajor.BUA:
        //        case DemographicMajor.CASS:
        //        case DemographicMajor.CED:
        //        case DemographicMajor.CEN:
        //        case DemographicMajor.CHES:
        //        case DemographicMajor.CSMS:
        //        case DemographicMajor.ECE:
        //        case DemographicMajor.FRE:
        //        case DemographicMajor.ING:
        //        case DemographicMajor.JMW:
        //        case DemographicMajor.MCM:
        //        case DemographicMajor.MCMS:
        //        case DemographicMajor.MPR:
        //        case DemographicMajor.MPRS:
        //        case DemographicMajor.MUCP:
        //        case DemographicMajor.MUPF:
        //        case DemographicMajor.PHYS:
        //        case DemographicMajor.PRE:
        //        case DemographicMajor.PREP:
        //        case DemographicMajor.PSCE:
        //        case DemographicMajor.SMAS:
        //        case DemographicMajor.TTE:
        //        case DemographicMajor.TTS:
        //        case DemographicMajor.UBUS:
        //        case DemographicMajor.UEDU:
        //        case DemographicMajor.UMCM:
        //        case DemographicMajor.VAR:
        //        case DemographicMajor.VARS:
        //            return false;
        //    }
        //}

        // It may be faster/easier to use the default: case for the majority case for the following functions, but
        // we explicitly list them out to force us to deal with new cases as they are added (if we don't then a
        // NotImplementedException will be thrown).

        public static bool GetIsSystems(DemographicMajor major)
        {
            switch (major)
            {
                case DemographicMajor.ACCS:
                case DemographicMajor.ARTS:
                case DemographicMajor.BIBS:
                case DemographicMajor.BIOS:
                case DemographicMajor.BUAS:
                case DemographicMajor.CAMS:
                case DemographicMajor.CASS:
                case DemographicMajor.CATS:
                case DemographicMajor.CDMS:
                case DemographicMajor.CEDS:
                case DemographicMajor.CEMS:
                case DemographicMajor.CENS:
                case DemographicMajor.CESS:
                case DemographicMajor.CGAS:
                case DemographicMajor.CHES:
                case DemographicMajor.CISS:
                case DemographicMajor.CMJS:
                case DemographicMajor.CMMM:
                case DemographicMajor.CNMS:
                case DemographicMajor.COSS:
                case DemographicMajor.CRJS:
                case DemographicMajor.CSMS:
                case DemographicMajor.DECS:
                case DemographicMajor.ECOS:
                case DemographicMajor.ENBS:
                case DemographicMajor.ENGS:
                case DemographicMajor.ENMS:
                case DemographicMajor.EPHS:
                case DemographicMajor.GATS:
                case DemographicMajor.HISS:
                case DemographicMajor.HPRS:
                case DemographicMajor.INGS:
                case DemographicMajor.ITBS:
                case DemographicMajor.ITSS:
                case DemographicMajor.MATS:
                case DemographicMajor.MCMS:
                case DemographicMajor.MESS:
                case DemographicMajor.MGTS:
                case DemographicMajor.MIS:
                case DemographicMajor.MKTS:
                case DemographicMajor.MPFS:
                case DemographicMajor.MPRS:
                case DemographicMajor.MUSS:
                case DemographicMajor.MWRS:
                case DemographicMajor.NASS:
                case DemographicMajor.PBRS:
                case DemographicMajor.PHIS:
                case DemographicMajor.PHYS:
                case DemographicMajor.POSS:
                case DemographicMajor.PPES:
                case DemographicMajor.PSYS:
                case DemographicMajor.PWRS:
                case DemographicMajor.SMAS:
                case DemographicMajor.SOCS:
                case DemographicMajor.SPAS:
                case DemographicMajor.SWKS:
                case DemographicMajor.VAMS:
                case DemographicMajor.VARS:
                case DemographicMajor.WBCS:
                case DemographicMajor.YMIS:
                    return true;

                case DemographicMajor._0000:
                case DemographicMajor.AAC:
                case DemographicMajor.ABA:
                case DemographicMajor.ACC:
                case DemographicMajor.AIA:
                case DemographicMajor.AMU:
                case DemographicMajor.ART:
                case DemographicMajor.ARTE:
                case DemographicMajor.ATR:
                case DemographicMajor.BBA:
                case DemographicMajor.BBS:
                case DemographicMajor.BCH:
                case DemographicMajor.BIA:
                case DemographicMajor.BIB:
                case DemographicMajor.BIO:
                case DemographicMajor.BIOE:
                case DemographicMajor.BIOM:
                case DemographicMajor.BUA:
                case DemographicMajor.CAE:
                case DemographicMajor.CAM:
                case DemographicMajor.CAS:
                case DemographicMajor.CASN:
                case DemographicMajor.CAT:
                case DemographicMajor.CDM:
                case DemographicMajor.CED:
                case DemographicMajor.CEM:
                case DemographicMajor.CEN:
                case DemographicMajor.CES:
                case DemographicMajor.CGA:
                case DemographicMajor.CHE:
                case DemographicMajor.CHEE:
                case DemographicMajor.CIA:
                case DemographicMajor.CIS:
                case DemographicMajor.CMCC:
                case DemographicMajor.CMCE:
                case DemographicMajor.CMI:
                case DemographicMajor.CMJ:
                case DemographicMajor.CMPM:
                case DemographicMajor.CNM:
                case DemographicMajor.COS:
                case DemographicMajor.CRJ:
                case DemographicMajor.CSC:
                case DemographicMajor.CSG:
                case DemographicMajor.CSM:
                case DemographicMajor.DEC:
                case DemographicMajor.ECE:
                case DemographicMajor.ECO:
                case DemographicMajor.EDS:
                case DemographicMajor.EED:
                case DemographicMajor.EEN:
                case DemographicMajor.EGE:
                case DemographicMajor.ENB:
                case DemographicMajor.ENG:
                case DemographicMajor.ENGE:
                case DemographicMajor.ENGL:
                case DemographicMajor.ENGW:
                case DemographicMajor.ENM:
                case DemographicMajor.ENR:
                case DemographicMajor.ENS:
                case DemographicMajor.EPH:
                case DemographicMajor.EST:
                case DemographicMajor.EXS:
                case DemographicMajor.FIN:
                case DemographicMajor.FMP:
                case DemographicMajor.FRE:
                case DemographicMajor.FREE:
                case DemographicMajor.GAT:
                case DemographicMajor.GBUA:
                case DemographicMajor.GENS:
                case DemographicMajor.GEO:
                case DemographicMajor.GHES:
                case DemographicMajor.GRST:
                case DemographicMajor.HCM:
                case DemographicMajor.HES:
                case DemographicMajor.HIS:
                case DemographicMajor.HISN:
                case DemographicMajor.HMS:
                case DemographicMajor.HPEE:
                case DemographicMajor.HPR:
                case DemographicMajor.HPRE:
                case DemographicMajor.ICS:
                case DemographicMajor.IDS:
                case DemographicMajor.ING:
                case DemographicMajor.ITB:
                case DemographicMajor.ITS:
                case DemographicMajor.JAD:
                case DemographicMajor.JMI:
                case DemographicMajor.JMW:
                case DemographicMajor.JRN:
                case DemographicMajor.LBA:
                case DemographicMajor.LJU:
                case DemographicMajor.MAED:
                case DemographicMajor.MAI:
                case DemographicMajor.MAT:
                case DemographicMajor.MATE:
                case DemographicMajor.MCM:
                case DemographicMajor.MES:
                case DemographicMajor.MGT:
                case DemographicMajor.MKC:
                case DemographicMajor.MKT:
                case DemographicMajor.MPF:
                case DemographicMajor.MPR:
                case DemographicMajor.MTR:
                case DemographicMajor.MUCP:
                case DemographicMajor.MUPF:
                case DemographicMajor.MUS:
                case DemographicMajor.MUSE:
                case DemographicMajor.MWR:
                case DemographicMajor.NAS:
                case DemographicMajor.OMG:
                case DemographicMajor.PAT:
                case DemographicMajor.PBH:
                case DemographicMajor.PBR:
                case DemographicMajor.PESL:
                case DemographicMajor.PHEP:
                case DemographicMajor.PHI:
                case DemographicMajor.PHME:
                case DemographicMajor.PHY:
                case DemographicMajor.PHYE:
                case DemographicMajor.PHYN:
                case DemographicMajor.PMI:
                case DemographicMajor.PMIN:
                case DemographicMajor.POS:
                case DemographicMajor.PPE:
                case DemographicMajor.PRE:
                case DemographicMajor.PREP:
                case DemographicMajor.PSCE:
                case DemographicMajor.PSY:
                case DemographicMajor.PSYN:
                case DemographicMajor.PWR:
                case DemographicMajor.RES:
                case DemographicMajor.RLE:
                case DemographicMajor.SAR:
                case DemographicMajor.SEN:
                case DemographicMajor.SMA:
                case DemographicMajor.SOC:
                case DemographicMajor.SOS:
                case DemographicMajor.SOSE:
                case DemographicMajor.SPA:
                case DemographicMajor.SPAE:
                case DemographicMajor.SSGE:
                case DemographicMajor.SSGO:
                case DemographicMajor.SSPS:
                case DemographicMajor.SSSO:
                case DemographicMajor.SSUS:
                case DemographicMajor.SSWC:
                case DemographicMajor.SUS:
                case DemographicMajor.SWK:
                case DemographicMajor.THR:
                case DemographicMajor.TMT:
                case DemographicMajor.TSED:
                case DemographicMajor.TTE:
                case DemographicMajor.TTS:
                case DemographicMajor.UBUS:
                case DemographicMajor.UEDU:
                case DemographicMajor.UMCM:
                case DemographicMajor.UMI:
                case DemographicMajor.UMIN:
                case DemographicMajor.VAM:
                case DemographicMajor.VAR:
                case DemographicMajor.VMI:
                case DemographicMajor.WBC:
                case DemographicMajor.WEL:
                case DemographicMajor.YBIB:
                case DemographicMajor.YMI:
                    return false;
            }

            throw new NotImplementedException(major.ToString());
        }

        public static bool GetIsEducation(DemographicMajor major)
        {
            switch (major)
            {
                case DemographicMajor.ARTE:
                case DemographicMajor.BIOE:
                case DemographicMajor.CAE:
                case DemographicMajor.CED:
                case DemographicMajor.CEM:
                case DemographicMajor.CEMS:
                case DemographicMajor.CHEE:
                case DemographicMajor.ECE:
                case DemographicMajor.EED:
                case DemographicMajor.ENGE:
                case DemographicMajor.FREE:
                case DemographicMajor.GHES:
                case DemographicMajor.HES:
                case DemographicMajor.HPEE:
                case DemographicMajor.HPR:
                case DemographicMajor.HPRE:
                case DemographicMajor.HPRS:
                case DemographicMajor.MAED:
                case DemographicMajor.MATE:
                case DemographicMajor.MUSE:
                case DemographicMajor.PHME:
                case DemographicMajor.PHYE:
                case DemographicMajor.PSCE:
                case DemographicMajor.SOSE:
                case DemographicMajor.SPAE:
                case DemographicMajor.TSED:
                case DemographicMajor.UEDU:
                    return true;

                case DemographicMajor.ACC:
                case DemographicMajor.ACCS:
                case DemographicMajor.ART:
                case DemographicMajor.ARTS:
                case DemographicMajor.BBA:
                case DemographicMajor.BBS:
                case DemographicMajor.BCH:
                case DemographicMajor.BIB:
                case DemographicMajor.BIBS:
                case DemographicMajor.BIO:
                case DemographicMajor.BIOS:
                case DemographicMajor.BUA:
                case DemographicMajor.CAS:
                case DemographicMajor.CASS:
                case DemographicMajor.CAT:
                case DemographicMajor.CATS:
                case DemographicMajor.CDM:
                case DemographicMajor.CDMS:
                case DemographicMajor.CEN:
                case DemographicMajor.CHE:
                case DemographicMajor.CES:
                case DemographicMajor.CHES:
                case DemographicMajor.CMI:
                case DemographicMajor.COS:
                case DemographicMajor.COSS:
                case DemographicMajor.CSC:
                case DemographicMajor.CSM:
                case DemographicMajor.CSMS:
                case DemographicMajor.DEC:
                case DemographicMajor.DECS:
                case DemographicMajor.ECO:
                case DemographicMajor.ECOS:
                case DemographicMajor.EDS:
                case DemographicMajor.EEN:
                case DemographicMajor.ENG:
                case DemographicMajor.ENGS:
                case DemographicMajor.ENS:
                case DemographicMajor.EPH:
                case DemographicMajor.EXS:
                case DemographicMajor.FIN:
                case DemographicMajor.FRE:
                case DemographicMajor.GAT:
                case DemographicMajor.GBUA:
                case DemographicMajor.GENS:
                case DemographicMajor.GEO:
                case DemographicMajor.HIS:
                case DemographicMajor.HISS:
                case DemographicMajor.ING:
                case DemographicMajor.ITB:
                case DemographicMajor.ITBS:
                case DemographicMajor.ITS:
                case DemographicMajor.ITSS:
                case DemographicMajor.JMW:
                case DemographicMajor.LBA:
                case DemographicMajor.MAI:
                case DemographicMajor.MAT:
                case DemographicMajor.MATS:
                case DemographicMajor.MCM:
                case DemographicMajor.MCMS:
                case DemographicMajor.MGT:
                case DemographicMajor.MGTS:
                case DemographicMajor.MKT:
                case DemographicMajor.MKTS:
                case DemographicMajor.MPF:
                case DemographicMajor.MPFS:
                case DemographicMajor.MPR:
                case DemographicMajor.MPRS:
                case DemographicMajor.MUCP:
                case DemographicMajor.MUPF:
                case DemographicMajor.MUS:
                case DemographicMajor.MWR:
                case DemographicMajor.MWRS:
                case DemographicMajor.PAT:
                case DemographicMajor.PBH:
                case DemographicMajor.PBR:
                case DemographicMajor.PBRS:
                case DemographicMajor.PESL:
                case DemographicMajor.PHI:
                case DemographicMajor.PHIS:
                case DemographicMajor.PHY:
                case DemographicMajor.PHYS:
                case DemographicMajor.POS:
                case DemographicMajor.POSS:
                case DemographicMajor.PPE:
                case DemographicMajor.PPES:
                case DemographicMajor.PRE:
                case DemographicMajor.PREP:
                case DemographicMajor.PSY:
                case DemographicMajor.PSYS:
                case DemographicMajor.PWR:
                case DemographicMajor.SEN:
                case DemographicMajor.SUS:
                case DemographicMajor.SMA:
                case DemographicMajor.SMAS:
                case DemographicMajor.SOC:
                case DemographicMajor.SOCS:
                case DemographicMajor.SPA:
                case DemographicMajor.SPAS:
                case DemographicMajor.SWK:
                case DemographicMajor.THR:
                case DemographicMajor.TTE:
                case DemographicMajor.TTS:
                case DemographicMajor.UBUS:
                case DemographicMajor.UMCM:
                case DemographicMajor.VAR:
                case DemographicMajor.VARS:
                case DemographicMajor.WBC:
                case DemographicMajor.WBCS:
                case DemographicMajor._0000:
                case DemographicMajor.AAC:
                case DemographicMajor.ABA:
                case DemographicMajor.AIA:
                case DemographicMajor.AMU:
                case DemographicMajor.ATR:
                case DemographicMajor.BIA:
                case DemographicMajor.BIOM:
                case DemographicMajor.BUAS:
                case DemographicMajor.CAM:
                case DemographicMajor.CAMS:
                case DemographicMajor.CASN:
                case DemographicMajor.CEDS:
                case DemographicMajor.CENS:
                case DemographicMajor.CESS:
                case DemographicMajor.CGA:
                case DemographicMajor.CGAS:
                case DemographicMajor.CIA:
                case DemographicMajor.CIS:
                case DemographicMajor.CISS:
                case DemographicMajor.CMCC:
                case DemographicMajor.CMCE:
                case DemographicMajor.CMJ:
                case DemographicMajor.CMJS:
                case DemographicMajor.CMMM:
                case DemographicMajor.CMPM:
                case DemographicMajor.CNM:
                case DemographicMajor.CNMS:
                case DemographicMajor.CRJ:
                case DemographicMajor.CRJS:
                case DemographicMajor.CSG:
                case DemographicMajor.EGE:
                case DemographicMajor.ENB:
                case DemographicMajor.ENBS:
                case DemographicMajor.ENGL:
                case DemographicMajor.ENGW:
                case DemographicMajor.ENM:
                case DemographicMajor.ENMS:
                case DemographicMajor.ENR:
                case DemographicMajor.EPHS:
                case DemographicMajor.EST:
                case DemographicMajor.FMP:
                case DemographicMajor.GATS:
                case DemographicMajor.GRST:
                case DemographicMajor.HCM:
                case DemographicMajor.HISN:
                case DemographicMajor.HMS:
                case DemographicMajor.ICS:
                case DemographicMajor.IDS:
                case DemographicMajor.INGS:
                case DemographicMajor.JAD:
                case DemographicMajor.JMI:
                case DemographicMajor.JRN:
                case DemographicMajor.LJU:
                case DemographicMajor.MES:
                case DemographicMajor.MESS:
                case DemographicMajor.MIS:
                case DemographicMajor.MKC:
                case DemographicMajor.MTR:
                case DemographicMajor.MUSS:
                case DemographicMajor.NAS:
                case DemographicMajor.NASS:
                case DemographicMajor.OMG:
                case DemographicMajor.PHEP:
                case DemographicMajor.PHYN:
                case DemographicMajor.PMI:
                case DemographicMajor.PMIN:
                case DemographicMajor.PSYN:
                case DemographicMajor.PWRS:
                case DemographicMajor.RES:
                case DemographicMajor.RLE:
                case DemographicMajor.SAR:
                case DemographicMajor.SOS:
                case DemographicMajor.SSGE:
                case DemographicMajor.SSGO:
                case DemographicMajor.SSPS:
                case DemographicMajor.SSSO:
                case DemographicMajor.SSUS:
                case DemographicMajor.SSWC:
                case DemographicMajor.SWKS:
                case DemographicMajor.TMT:
                case DemographicMajor.UMI:
                case DemographicMajor.UMIN:
                case DemographicMajor.VAM:
                case DemographicMajor.VAMS:
                case DemographicMajor.VMI:
                case DemographicMajor.WEL:
                case DemographicMajor.YBIB:
                case DemographicMajor.YMI:
                case DemographicMajor.YMIS:
                    return false;
            }

            throw new NotImplementedException(major.ToString());
        }

        public static bool GetIsGraduate(DemographicMajor major)
        {
            switch (major)
            {
                case DemographicMajor.GBUA:
                case DemographicMajor.GENS:
                case DemographicMajor.GHES:
                case DemographicMajor.GRST:
                    return true;

                case DemographicMajor.ACCS:
                case DemographicMajor.ARTS:
                case DemographicMajor.BIBS:
                case DemographicMajor.BIOS:
                case DemographicMajor.BUAS:
                case DemographicMajor.CAMS:
                case DemographicMajor.CASS:
                case DemographicMajor.CATS:
                case DemographicMajor.CDMS:
                case DemographicMajor.CEDS:
                case DemographicMajor.CEMS:
                case DemographicMajor.CENS:
                case DemographicMajor.CESS:
                case DemographicMajor.CGAS:
                case DemographicMajor.CHES:
                case DemographicMajor.CISS:
                case DemographicMajor.CMJS:
                case DemographicMajor.CMMM:
                case DemographicMajor.CNMS:
                case DemographicMajor.COSS:
                case DemographicMajor.CRJS:
                case DemographicMajor.CSMS:
                case DemographicMajor.DECS:
                case DemographicMajor.ECOS:
                case DemographicMajor.ENBS:
                case DemographicMajor.ENGS:
                case DemographicMajor.ENMS:
                case DemographicMajor.EPHS:
                case DemographicMajor.GATS:
                case DemographicMajor.HISS:
                case DemographicMajor.HPRS:
                case DemographicMajor.INGS:
                case DemographicMajor.ITBS:
                case DemographicMajor.ITSS:
                case DemographicMajor.MATS:
                case DemographicMajor.MCMS:
                case DemographicMajor.MESS:
                case DemographicMajor.MGTS:
                case DemographicMajor.MIS:
                case DemographicMajor.MKTS:
                case DemographicMajor.MPFS:
                case DemographicMajor.MPRS:
                case DemographicMajor.MUSS:
                case DemographicMajor.MWRS:
                case DemographicMajor.NASS:
                case DemographicMajor.PBRS:
                case DemographicMajor.PHIS:
                case DemographicMajor.PHYS:
                case DemographicMajor.POSS:
                case DemographicMajor.PPES:
                case DemographicMajor.PSYS:
                case DemographicMajor.PWRS:
                case DemographicMajor.SMAS:
                case DemographicMajor.SOCS:
                case DemographicMajor.SPAS:
                case DemographicMajor.SWKS:
                case DemographicMajor.VAMS:
                case DemographicMajor.VARS:
                case DemographicMajor.WBCS:
                case DemographicMajor.YMIS:
                case DemographicMajor._0000:
                case DemographicMajor.AAC:
                case DemographicMajor.ABA:
                case DemographicMajor.ACC:
                case DemographicMajor.AIA:
                case DemographicMajor.AMU:
                case DemographicMajor.ART:
                case DemographicMajor.ARTE:
                case DemographicMajor.ATR:
                case DemographicMajor.BBA:
                case DemographicMajor.BBS:
                case DemographicMajor.BCH:
                case DemographicMajor.BIA:
                case DemographicMajor.BIB:
                case DemographicMajor.BIO:
                case DemographicMajor.BIOE:
                case DemographicMajor.BIOM:
                case DemographicMajor.BUA:
                case DemographicMajor.CAE:
                case DemographicMajor.CAM:
                case DemographicMajor.CAS:
                case DemographicMajor.CASN:
                case DemographicMajor.CAT:
                case DemographicMajor.CDM:
                case DemographicMajor.CED:
                case DemographicMajor.CEM:
                case DemographicMajor.CEN:
                case DemographicMajor.CES:
                case DemographicMajor.CGA:
                case DemographicMajor.CHE:
                case DemographicMajor.CHEE:
                case DemographicMajor.CIA:
                case DemographicMajor.CIS:
                case DemographicMajor.CMCC:
                case DemographicMajor.CMCE:
                case DemographicMajor.CMI:
                case DemographicMajor.CMJ:
                case DemographicMajor.CMPM:
                case DemographicMajor.CNM:
                case DemographicMajor.COS:
                case DemographicMajor.CRJ:
                case DemographicMajor.CSC:
                case DemographicMajor.CSG:
                case DemographicMajor.CSM:
                case DemographicMajor.DEC:
                case DemographicMajor.ECE:
                case DemographicMajor.ECO:
                case DemographicMajor.EDS:
                case DemographicMajor.EED:
                case DemographicMajor.EEN:
                case DemographicMajor.EGE:
                case DemographicMajor.ENB:
                case DemographicMajor.ENG:
                case DemographicMajor.ENGE:
                case DemographicMajor.ENGL:
                case DemographicMajor.ENGW:
                case DemographicMajor.ENM:
                case DemographicMajor.ENR:
                case DemographicMajor.ENS:
                case DemographicMajor.EPH:
                case DemographicMajor.EST:
                case DemographicMajor.EXS:
                case DemographicMajor.FIN:
                case DemographicMajor.FMP:
                case DemographicMajor.FRE:
                case DemographicMajor.FREE:
                case DemographicMajor.GAT:
                case DemographicMajor.GEO:
                case DemographicMajor.HCM:
                case DemographicMajor.HES:
                case DemographicMajor.HIS:
                case DemographicMajor.HISN:
                case DemographicMajor.HMS:
                case DemographicMajor.HPEE:
                case DemographicMajor.HPR:
                case DemographicMajor.HPRE:
                case DemographicMajor.ICS:
                case DemographicMajor.IDS:
                case DemographicMajor.ING:
                case DemographicMajor.ITB:
                case DemographicMajor.ITS:
                case DemographicMajor.JAD:
                case DemographicMajor.JMI:
                case DemographicMajor.JMW:
                case DemographicMajor.JRN:
                case DemographicMajor.LBA:
                case DemographicMajor.LJU:
                case DemographicMajor.MAED:
                case DemographicMajor.MAI:
                case DemographicMajor.MAT:
                case DemographicMajor.MATE:
                case DemographicMajor.MCM:
                case DemographicMajor.MES:
                case DemographicMajor.MGT:
                case DemographicMajor.MKC:
                case DemographicMajor.MKT:
                case DemographicMajor.MPF:
                case DemographicMajor.MPR:
                case DemographicMajor.MTR:
                case DemographicMajor.MUCP:
                case DemographicMajor.MUPF:
                case DemographicMajor.MUS:
                case DemographicMajor.MUSE:
                case DemographicMajor.MWR:
                case DemographicMajor.NAS:
                case DemographicMajor.OMG:
                case DemographicMajor.PAT:
                case DemographicMajor.PBH:
                case DemographicMajor.PBR:
                case DemographicMajor.PESL:
                case DemographicMajor.PHEP:
                case DemographicMajor.PHI:
                case DemographicMajor.PHME:
                case DemographicMajor.PHY:
                case DemographicMajor.PHYE:
                case DemographicMajor.PHYN:
                case DemographicMajor.PMI:
                case DemographicMajor.PMIN:
                case DemographicMajor.POS:
                case DemographicMajor.PPE:
                case DemographicMajor.PRE:
                case DemographicMajor.PREP:
                case DemographicMajor.PSCE:
                case DemographicMajor.PSY:
                case DemographicMajor.PSYN:
                case DemographicMajor.PWR:
                case DemographicMajor.RES:
                case DemographicMajor.RLE:
                case DemographicMajor.SAR:
                case DemographicMajor.SEN:
                case DemographicMajor.SMA:
                case DemographicMajor.SOC:
                case DemographicMajor.SOS:
                case DemographicMajor.SOSE:
                case DemographicMajor.SPA:
                case DemographicMajor.SPAE:
                case DemographicMajor.SSGE:
                case DemographicMajor.SSGO:
                case DemographicMajor.SSPS:
                case DemographicMajor.SSSO:
                case DemographicMajor.SSUS:
                case DemographicMajor.SSWC:
                case DemographicMajor.SUS:
                case DemographicMajor.SWK:
                case DemographicMajor.THR:
                case DemographicMajor.TMT:
                case DemographicMajor.TSED:
                case DemographicMajor.TTE:
                case DemographicMajor.TTS:
                case DemographicMajor.UBUS:
                case DemographicMajor.UEDU:
                case DemographicMajor.UMCM:
                case DemographicMajor.UMI:
                case DemographicMajor.UMIN:
                case DemographicMajor.VAM:
                case DemographicMajor.VAR:
                case DemographicMajor.VMI:
                case DemographicMajor.WBC:
                case DemographicMajor.WEL:
                case DemographicMajor.YBIB:
                case DemographicMajor.YMI:
                    return false;
            }

            throw new NotImplementedException(major.ToString());
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public override void Execute(DateTime runDate, Action<String> logMessage, CancellationToken cancellationToken)
        {
            using (IDirectoryRepository source = RepositoryFactory.CreateDirectoryRepository(_directoryArgs))
            {
                logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                List<String> modified = new List<String>();
                Int32 newCount = 0;

                using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(_harvesterArgs))
                {
                    logMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                    IEnumerable<DirectoryObjectMetadata> directoryFiles = source.ListFiles("/");
                    Dictionary<String, DirectoryRecord> dictionary = harvester.DataContext.DirectoryRecords.Where(d => d.Operation.Name == Name && d.Repository.Name == source.Name).ToDictionary(d => d.FilePath);

                    Entities.Repository repository = harvester.DataContext.Repositories.First(x => x.Name == source.Name);

                    if (OperationID == 0)
                    {
                        logMessage("Warning: OperationID was not set properly. Correcting this.");
                        OperationID = harvester.DataContext.Operations.First(d => d.Name == Name).ID;
                    }

                    foreach (DirectoryObjectMetadata file in directoryFiles)
                    {
                        if (!dictionary.ContainsKey(file.Path))
                        {
                            modified.Add(file.Name);
                            newCount++;

                            harvester.DataContext.DirectoryRecords.InsertOnSubmit(new DirectoryRecord
                            {
                                OperationID = OperationID,
                                RepositoryID = repository.ID,
                                FilePath = file.Path,
                                FileModifiedDate = file.ModifiedDate,
                                CreationDate = DateTime.Now,
                                ModifiedDate = DateTime.Now
                            });
                        }
                        else
                        {
                            DirectoryRecord element = dictionary[file.Path];

                            if (file.ModifiedDate > element.FileModifiedDate)
                            {
                                modified.Add(file.Name);
                                element.FileModifiedDate = file.ModifiedDate;
                                element.ModifiedDate = DateTime.Now;
                            }
                        }
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        source.Dispose();
                        harvester.Dispose();
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    harvester.DataContext.SubmitChanges();
                }

                logMessage($"Discovered {modified.Count} files to be processed ({newCount} new and {modified.Count - newCount} updated).");

                if (modified.Count == 0)
                    return;

                using (IDatabaseRepository<IStatisticsDataContext> destination = RepositoryFactory.CreateStatisticsRepository(_statisticsArgs))
                {
                    logMessage($"Connected to database '{destination.Name}' ({destination.ConnectionString})");

                    StreamParser<DemographicRecord> Parser = new StreamParser<DemographicRecord>();

                    var records = modified.Select(file =>
                    {
                        logMessage($"Processing '{file}':");

                        Tuple<DateTime, DateTime> dateRange = GetDateRange(file);

                        Stream inputStream = source.OpenFile(file);

                        var counter = CountingEnumerable.Wrap(Parser.ParseStream(inputStream).Select(r => new { DateRange = dateRange, Record = r }));

                        return Tuple.Create(counter, (Action)(() =>
                        {
                            inputStream.Dispose();
                            logMessage($"\t{counter.Count} records processed.");
                        }));
                    }).SelectMany(i => i).Select((r, i) => new
                    {
                        ItemIndex = i,
                        Barcode = r.Record.Barcode,
                        Gender = GetGender(r.Record.Gender),
                        StudentLevel = GetStudentLevel(r.Record.Level),
                        StudentType = GetStudentType(r.Record.StudentType),
                        StudentClass = GetStudentClass(r.Record.Class),
                        StartDate = r.DateRange.Item1,
                        EndDate = r.DateRange.Item2,
                        Gpa = r.Record.Gpa,
                        Residence = r.Record.Residence.Bind(a => new { ResidentName = GetResidenceName(a), ResidenceCategory = GetResidenceCategory(a) }),
                        Majors = r.Record.Major1.Bind(m1 => r.Record.Major2.Bind(m2 => new[] { m1, m2 }, new[] { m1 })).Select((m, j) => new
                        {
                            MajorName = GetMajorName(m),
                            MajorCode = m,
                            MajorType = j == 0 ? MajorType.Major1 : MajorType.Major2,
                            IsSystems = GetIsSystems(m),
                            IsEducation = GetIsEducation(m),
                            IsGraduate = GetIsGraduate(m)
                        })
                    });

                    try
                    {
                        destination.DataContext.BulkImportDemographics(
                            records
                                .ToDataReader(r => new object[] { r.ItemIndex, null, null, null, r.StartDate, r.EndDate, r.Barcode, r.Gender, r.StudentLevel, r.StudentType, r.StudentClass, r.Gpa }),
                            records.SelectMany(r => r.Majors, (r, m) => new { Index = r.ItemIndex, Major = m })
                                .ToDataReader(r => new object[] { r.Index, r.Major.MajorName, r.Major.MajorCode, r.Major.MajorType, r.Major.IsSystems, r.Major.IsEducation, r.Major.IsGraduate }),
                            records.Where(r => r.Residence != null)
                                .ToDataReader(r => new object[] { r.ItemIndex, r.Residence.ResidentName, r.Residence.ResidenceCategory }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                    finally
                    {
                        destination.DataContext.Connection.Close();
                        destination.DataContext.Dispose();
                    }
                }
            }
        }

        private Tuple<DateTime, DateTime> GetDateRange(string fileName)
        {
            Int32 year = int.Parse(fileName.Substring(0, 4));
            Char num = fileName[4];
            switch (num)
            {
                case '1':
                    // Interterm
                    return Tuple.Create(new DateTime(year, 1, 1), new DateTime(year, 2, 1));
                case '2':
                    // Spring Term
                    return Tuple.Create(new DateTime(year, 2, 1), new DateTime(year, 6, 1));
                case '5':
                    // Summer Term
                    return Tuple.Create(new DateTime(year, 6, 1), new DateTime(year, 9, 1));
                case '9':
                    // Fall Term
                    return Tuple.Create(new DateTime(year, 9, 1), new DateTime(year + 1, 1, 1));
            }

            throw new NotImplementedException();
        }
    }
}
