namespace BlockoutRoguelike.Cards.Person
{
    public class PlayerBattle: BasePerson
    {
        protected override void Start()
        {
            _currentHp = Player.HP;
            _maxHp = Player.MaxHp;
            base.Start();
        }
    }
}