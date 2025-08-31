using System;

namespace BlockoutRoguelike
{
    public class Player
    {
        private static int _hp = 30;
        private static int _maxHp = 30;
        private static int _attack = 3;
        private static int _bottle = 0;
        public static int HP => _hp;
        public static int MaxHp => _maxHp;
        public static int Attack => _attack;
        public static int Bottle => _bottle;
        

        public static Action<int> OnHPUpdate;
        public static Action<int> OnBottleUpdate;

        public static void AddHP(int amount)
        {
            _hp += amount;
            if (_hp < 0)
            {
                _hp = 0;
            }
            OnHPUpdate?.Invoke(_hp);
        }
        
        public static void AddBottle(int amount)
        {
            _bottle += amount;
            OnBottleUpdate?.Invoke(_bottle);
        }
    }
}