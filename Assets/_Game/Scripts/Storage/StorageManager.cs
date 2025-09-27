namespace NothingAwaits.Storage
{
    public static class StorageManager
    {
        private static IStorage _storage;
        
        public static IStorage Storage
        {
            get => _storage ??= new PlayerPrefsStorage();
            private set => _storage = value;
        }
        
        public static void SetStorage(IStorage storage) => Storage = storage;
    }
}