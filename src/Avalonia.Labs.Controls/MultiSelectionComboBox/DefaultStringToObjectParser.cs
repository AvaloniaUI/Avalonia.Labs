using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Avalonia.Labs.Controls
{
    /// <summary>
    ///   This class is a helper class for the <see cref="MultiSelectionComboBox"/>. 
    ///   It uses the <see cref="TypeConverter"/> for the elements <see cref="Type"/>. If you need more control
    ///   over the conversion, you should create your own class which implements <see cref="IParseStringToObject"/>
    /// </summary>
    public class DefaultStringToObjectParser : IParseStringToObject
    {
        /// <summary>
        /// The default instance of the <see cref="DefaultStringToObjectParser"/>.
        /// This can be used if you don't need to create your own implementation of the <see cref="IParseStringToObject"/> interface.
        /// </summary>
        public static DefaultStringToObjectParser Instance {get; } = new();

        /// <inheritdoc />
        [UnconditionalSuppressMessage("Trimming", 
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", 
            Justification = "The TypeConverter is used in a try/catch block and is only a last fallback if the input string cannot be parsed by other means. If the TypeConverter is not available, the method will simply return false and the MultiSelectionComboBox will not be able to parse the input string, but it will not break the application.")]
        public bool TryCreateObjectFromString(
            string? input,
            out object? result,
            CultureInfo? culture = null,
            string? stringFormat = null,
            Type? targetType = null)
        {
            culture ??= CultureInfo.InvariantCulture;

            if (input is null)
            {
                result = null;
                return true;
            }

            if (targetType is null)
            {
                result = null;
                return false;
            }

            var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (nonNullableType == typeof(string))
                {
                    result = input;
                    return true;
                }

                if (nonNullableType.IsEnum)
                {
                    result = Enum.Parse(nonNullableType, input, ignoreCase: true);
                    return true;
                }

                if (nonNullableType == typeof(Guid))
                {
                    if (Guid.TryParse(input, out var guid))
                    {
                        result = guid;
                        return true;
                    }

                    result = null;
                    return false;
                }

                // Covers int, double, decimal, DateTime, etc. when IConvertible.
                result = Convert.ChangeType(input, nonNullableType, culture);
                return true;
            }
            catch
            {
                // Last fallback only if needed.
                try
                {
                    var converter = TypeDescriptor.GetConverter(nonNullableType);
                    if (converter.CanConvertFrom(typeof(string)))
                    {
                        result = converter.ConvertFrom(null, culture, input);
                        return true;
                    }
                }
                catch
                {
                    // ignored
                }

                result = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to get the elements <see cref="Type"/> for a given <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="list">Any collection of elements</param>
        /// <returns>the elements <see cref="Type"/></returns>
        public Type? GetElementType(IEnumerable? list)
        {
            if (list is null)
            {
                return null;
            }

            var listType = list.GetType();

            return listType.IsGenericType ? listType.GetGenericArguments().FirstOrDefault() : listType.GetElementType();
        }
    }
}
