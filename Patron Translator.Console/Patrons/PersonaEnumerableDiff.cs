using System;
using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.PatronTranslator.Console.Patrons
{
    /// <summary>
    /// Provides an concrete implementation of <see cref="IEnumerableDiff"/> that allows different snapshots of Personas to be compared.
    /// </summary>
    public class PersonaEnumerableDiff : EnumerableDiffBase<Persona, Persona, String>
    {
        /// <inheritdoc/>
        protected override Func<Persona, String> GetKey
        {
            get { return p => p.wmsCircPatronInfo.barcode; }
        }

        /// <inheritdoc/>
        protected override Func<Persona, Persona> OnRecordAdded
        {
            get { return p => p; }
        }

        /// <inheritdoc/>
        protected override Func<Persona, Persona> OnRecordRemoved
        {
            get
            {
                return p =>
                {
                    p.oclcExpirationDate = DateTime.Now;
                    p.oclcExpirationDateSpecified = true;
                    return p;
                };
            }
        }

        /// <inheritdoc/>
        protected override Func<Persona, Persona, Persona> OnRecordChanged
        {
            get { return (oldRecord, newRecord) => newRecord; }
        }

        /// <inheritdoc/>
        protected override Func<Persona, Persona, Persona> OnRecordUnchanged => null;
    }
}
