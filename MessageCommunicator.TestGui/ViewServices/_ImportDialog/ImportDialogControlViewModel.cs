using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using Force.DeepCloner;
using MessageCommunicator.TestGui.Data;
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

        public ImportDialogControlViewModel(ICollection<T> importTarget, IEnumerable<T> importedObjects, string nameProperty, string dataTypeName)
        {
            _importTarget = importTarget;
            _nameProperty = nameProperty;
            _dataTypeName = dataTypeName;

            var namePropertyObj = typeof(T).GetProperty(nameProperty);
            if(namePropertyObj == null){ throw new InvalidOperationException($"Property {nameProperty} is not available on type {typeof(T).FullName}!"); }

            foreach (var actImportedObject in importedObjects)
            {
                var actImportObjectName = (string?)namePropertyObj.GetValue(actImportedObject);
                if(string.IsNullOrEmpty(actImportObjectName)){ continue; }

                var isAvailable = false;
                foreach (var actExistingObject in importTarget)
                {
                    if ((string?)namePropertyObj.GetValue(actExistingObject) == actImportObjectName)
                    {
                        isAvailable = true;
                        break;
                    }
                }

                this.ImportLines.Add(new ImportLine(actImportObjectName, actImportedObject, !isAvailable, isAvailable));
            }
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

            public object ObjectToImport { get; }

            public bool DoImport { get; set; }

            public bool WouldOverride { get; set; }

            public string DisplayText
            {
                get
                {
                    if (this.WouldOverride) { return $"{this.Name} (override)"; }
                    else { return this.Name; }
                }
            }

            public ImportLine(string name, object objToImport, bool doImport, bool wouldOverride)
            {
                this.Name = name;
                this.ObjectToImport = objToImport;
                this.DoImport = doImport;
                this.WouldOverride = wouldOverride;
            }
        }
    }
}
