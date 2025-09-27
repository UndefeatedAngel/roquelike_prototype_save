using NothingAwaits.Core.Actor.UI.Presenters;

namespace NothingAwaits.Core.Health
{
    public class HealthPresenter : BasePresenter<HealthModel, HealthView>
    {
        public HealthPresenter(HealthModel model, HealthView view) 
            : base(model, view, () => view.UpdateHealthDisplay(model.Current, model.Max))
        {
        }

        protected override void SubscribeToModelChanges()
        {
            _model.OnChanged += OnHealthChanged;
        }

        protected override void UnsubscribeFromModelChanges()
        {
            _model.OnChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int currentHealth) => UpdateView();
    }
}