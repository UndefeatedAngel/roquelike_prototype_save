namespace BlockoutRoguelike.Cards.Person
{
    public class PlayerBattle: BasePerson
    {
        protected override void Start()
        {
            _currentHp = Player.HP;
            _maxHp = Player.MaxHp;
            UpdateHp();
        }

        public override void AddHp(int amount)
        {
            base.AddHp(amount);
            Player.AddHP(amount);
        }
    }
}