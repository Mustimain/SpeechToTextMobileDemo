using System;
using Prism.Mvvm;
using Prism.Navigation;

namespace SpeechToTextMobileDemo.Bases
{
    public abstract class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // INavigationAware
        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        // INavigationAware
        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        // IDestructible
        public virtual void Destroy()
        {

        }
    }
}

