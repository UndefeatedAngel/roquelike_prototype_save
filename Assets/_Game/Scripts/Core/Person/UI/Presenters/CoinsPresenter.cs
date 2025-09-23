using BlockoutRoguelike.Core.Person.Models.Items;
using BlockoutRoguelike.Core.Person.UI.Views;

namespace BlockoutRoguelike.Core.Person.UI.Presenters
{
    public class CoinsPresenter : BasePresenter<Coins, CoinsUI>
    {
        public CoinsPresenter(Coins model, CoinsUI view) 
            : base(model, view, () => view.UpdateCoinsDisplay(model.Current))
        {
        }

        protected override void SubscribeToModelChanges()
        {
            _model.OnChanged += OnCoinsChanged;
        }

        protected override void UnsubscribeFromModelChanges()
        {
            _model.OnChanged -= OnCoinsChanged;
        }

        private void OnCoinsChanged(int currentCoins) => UpdateView();
    }
}