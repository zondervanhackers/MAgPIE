namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public enum CounterReport
    {
        /// <summary>
        /// Number of successful title requests by month and title.
        /// </summary>
        BR1,
        
        /// <summary>
        /// Number of successful section requests by month and title.
        /// </summary>
        BR2,

        /// <summary>
        /// Access denied to content items by month, title, and category.
        /// </summary>
        BR3,

        /// <summary>
        /// Total searches by month and title.
        /// </summary>
        BR5,

        /// <summary>
        /// Total searches, result clicks, and record views by month and database.
        /// </summary>
        DB1,

        /// <summary>
        /// Access denied by month, database, and category.
        /// </summary>
        DB2,

        /// <summary>
        /// Number of successful full-text article requests by month and journal.
        /// </summary>
        JR1,

        /// <summary>
        /// Access denied to full-text articles by month, journal, and category.
        /// </summary>
        JR2,

        /// <summary>
        /// Number of successful item reqeusts by month, journal, and page-type.
        /// </summary>
        JR3,

        /// <summary>
        /// Number of successful multimedia full content unit requests by month and collection.
        /// </summary>
        MR1,

        /// <summary>
        /// Number of successful multimedia full content unit requests by month, collection, and item type.
        /// </summary>
        MR2,

        /// <summary>
        /// Total searches, result clicks, and record views by month and platform.
        /// </summary>
        PR1,

        /// <summary>
        /// Searches, Federated Searches, Sessions, and Federated Sessions by Service.
        /// </summary>
        DB3,

        /// <summary>
        /// Total searches by month and database
        /// </summary>
        CR2,

        /// <summary>
        /// Total Searches Run By Month and Collection
        /// </summary>
        JR4
    }
}
