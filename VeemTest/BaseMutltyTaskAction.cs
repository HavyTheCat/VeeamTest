
namespace VeemTest
{
    public abstract class BaseMutltyTaskAction
    {
        protected bool _cancelled = false;
        protected bool _success = false;

        /// <summary>
        /// Start of multytask 
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stop of multytask 
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Result of action
        /// </summary>
        public abstract int Result();

        /// <summary>
        /// Task 
        /// </summary>
        protected abstract void Action();

        
    }
}
