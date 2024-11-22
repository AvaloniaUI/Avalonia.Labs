using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Avalonia.Labs.Catalog.MarkupExtensions;

    public sealed class EnumItemsSourceExtension : MarkupExtension
    {
        #region MarkupExtension Members

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Type == null) throw new InvalidOperationException("You need to set the Enum type");

            Type actualType = Nullable.GetUnderlyingType(Type) ?? Type;
            TypeConverter typeConverter;
            ICollection? standardValues;

            if ((typeConverter = TypeDescriptor.GetConverter(actualType)) == null ||
                (standardValues = typeConverter.GetStandardValues(serviceProvider as ITypeDescriptorContext)) == null)
                throw new ArgumentException(null, nameof(serviceProvider));

            object?[] items = Type == actualType
                ? new object?[standardValues.Count]
                : new object?[standardValues.Count + 1];
            var index = 0;

            if (Converter == null)
            {
                foreach (object standardValue in standardValues) items[index++] = standardValue;
            }
            else
            {
                var culture = ConverterCulture ?? GetCulture(serviceProvider) ?? CultureInfo.CurrentCulture;

                foreach (object standardValue in standardValues)
                    items[index++] = Converter.Convert(standardValue, typeof(object), ConverterParameter, culture);

                if (Type != actualType)
                    items[index] = Converter.Convert(null, typeof(object), ConverterParameter, culture);
            }

            return items;
        }

        #endregion

        #region Private Methods

        private static CultureInfo? GetCulture(IServiceProvider serviceProvider)
        {
     
            // var provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            // TODO

            //if (provideValueTarget != null)
            //{
            //    IAvaloniaObject targetObject = provideValueTarget.TargetObject as IAvaloniaObject;
            //    XmlLanguage language;

            //    if ((targetObject = provideValueTarget.TargetObject as DependencyObject) != null &&
            //        (language = (XmlLanguage)targetObject.GetValue(FrameworkElement.LanguageProperty)) != null)
            //    { return language.GetSpecificCulture(); }
            //}
            return null;
        }

        #endregion

        #region Fields

        #endregion

        #region Constructors

        public EnumItemsSourceExtension()
        {
        }

        public EnumItemsSourceExtension(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            Type = type;
        }

        #endregion

        #region Properties

        [ConstructorArgument("type")] public Type? Type { get; set; }

        public IValueConverter? Converter { get; set; }

        public CultureInfo? ConverterCulture { get; set; }

        public object? ConverterParameter { get; set; }

        #endregion
    }
