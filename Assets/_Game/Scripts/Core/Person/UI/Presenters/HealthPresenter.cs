using BlockoutRoguelike.Core.Person.Models.Items;
using BlockoutRoguelike.Core.Person.UI.Views;

namespace BlockoutRoguelike.Core.Person.UI.Presenters
{
    public class HealthPresenter : BasePresenter<Health, HealthUI>
    {
        public HealthPresenter(Health model, HealthUI view) 
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