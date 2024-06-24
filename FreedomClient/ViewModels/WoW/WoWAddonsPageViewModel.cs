using FreedomClient.Commands;
using FreedomClient.DAL;
using FreedomClient.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FreedomClient.ViewModels.WoW
{
    [AddINotifyPropertyChangedInterface]
    public class WoWAddonsPageViewModel : IViewModel
    {
        private AddonsRepository _repository;
        private ApplicationState _appState;
        private readonly IMediator _mediator;

        public AddonsViewModel AddonsViewModel { get; set; }

        public WoWAddonsPageViewModel(AddonsRepository repository, IMediator mediator, ApplicationState appState)
        {
            _repository = repository;
            _mediator = mediator;
            _appState = appState;

            AddonsViewModel = new AddonsViewModel
            {
                IsLoading = true,
                InstallCommand = new RelayCommand(
                    (_) => !_appState.UIOperation.IsBusy,
                    (obj) => _mediator.Send(new InstallWoWAddonCommand(obj as Addon))
                ),
                RemoveCommand = new RelayCommand(
                    (_) => !_appState.UIOperation.IsBusy,
                    (obj) => _mediator.Send(new RemoveWoWAddonCommand(obj as Addon))
                ),
                InstallRecommendedCommand = new RelayCommand(
                    (_) => !_appState.UIOperation.IsBusy,
                    (obj) => _mediator.Send(new InstallWoWAddonCollectionCommand(obj as AddonCollection))
                ),
                RemoveRecommendedCommand = new RelayCommand(
                    (_) => !_appState.UIOperation.IsBusy,
                    (obj) => _mediator.Send(new RemoveWoWAddonCollectionCommand(obj as AddonCollection))
                ),
            };

            LoadAddonsAsync().ConfigureAwait(false);
        }

        private async Task LoadAddonsAsync()
        {
            var recommendedAddons = await _repository.GetFreedomRecommendedAddons();
            AddonsViewModel.RecommendedAddons = recommendedAddons;
            var addons = await _repository.GetAddons();
            AddonsViewModel.Addons = addons;
            AddonsViewModel.IsLoading = false;
        }
    }
}
