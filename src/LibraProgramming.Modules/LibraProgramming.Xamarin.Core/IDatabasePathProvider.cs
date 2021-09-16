namespace LibraProgramming.Xamarin.Core
{
    /// <summary>
    /// Platform-dependent Database path provider abstraction.
    /// </summary>
    public interface IDatabasePathProvider
    {
        /// <summary>
        /// Gets Database path for the platform.
        /// </summary>
        /// <param name="databaseName">Local database name.</param>
        /// <returns>The absolute path to file.</returns>
        string GetDatabasePath(string databaseName);
    }
}