/*
    Seeing# and all applications distributed together with it. 
	Exceptions are projects where it is noted otherwise.
    More info at 
     - https://github.com/RolandKoenig/SeeingSharp2 (sourcecode)
     - http://www.rolandk.de (the authors homepage, german)
    Copyright (C) 2019 Roland König (RolandK)
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see http://www.gnu.org/licenses/.
*/

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using ReactiveUI;

namespace MessageCommunicator.TestGui
{
    public class PropertyGridViewModel : OwnViewModelBase
    {
        private List<ConfigurablePropertyMetadata> _propertyMetadata;
        private object? _selectedObject;

        public object? SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    this.RaisePropertyChanged(nameof(this.SelectedObject));

                    this.UpdatePropertyCollection();
                }
            }
        }

        public List<ConfigurablePropertyMetadata> PropertyMetadata
        {
            get => _propertyMetadata;
            private set
            {
                if (_propertyMetadata != value)
                {
                    _propertyMetadata = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public PropertyGridViewModel()
        {
            _propertyMetadata = new List<ConfigurablePropertyMetadata>(0);
        }

        private void UpdatePropertyCollection()
        {
            var newPropertyMetadata = new List<ConfigurablePropertyMetadata>();

            var selectedObject = this.SelectedObject;
            if (selectedObject == null)
            {
                this.PropertyMetadata = newPropertyMetadata;
                return;
            }

            // Get properties for PropertyGrid
            PropertyDescriptorCollection properties;
            var metadataAttrib = selectedObject.GetType().GetCustomAttribute<MetadataTypeAttribute>();
            if (metadataAttrib != null)
            {
                properties = TypeDescriptor.GetProperties(metadataAttrib.MetadataClassType);
            }
            else
            {
                properties = TypeDescriptor.GetProperties(selectedObject);
            }

            // Create a viewmodel for each property
            foreach (PropertyDescriptor? actProperty in properties)
            {
                if (actProperty == null){ continue; }
                if (!actProperty.IsBrowsable){ continue; }

                newPropertyMetadata.Add(new ConfigurablePropertyMetadata(actProperty, selectedObject));
            }

            this.PropertyMetadata = newPropertyMetadata;
        }
    }
}