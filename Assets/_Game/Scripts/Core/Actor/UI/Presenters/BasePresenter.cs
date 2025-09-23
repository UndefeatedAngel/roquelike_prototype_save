using System;

namespace BlockoutRoguelike.Core.Actor.UI.Presenters
{
    public abstract class BasePresenter<TModel, TView>
    {
        protected TModel _model;
        protected TView _view;
        private Action _updateViewAction;

        protected BasePresenter(TModel model, TView view, Action updateViewAction)
        {
            _model = model;
            _view = view;
            _updateViewAction = updateViewAction;
        }

        public virtual void Enable()
        {
            SubscribeToModelChanges();
            UpdateView();
        }

        public virtual void Disable()
        {
            UnsubscribeFromModelChanges();
        }

        protected abstract void SubscribeToModelChanges();
        protected abstract void UnsubscribeFromModelChanges();
        protected void UpdateView()
        {
            _updateViewAction?.Invoke();
        }
    }
}