using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Linq;
using System.Reflection;
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
