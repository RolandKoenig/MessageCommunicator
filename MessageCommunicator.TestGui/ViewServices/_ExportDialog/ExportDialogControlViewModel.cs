using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ExportDialogControlViewModel<T> : OwnViewModelBase
        where T : class
    {
        private string _dataTypeName;

        public ObservableCollection<ExportLine> ExportLines
        {
            get;
        } = new ObservableCollection<ExportLine>();

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        public ReactiveCommand<object?, Unit> Command_Cancel { get; }

        public ExportDialogControlViewModel(IEnumerable<T> allObjects, IEnumerable<T> objectsToExport, string nameProperty, string dataTypeName)
        {
            _dataTypeName = dataTypeName;

            PropertyInfo? namePropertyObj = null;
            foreach (var actObject in allObjects)
            {
                if(actObject == null){ continue; }

                if (namePropertyObj == null)
                {
                    namePropertyObj = actObject.GetType().GetProperty(nameProperty);
                    if(namePropertyObj == null){ throw new InvalidOperationException($"Unable to find property {nameProperty}!"); }
                }

                var currentName = (namePropertyObj.GetValue(actObject) as string) ?? "?";
                var currentIsSelected = ContainsObject(objectsToExport, actObject);
                this.ExportLines.Add(new ExportLine(currentName, actObject, currentIsSelected));
            }

            this.Command_OK = ReactiveCommand.CreateFromTask<object?>(this.OnCommandOKAsync);
            this.Command_Cancel = ReactiveCommand.Create<object?>(
                arg => this.CloseWindow(null));
        }

        private async Task OnCommandOKAsync(object? arg)
        {
            var srvMessageBox = this.GetViewService<IMessageBoxService>();

            var objectToExport =
                from actLine in this.ExportLines
                where actLine.DoExport
                select actLine.ObjectToExport;
            if (!objectToExport.Any())
            {
                await srvMessageBox.ShowAsync("Export", "Nothing to export!", MessageBoxButtons.Ok);
                return;
            }

            // Show save file dialog
            var srvSaveFile = this.GetViewService<ISaveFileViewService>();
            var fileName = await srvSaveFile.ShowSaveFileDialogAsync(
                new[]
                {
                    new FileDialogFilter()
                    {
                        Name = "Data-Package (*.dataPackage)", 
                        Extensions = { "dataPackage" }
                    }
                }, "dataPackage" );
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            // Export the file
            try
            {
                using (var packageFile = new DataPackageFile(fileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    packageFile.WriteSingleFile(objectToExport, _dataTypeName);
                }
            }
            catch (Exception ex)
            {
                await srvMessageBox.ShowAsync(
                    "Export", 
                    $"Error while exporting file:{Environment.NewLine}{ex.Message}", 
                    MessageBoxButtons.Ok);
                return;
            }

            // Show success message
            await srvMessageBox.ShowAsync(
                "Export", 
                $"Successfully exported {objectToExport.Count()} object(s).",
                MessageBoxButtons.Ok);

            this.CloseWindow(null);
        }

        private static bool ContainsObject(IEnumerable enumerableToCheck, object objToCheckFor)
        {
            foreach (var actObject in enumerableToCheck)
            {
                if (actObject == objToCheckFor) { return true; }
            }
            return false;
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Helper for selecting/deselecting objects.
        /// </summary>
        public class ExportLine
        {
            public string Name { get; }

            public T ObjectToExport { get; }

            public bool DoExport { get; set; }

            public ExportLine(string name, T objToExport, bool doExport)
            {
                this.Name = name;
                this.ObjectToExport = objToExport;
                this.DoExport = doExport;
            }
        }
    }
}
