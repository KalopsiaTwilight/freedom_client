using FreedomClient.Core;
using FreedomClient.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FreedomClient.Commands
{
    public class InstallWoWAddonCollectionCommand : IRequest
    {
        public AddonCollection? Collection { get; set; }
        public InstallWoWAddonCollectionCommand(AddonCollection? collection)
        {
            Collection = collection;
        }
    }

    public class InstallWoWAddonCollectionCommandHandler :  IRequestHandler<InstallWoWAddonCollectionCommand>
    {
        private ApplicationState _appState;
        private ILogger _logger;
        private IMediator _mediator;

        public InstallWoWAddonCollectionCommandHandler(ApplicationState appState, ILogger<InstallWoWAddonCollectionCommandHandler> logger, IMediator mediator)
        {
            _appState = appState;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(InstallWoWAddonCollectionCommand request, CancellationToken cancellationToken)
        {
            if (request.Collection == null)
            {
                return;
            }
            try
            {
                _appState.UIOperation = new UIOperation()
                {
                    Name = "Downloading Addon Files",
                    IsCancellable = true,
                    Message = $"Installing {request.Collection.Title}...",
                    Progress = 0
                };

                foreach(var addonTitle in request.Collection.Addons)
                {
                    var addon = _appState.AvailableAddons.FirstOrDefault(x => x.Title == addonTitle);
                    await _mediator.Send(new InstallWoWAddonCommand(addon));
                }

                request.Collection.IsInstalled = true;

                _appState.UIOperation.Progress = 100;
                _appState.UIOperation.Message = $"Successfully installed collection: {request.Collection.Title}!";
                _appState.UIOperation.ProgressReport = "";
                _appState.UIOperation.IsFinished = true;
                _appState.LoadState = ApplicationLoadState.ReadyToLaunch;
                CommandManager.InvalidateRequerySuggested();
            }
            catch(OperationCanceledException)
            {
                _appState.UIOperation.Message = "Installation cancelled.";
                _appState.UIOperation.IsCancelled = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
