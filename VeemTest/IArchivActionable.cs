namespace VeemTest
{
    /// <summary>
    /// Archivable action
    /// </summary>
    public interface IArchivActionable
    {
        /// <summary>
        /// Start action
        /// </summary>
        void Start();

        /// <summary>
        /// stop action
        /// </summary>
        void Stop();

        /// <summary>
        /// Action result status
        /// </summary>
        /// <returns>0 - success  1 - faild</returns>
        int Result();
    }
}