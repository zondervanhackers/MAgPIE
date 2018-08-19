using System;
using System.Collections.Generic;
using System.Linq;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.PatronTranslator.Console.Repository;

namespace ZondervanLibrary.PatronTranslator.Console.Patrons
{
    public class ImportPatronsOperation : IOperation
    {
        private readonly IRepository<Patron> _patrons;
        private readonly IRepository<Persona> _personas;
        private readonly IRepository<Persona> _changedPersonas;
        private readonly IConverter<Patron, Persona> _converter;
        private readonly IEnumerableDiff<Persona, Persona> _differentiator;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patrons"></param>
        /// <param name="existingPersonas"></param>
        /// <param name="changedPersonas"></param>
        public ImportPatronsOperation(IRepository<Patron> patrons, IRepository<Persona> existingPersonas, IRepository<Persona> changedPersonas, IConverter<Patron, Persona> converter, IEnumerableDiff<Persona, Persona> differentiator)
        {
            _patrons = patrons;
            _personas = existingPersonas;
            _changedPersonas = changedPersonas;
            _converter = converter;
            _differentiator = differentiator;
        }

        public void Execute(DateTime runDate, Action<Double, String> reportProgress)
        {
            reportProgress(0.0, "Converting ExLibris Patrons to OCLC Personas.");

            IEnumerable<Persona> newPersonas;

            try
            {
                newPersonas = _patrons.AsQueryable().Select(patron => _converter.Convert(patron));
            }
            catch (Exception ex)
            {
                reportProgress(0.0, $"An error was encountered when converting the Patrons source. {ex.Message}");
                return;
            }

            reportProgress(0.5, $"{newPersonas.Count()} records converted.");

            try
            {
                IEnumerable<Persona> workingSet = _personas.AsQueryable();

                if (workingSet.Any())
                {
                    throw new Exception();
                }

                IEnumerable<Persona> differentiatedResult = _differentiator.ComputeDiff(workingSet, newPersonas);

                reportProgress(0.5, $"{differentiatedResult.Count()} changes recorded.");

                _changedPersonas.InsertAllOnSubmit(differentiatedResult);
                _changedPersonas.SubmitChanges();
            }
            catch (Exception)
            {
                reportProgress(0.5, "No existing repository of personas found; no difference will be calculated.");
            }

            //_personas.DeleteAllOnSubmit();
            _personas.InsertAllOnSubmit(newPersonas);
            _personas.SubmitChanges();
        }
    }
}
