using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageCommunicator.TestGui.Data;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ExportDialogControlViewModel : OwnViewModelBase
    {
        public ObservableCollection<ExportLine> ExportLines
        {
            get;
        } = new ObservableCollection<ExportLine>();

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        public ReactiveCommand<object?, Unit> Command_Cancel { get; }

        public ExportDialogControlViewModel(IEnumerable allObjects, IEnumerable objectsToExport, string nameProperty)
        {
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
            var objectToExport =
                from actLine in this.ExportLines
                where actLine.DoExport
                select actLine;
            if (!objectToExport.Any())
            {
                // TODO: Show error info
                return;
            }

            // Show save file dialog
            var srvSaveFile = this.GetViewService<ISaveFileViewService>();
            var fileName = await srvSaveFile.ShowSaveFileDialogAsync(
                new FileDialogFilter[]
                {
                    new FileDialogFilter()
                    {
                        Name = "Json-File (*.json)", 
                        Extensions = { "json" }
                    }
                }, "json" );
            if (string.IsNullOrEmpty(fileName))
            {
                // TODO: Show 'no file selected' message
                return;
            }

            // TODO: Save file

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

            public object ObjectToExport { get; }

            public bool DoExport { get; set; }

            public ExportLine(string name, object objToExport, bool doExport)
            {
                this.Name = name;
                this.ObjectToExport = objToExport;
                this.DoExport = doExport;
            }
        }
    }
}
