using FreedomClient.Core;
using FreedomClient.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FreedomClient.Commands
{
    public class RestoreWoWDB2FilesCommand : IRequest
    {
    }

    public class RestoreWoWDB2FilesCommandHandler : FileClientUIOperationCommandHandler, IRequestHandler<RestoreWoWDB2FilesCommand>
    {
        public RestoreWoWDB2FilesCommandHandler(VerifiedFileClient fileClient, ApplicationState appState, ILogger<RestoreWoWDB2FilesCommandHandler> logger)
            : base(fileClient, appState, logger)
        {

        }

        public async Task Handle(RestoreWoWDB2FilesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting DB2 file restore...");
                _appState.UIOperation = new UIOperation()
                {
                    Name = "Restore WoW DB2 Files",
                    IsCancellable = true,
                    Message = "Verifiying DB2 files integrity..."
                };
                _appState.LoadState = ApplicationLoadState.VerifyingFiles;
                var manifest = _appState.LastManifest;
                if (manifest.Count == 0)
                {
                    manifest = await _fileClient.GetManifest(_appState.UIOperation.CancellationTokenSource.Token);
                    _appState.LastManifest = manifest;
                }

                manifest = manifest
                    .Filter(x => x.Key.Contains("files/dbfilesclient/"));

                await _fileClient.EnsureFilesInManifest(manifest, _appState.InstallPath!, _appState.UIOperation.CancellationTokenSource.Token);
                StopClientOperation();
                _logger.LogInformation("DB2 files restored!");


                _appState.LoadState = ApplicationLoadState.ReadyToLaunch;
                _appState.UIOperation.Progress = 100;
                _appState.UIOperation.Message = "DB2 files succesfully restored!.";
                _appState.UIOperation.ProgressReport = "";
                _appState.UIOperation.IsFinished = true;
                CommandManager.InvalidateRequerySuggested();
            }
            catch(OperationCanceledException) {
                _appState.UIOperation.Message = "DB2 file verification cancelled.";
                _appState.UIOperation.IsCancelled = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

    }
}
