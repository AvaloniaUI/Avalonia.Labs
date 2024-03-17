#if AVALONIA_COMPOSITION_TODO
using Avalonia.Rendering.Composition;

namespace Avalonia.Labs.ExpressionBuilder
{
    /// <summary>
    /// Class ColorBrushReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Avalonia.Labs.ExpressionBuilder.ReferenceNode" />
    public sealed class ColorBrushReferenceNode : ReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorBrushReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="brush">The brush.</param>
        internal ColorBrushReferenceNode(string paramName, CompositionColorBrush brush = null)
            : base(paramName, brush)
        {
        }

        /// <summary>
        /// Creates the target reference.
        /// </summary>
        /// <returns>ColorBrushReferenceNode.</returns>
        internal static ColorBrushReferenceNode CreateTargetReference()
        {
            var node = new ColorBrushReferenceNode(null);
            node.NodeType = ExpressionNodeType.TargetReference;

            return node;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        public ColorNode Color
        {
            get { return ReferenceProperty<ColorNode>("Color"); }
        }
    }
}
#endif
