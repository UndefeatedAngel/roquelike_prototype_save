namespace BlockoutRoguelike.DataStorage
{
    public class StorageManager
    {
        private static IDataStorage _storage;
        
        public static IDataStorage Storage 
        { 
            get 
            { 
                if (_storage == null)
                    _storage = new PlayerPrefsDataStorage();
                return _storage;
            }
            set { _storage = value; }
        }
        
        public static void SetStorage(IDataStorage storage) => Storage = storage;
    }
}