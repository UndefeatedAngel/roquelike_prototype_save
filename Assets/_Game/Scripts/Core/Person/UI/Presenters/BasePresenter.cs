using System;

namespace BlockoutRoguelike.Core.Person.UI.Presenters
{
    public abstract class BasePresenter<TModel, TView>
    {
        protected TModel _model;
        protected TView _view;
        protected Action _updateViewAction;

        public BasePresenter(TModel model, TView view, Action updateViewAction)
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