namespace Pantheon.Banking.Service
{
    public interface IMapper
    {
        // Unique key to identify mapper across the framework
        string Key { get; }

        /// <summary>
        /// Converts object of type T to an object of type K
        /// </summary>
        /// <typeparam name="T">Source object type</typeparam>
        /// <typeparam name="K">Target object type</typeparam>               
        K Map<T, K>(T dto);
    }
}
