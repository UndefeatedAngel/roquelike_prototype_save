using NothingAwaits.Core.Actor.UI.Presenters;

namespace NothingAwaits.Core.Coins
{
    public class CoinsPresenter : BasePresenter<CoinsModel, CoinsView>
    {
        public CoinsPresenter(CoinsModel model, CoinsView view) 
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