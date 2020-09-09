using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MessageCommunicator.TestGui
{
    public class ConfigurablePropertyMetadata : ValidatableViewModelBase
    {
        private object _hostObject;
        private PropertyInfo _propertyInfo;

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
                    }
                    catch (Exception e)
                    {
                        this.RaisePropertyChanged(nameof(this.ValueAccessor));
                        this.SetError(nameof(this.ValueAccessor), e.Message);
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

        internal ConfigurablePropertyMetadata(PropertyInfo propertyInfo, object hostObject)
        {
            _propertyInfo = propertyInfo;
            _hostObject = hostObject;

            this.CategoryName = propertyInfo.GetCustomAttribute<CategoryAttribute>()?.Category ?? string.Empty;

            this.PropertyName = propertyInfo.Name;
            this.PropertyDisplayName = propertyInfo.Name;
            var attribDisplayName = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
            if (attribDisplayName != null)
            {
                this.PropertyDisplayName = attribDisplayName.DisplayName;
            }

            var propertyType = _propertyInfo.PropertyType;
            if (_propertyInfo.GetCustomAttribute<EncodingWebNameAttribute>() != null)
            {
                this.ValueType = PropertyValueType.EncodingWebName;
            }
            else if (_propertyInfo.GetCustomAttribute<TextAndHexadecimalEditAttribute>() != null)
            {
                this.ValueType = PropertyValueType.TextAndHexadecimalEdit;
            }
            else if (propertyType == typeof(bool))
            {
                this.ValueType = PropertyValueType.Bool;
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
            else if(propertyInfo.GetCustomAttribute<DetailSettingsAttribute>() != null)
            {
                this.ValueType = PropertyValueType.DetailSettings;
            }
            else
            {
                throw new ApplicationException($"Unsupported property type {propertyType.FullName}!");
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
            return Enum.GetValues(_propertyInfo.PropertyType);
        }

        public object? GetValue()
        {
            return _propertyInfo.GetValue(_hostObject);
        }

        public void SetValue(object? value)
        {
            if (value == null)
            {
                var targetType = _propertyInfo.PropertyType;
                if(targetType.IsValueType){ _propertyInfo.SetValue(_hostObject, Activator.CreateInstance(targetType)); }
                else{ _propertyInfo.SetValue(_hostObject, null); }
            }
            else
            {
                var givenType = value.GetType();
                var targetType = _propertyInfo.PropertyType;
                if (givenType == targetType)
                {
                    _propertyInfo.SetValue(_hostObject, value);
                }
                else
                {
                    _propertyInfo.SetValue(_hostObject, Convert.ChangeType(value, targetType));
                }
            }
        }

        public T? GetCustomAttribute<T>()
            where T : Attribute
        {
            return _propertyInfo.GetCustomAttribute<T>();
        }

        private void ValidateCurrentValue()
        {
            var errorsFound = false;
            var ctx = new ValidationContext(_hostObject);
            foreach (var actValidAttrib in _propertyInfo.GetCustomAttributes<ValidationAttribute>())
            {
                var validationResult = actValidAttrib.GetValidationResult(this.ValueAccessor, ctx);
                if (validationResult != null)
                {
                    this.SetError(nameof(this.ValueAccessor), validationResult.ErrorMessage);
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