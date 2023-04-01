namespace chldr_data.Services
{
    public class ServiceLocator
    {
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

        public void Register<T>(T service)
        {
            services[typeof(T)] = service;
        }

        public T GetService<T>()
        {
            return (T)services[typeof(T)];
        }
    }
}
