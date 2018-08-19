namespace ZondervanLibrary.Harvester.Core.Repository.Directory
{
    /// <summary>
    /// Specifies behavior for creating a file.
    /// </summary>
    public enum FileCreationMode
    {
        /// <summary>
        /// Throw an exception if the file already exists.
        /// </summary>
        ThrowIfFileExists,

        /// <summary>
        /// Overwrite the destination file if it exists.
        /// </summary>
        Overwrite,

        /// <summary>
        /// Append to the destination file if it exists.
        /// </summary>
        Append
    }
}
