using System;
using NothingAwaits.Core.Actor.UI.Presenters;

namespace NothingAwaits.Core.Blood
{
    public class BloodPresenter: BasePresenter<BloodModel, BloodView>
    {
        public BloodPresenter(BloodModel model, BloodView view, Action updateViewAction) 
            : base(model, view, ()=> view.UpdateBloodDisplay(model.Current, model.Max))
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