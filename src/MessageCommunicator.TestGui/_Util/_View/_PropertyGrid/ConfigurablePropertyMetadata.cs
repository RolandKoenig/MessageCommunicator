﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using FirLib.Core.Patterns.Mvvm;

namespace MessageCommunicator.TestGui
{
    public class ConfigurablePropertyMetadata : ValidatableViewModelBase
    {
        private object _hostObject;
        private IPropertyContractResolver? _propertyContractResolver;
        private PropertyDescriptor _descriptor;

        public object? ValueAccessor
        {
            get => this.GetValue();
            set
            {
                if (value != this.GetValue())
                {
                    try
                    {
                        this.SetValue(value);

                        this.RaisePropertyChanged(nameof(this.ValueAccessor));
                        this.RaisePropertyChanged(nameof(this.ValueAccessor)); // <-- Call this twice because otherwise 'TextAndHexadecimal" edit control works not correctly
                    }
                    catch (Exception e)
                    {
                        this.SetError(nameof(this.ValueAccessor), e.Message);
                        this.RaisePropertyChanged(nameof(this.ValueAccessor));
                        return;
                    }

                    this.ValidateCurrentValue();
                }
            }
        }

        public string CategoryName
        {
            get;
            set;
        }

        public string PropertyName
        {
            get;
            set;
        }

        public string PropertyDisplayName
        {
            get;
            set;
        }

        public PropertyValueType ValueType
        {
            get;
            set;
        }

        public bool IsReadOnly
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public Type HostObjectType => _hostObject.GetType();
        
        internal ConfigurablePropertyMetadata(PropertyDescriptor propertyInfo, object hostObject, IPropertyContractResolver? propertyContractResolver)
        {
            _descriptor = propertyInfo;
            _hostObject = hostObject;
            _propertyContractResolver = propertyContractResolver;

            var categoryAttrib = this.GetCustomAttribute<CategoryAttribute>();
            this.CategoryName = categoryAttrib?.Category ?? string.Empty;

            this.PropertyName = propertyInfo.Name;
            this.IsReadOnly = propertyInfo.IsReadOnly;

            this.PropertyDisplayName = propertyInfo.Name;
            var displayNameAttrib = this.GetCustomAttribute<DisplayNameAttribute>();
            if ((displayNameAttrib != null) &&
                (!string.IsNullOrEmpty(displayNameAttrib.DisplayName)))
            {
                this.PropertyDisplayName = displayNameAttrib.DisplayName;
            }

            this.Description = propertyInfo.Description ?? string.Empty;

            var propertyType = _descriptor.PropertyType;
            if (this.GetCustomAttribute<EncodingWebNameAttribute>() != null)
            {
                this.ValueType = PropertyValueType.EncodingWebName;
            }
            else if (this.GetCustomAttribute<TextAndHexadecimalEditAttribute>() != null)
            {
                this.ValueType = PropertyValueType.TextAndHexadecimalEdit;
            }
            else if (propertyType == typeof(bool))
            {
                this.ValueType = PropertyValueType.Bool;
            }
            else if (this.GetCustomAttribute<FixedPossibleValuesAttribute>() != null)
            {
                this.ValueType = PropertyValueType.FixedPossibleValues;
            }
            else if (propertyType == typeof(string) || propertyType == typeof(char) ||
                    propertyType == typeof(double) || propertyType == typeof(float) || propertyType == typeof(decimal) ||
                    propertyType == typeof(int) || propertyType == typeof(uint) ||
                    propertyType == typeof(byte) ||
                    propertyType == typeof(short) || propertyType == typeof(ushort) ||
                    propertyType == typeof(long) || propertyType == typeof(ulong))
            {
                this.ValueType = PropertyValueType.String;
            }
            else if (propertyType.IsSubclassOf(typeof(Enum)))
            {
                this.ValueType = PropertyValueType.Enum;
            }
            else if(this.GetCustomAttribute<DetailSettingsAttribute>() != null)
            {
                this.ValueType = PropertyValueType.DetailSettings;
            }
            else
            {
                this.ValueType = PropertyValueType.Unsupported;
            }

            this.ValidateCurrentValue();
        }

        public override string ToString()
        {
            return $"{this.CategoryName} - {this.PropertyDisplayName} (type {this.ValueType})";
        }

        public Array GetEnumMembers()
        {
            if (this.ValueType != PropertyValueType.Enum) { throw new InvalidOperationException($"Method {nameof(this.GetEnumMembers)} not supported on value type {this.ValueType}!"); }
            return Enum.GetValues(_descriptor.PropertyType);
        }

        public object? GetValue()
        {
            return _descriptor.GetValue(_hostObject);
        }

        public void SetValue(object? value)
        {
            if (value == null)
            {
                var targetType = _descriptor.PropertyType;
                if(targetType.IsValueType){ _descriptor.SetValue(_hostObject, Activator.CreateInstance(targetType)); }
                else{ _descriptor.SetValue(_hostObject, null); }
            }
            else
            {
                var givenType = value.GetType();
                var targetType = _descriptor.PropertyType;
                if (givenType == targetType)
                {
                    _descriptor.SetValue(_hostObject, value);
                }
                else
                {
                    _descriptor.SetValue(_hostObject, Convert.ChangeType(value, targetType));
                }
            }
        }

        public T? GetCustomAttribute<T>()
            where T : Attribute
        {
            var resultFromDataAnnotator = _propertyContractResolver?.GetDataAnnotation<T>(_descriptor.ComponentType, _descriptor.Name);
            if (resultFromDataAnnotator != null)
            {
                return resultFromDataAnnotator;
            }

            foreach (var actAttribute in _descriptor.Attributes)
            {
                if (actAttribute is T foundAttribute)
                {
                    return foundAttribute;
                }
            }

            return null;
        }

        private void ValidateCurrentValue()
        {
            var errorsFound = false;
            var ctx = new ValidationContext(_hostObject);
            ctx.DisplayName = this.PropertyDisplayName;
            ctx.MemberName = this.PropertyName;
            foreach (var actAttrib in _descriptor.Attributes)
            {
                if(!(actAttrib is ValidationAttribute actValidAttrib)){ continue; }

                var validationResult = actValidAttrib.GetValidationResult(this.ValueAccessor, ctx);
                if (validationResult != null)
                {
                    this.SetError(nameof(this.ValueAccessor), validationResult.ErrorMessage ?? "Unknown");
                    errorsFound = true;
                }
            }

            if (!errorsFound)
            {
                this.RemoveErrors(nameof(this.ValueAccessor));
            }
        }
    }
}