namespace chldr_android.Services
{
    public class ServiceLocator
    {
        private readonly IDictionary<Type, object> _services;

        public ServiceLocator()
        {
            _services = new Dictionary<Type, object>();
        }

        public void RegisterService<TService>(TService implementation) where TService : class
        {
            _services[typeof(TService)] = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }

        public TService GetService<TService>() where TService : class
        {
            if (_services.TryGetValue(typeof(TService), out var service))
            {
                return (TService)service;
            }

            throw new InvalidOperationException($"Service not registered: {typeof(TService)}");
        }
    }

}
