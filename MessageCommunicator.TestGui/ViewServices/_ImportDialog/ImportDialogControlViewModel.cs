using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace MessageCommunicator.TestGui.ViewServices
{
    public class ImportDialogControlViewModel<T> : OwnViewModelBase
        where T : class
    {
        private ICollection<T> _importTarget;
        private string _nameProperty;
        private string _dataTypeName;

        public ObservableCollection<ImportLine> ImportLines
        {
            get;
        } = new ObservableCollection<ImportLine>();

        public ReactiveCommand<object?, Unit> Command_OK { get; }

        public ReactiveCommand<object?, Unit> Command_Cancel { get; }

        public ImportDialogControlViewModel(ICollection<T> importTarget, IEnumerable<T> importedObjects, string nameProperty, string dataTypeName)
        {
            _importTarget = importTarget;
            _nameProperty = nameProperty;
            _dataTypeName = dataTypeName;

            var namePropertyObj = typeof(T).GetProperty(_nameProperty);
            if(namePropertyObj == null){ throw new InvalidOperationException($"Property {_nameProperty} is not available on type {typeof(T).FullName}!"); }

            foreach (var actImportedObject in importedObjects)
            {
                var actImportObjectName = (string?)namePropertyObj.GetValue(actImportedObject);
                if(string.IsNullOrEmpty(actImportObjectName)){ continue; }

                T? existingObject = null;
                foreach (var actExistingObject in importTarget)
                {
                    if ((string?)namePropertyObj.GetValue(actExistingObject) == actImportObjectName)
                    {
                        existingObject = actExistingObject;
                        break;
                    }
                }

                this.ImportLines.Add(new ImportLine(actImportObjectName, actImportedObject, existingObject));
            }

            this.Command_OK = ReactiveCommand.Create<object?>(this.OnCommandOK);
            this.Command_Cancel = ReactiveCommand.Create<object?>(
                arg => this.CloseWindow(null));
        }

        private void OnCommandOK(object? arg)
        {
            var result = new ImportResult<T>();
            foreach (var actImportLine in this.ImportLines)
            {
                if(!actImportLine.DoImport){ continue; }

                if (actImportLine.WouldOverride)
                {
                    result.UpdatedObjects.Add(new UpdatedObjectInfo<T>(
                        actImportLine.ExistingObject!, actImportLine.ObjectToImport));
                }
                else
                {
                    result.NewObjects.Add(actImportLine.ObjectToImport);
                }
            }

            this.CloseWindow(result);
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        /// <summary>
        /// Helper for selecting/deselecting objects.
        /// </summary>
        public class ImportLine
        {
            public string Name { get; }

            public T ObjectToImport { get; }

            public bool DoImport { get; set; }

            public bool WouldOverride { get; set; }

            public T? ExistingObject { get; set; }

            public string DisplayText
            {
                get
                {
                    if (this.WouldOverride) { return $"{this.Name} (override)"; }
                    else { return this.Name; }
                }
            }

            public ImportLine(string name, T objToImport, T? existingObject)
            {
                this.Name = name;
                this.ObjectToImport = objToImport;
                this.DoImport = existingObject == null;
                this.WouldOverride = existingObject != null;
                this.ExistingObject = existingObject;
            }
        }
    }
}
