using BlockoutRoguelike.Core.Actor.Models.Items;
using BlockoutRoguelike.Core.Actor.UI.Views;

namespace BlockoutRoguelike.Core.Actor.UI.Presenters
{
    public class HealthPresenter : BasePresenter<Health, HealthView>
    {
        public HealthPresenter(Health model, HealthView view) 
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