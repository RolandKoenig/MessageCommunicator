using System;
using System.Collections.Generic;
using System.Reflection;
using FirLib.Core.Checking;

namespace FirLib.Core.Patterns.ErrorAnalysis
{
    public class ExceptionInfoNode : IComparable<ExceptionInfoNode>
    {
        public Exception? Exception { get; }

        /// <summary>
        /// Gets a collection containing all child nodes.
        /// </summary>
        public List<ExceptionInfoNode> ChildNodes { get; } = new();

        public bool IsExceptionNode => this.Exception != null;

        public string PropertyName { get; }

        public string PropertyValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionInfoNode"/> class.
        /// </summary>
        public ExceptionInfoNode(Exception ex)
        {
            ex.EnsureNotNull(nameof(ex));

            this.Exception = ex;

            this.PropertyName = ex.GetType().GetTypeInfo().Name;
            this.PropertyValue = ex.Message;
        }

        public ExceptionInfoNode(ExceptionProperty property)
        {
            property.EnsureNotNull(nameof(property));

            this.PropertyName = property.Name;
            this.PropertyValue = property.Value;
        }

        public int CompareTo(ExceptionInfoNode? other)
        {
            if (other == null) { return -1; }
            if(this.IsExceptionNode != other.IsExceptionNode)
            {
                if (this.IsExceptionNode) { return 1; }
                else { return -1; }
            }

            return 0;
        }

        public override string ToString()
        {
            return $"{this.PropertyName}: {this.PropertyValue}";
        }
    }
}
