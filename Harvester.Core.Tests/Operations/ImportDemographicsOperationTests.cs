using System;
using System.Collections.Generic;
using Xunit;

using ZondervanLibrary.Harvester.Core.Operations;
using ZondervanLibrary.Harvester.Core.Operations.Demographics;

namespace ZondervanLibrary.Harvester.Core.Tests.Operations
{
    public class ImportDemographicsOperationTests
    {

        private void CheckInputEnumCoverage<TEnum, TResult>(Func<TEnum, TResult> function)
        {
            foreach (TEnum e in Enum.GetValues(typeof(TEnum)))
            {
                function(e);
            }
        }

        private void CheckOutputEnumCoverage<TEnum, TResult>(Func<TEnum, TResult> function)
        {
            HashSet<TResult> outputs = new HashSet<TResult>();

            foreach (TEnum e in Enum.GetValues(typeof(TEnum)))
            {
                outputs.Add(function(e));
            }

            Type resultType = typeof(TResult).IsEnum ? typeof(TResult) : typeof(TResult).GetGenericArguments()[0];

            foreach (TResult result in Enum.GetValues(resultType))
            {
                Assert.True(outputs.Contains(result), result.ToString());
            }
        }

        private void CheckInputNotInRange<TEnum, TResult>(Func<TEnum, TResult> function)
        {
            TEnum val = (TEnum)(object)Int32.MaxValue;

            Assert.Throws<NotImplementedException>(() => function(val));
        }

        #region GetGender

        [Fact]
        public void GetGender_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicGender g) => ImportDemographicsOperation.GetGender(g));
        }

        [Fact]
        public void GetGender_Should_Cover_All_Outputs()
        {
            CheckOutputEnumCoverage((DemographicGender g) => ImportDemographicsOperation.GetGender(g));
        }

        [Fact]
        public void GetGender_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicGender g) => ImportDemographicsOperation.GetGender(g));
        }

        #endregion

        #region GetStudentLevel

        [Fact]
        public void GetStudentLevel_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicLevel l) => ImportDemographicsOperation.GetStudentLevel(l));
        }

        [Fact]
        public void GetStudentLevel_Should_Cover_All_Outputs()
        {
            CheckOutputEnumCoverage((DemographicLevel l) => ImportDemographicsOperation.GetStudentLevel(l));
        }

        [Fact]
        public void GetStudentLevel_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicLevel l) => ImportDemographicsOperation.GetStudentLevel(l));
        }

        #endregion

        #region GetStudentType

        [Fact]
        public void GetStudentType_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicStudentType l) => ImportDemographicsOperation.GetStudentType(l));
        }

        [Fact]
        public void GetStudentType_Should_Cover_All_Outputs()
        {
            CheckOutputEnumCoverage((DemographicStudentType l) => ImportDemographicsOperation.GetStudentType(l));
        }

        [Fact]
        public void GetStudentType_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicStudentType l) => ImportDemographicsOperation.GetStudentType(l));
        }

        #endregion

        #region GetStudentClass

        [Fact]
        public void GetStudentClass_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicClass l) => ImportDemographicsOperation.GetStudentClass(l));
        }

        [Fact]
        public void GetStudentClass_Should_Cover_All_Outputs()
        {
            CheckOutputEnumCoverage((DemographicClass l) => ImportDemographicsOperation.GetStudentClass(l));
        }

        [Fact]
        public void GetStudentClass_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicClass l) => ImportDemographicsOperation.GetStudentClass(l));
        }

        #endregion

        #region GetResidenceName

        [Fact]
        public void GetResidenceName_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicResidence l) => ImportDemographicsOperation.GetResidenceName(l));
        }

        [Fact]
        public void GetResidenceName_Should_Cover_All_Outputs()
        {
            CheckOutputEnumCoverage((DemographicResidence l) => ImportDemographicsOperation.GetResidenceName(l));
        }

        [Fact]
        public void GetResidenceName_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicResidence l) => ImportDemographicsOperation.GetResidenceName(l));
        }

        #endregion

        #region GetResidenceCategory

        [Fact]
        public void GetResidenceCategory_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicResidence l) => ImportDemographicsOperation.GetResidenceCategory(l));
        }

        [Fact]
        public void GetResidenceCategory_Should_Cover_All_Outputs()
        {
            CheckOutputEnumCoverage((DemographicResidence l) => ImportDemographicsOperation.GetResidenceCategory(l));
        }

        [Fact]
        public void GetResidenceCategory_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicResidence l) => ImportDemographicsOperation.GetResidenceCategory(l));
        }

        #endregion

        #region GetMajorName

        [Fact]
        public void GetMajorName_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetMajorName(m));
        }

        [Fact]
        public void GetMajorName_Should_Cover_All_Outputs()
        {
            CheckOutputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetMajorName(m));
        }

        [Fact]
        public void GetMajorName_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicMajor m) => ImportDemographicsOperation.GetMajorName(m));
        }

        #endregion

        #region GetMajorBase

        //[Fact]
        //public void GetMajorBase_Should_Cover_All_Inputs()
        //{
        //    //Check if Demographic Major is handled completely by GetMajorBase
        //    CheckInputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetMajorBase(m));
        //}

        //[Fact]
        //public void GetMajorBase_Should_Cover_All_Outputs()
        //{
        //    CheckOutputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetMajorBase(m));
        //}

        //[Fact]
        //public void GetMajorBase_Should_Not_Cover_Unknown_Input()
        //{
        //    CheckInputNotInRange((DemographicMajor m) => ImportDemographicsOperation.GetMajorBase(m));
        //}

        #endregion

        /*#region GetDepartment

        [Fact]
        public void GetDepartment_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetDepartment(m));
        }

        [Fact]
        public void GetDepartment_Should_Cover_All_Outputs()
        {
            CheckOutputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetDepartment(m));
        }

        [Fact]
        public void GetDepartment_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicMajor m) => ImportDemographicsOperation.GetDepartment(m));
        }

        #endregion*/

        #region GetIsSystems

        [Fact]
        public void GetIsSystems_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetIsSystems(m));
        }

        [Fact]
        public void GetIsSystems_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicMajor m) => ImportDemographicsOperation.GetIsSystems(m));
        }

        #endregion

        #region GetIsEducation

        [Fact]
        public void GetIsEducation_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetIsEducation(m));
        }

        [Fact]
        public void GetIsEducation_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicMajor m) => ImportDemographicsOperation.GetIsEducation(m));
        }

        #endregion

        #region GetIsGraduate

        [Fact]
        public void GetIsGraduate_Should_Cover_All_Inputs()
        {
            CheckInputEnumCoverage((DemographicMajor m) => ImportDemographicsOperation.GetIsGraduate(m));
        }

        [Fact]
        public void GetIsGraduate_Should_Not_Cover_Unknown_Input()
        {
            CheckInputNotInRange((DemographicMajor m) => ImportDemographicsOperation.GetIsGraduate(m));
        }

        #endregion
    }
}
