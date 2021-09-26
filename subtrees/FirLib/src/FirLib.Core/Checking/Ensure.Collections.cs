using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FirLib.Core.Checking
{
    public static partial class Ensure
    {
        [Conditional("DEBUG")]
        public static void EnsureDoesNotContain<T>(
            this IEnumerable<T> collection, T element, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            // Check result
            if (collection.Contains(element))
            {
                throw new FirLibCheckException(
                    $"Collection {checkedVariableName} within method {callerMethod} must not contain element {element}!");
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureMoreThanZeroElements<T>(
            this IEnumerable<T> collection, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            // Get the collection count
            var hasAnyElement = collection.Any();

            // Check result
            if (!hasAnyElement)
            {
                throw new FirLibCheckException(
                    $"Collection {checkedVariableName} within method {callerMethod} must have more than zero elements!");
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureCountInRange<T>(
            this IEnumerable<T> collection, int countMin, int countMax, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            // Get the collection count
            var collectionCount = -1;
            collectionCount = collection.Count();

            // Check result
            if (collectionCount < countMin ||
                collectionCount > countMax)
            {
                throw new FirLibCheckException(
                    $"Collection {checkedVariableName} within method {callerMethod} does not have the expected count of elements (expected min {countMin} to max {countMax}, current count is {collectionCount})!");
            }
        }

        [Conditional("DEBUG")]
        public static void EnsureCountEquals<T>(
            this IEnumerable<T> collection, int count, string checkedVariableName,
            [CallerMemberName]
            string callerMethod = "")
        {
            if (string.IsNullOrEmpty(callerMethod)) { callerMethod = "Unknown"; }

            // Get the collection count
            var collectionCount = -1;
            collectionCount = collection.Count();

            // Check result
            if (collectionCount != count)
            {
                throw new FirLibCheckException(
                    $"Collection {checkedVariableName} within method {callerMethod} does not have the expected count of elements (expected {count}, current {collectionCount})!");
            }
        }
    }
}
