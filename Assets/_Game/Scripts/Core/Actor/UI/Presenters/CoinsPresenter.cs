using NothingAwaits.Core.Actor.Models.Items;
using NothingAwaits.Core.Actor.UI.Views;

namespace NothingAwaits.Core.Actor.UI.Presenters
{
    public class CoinsPresenter : BasePresenter<Coins, CoinsView>
    {
        public CoinsPresenter(Coins model, CoinsView view) 
            : base(model, view, () => view.UpdateCoinsDisplay(model.Amount))
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