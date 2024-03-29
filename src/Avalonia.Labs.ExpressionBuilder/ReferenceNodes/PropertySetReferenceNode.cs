
using Avalonia.Rendering.Composition;

namespace Avalonia.Labs.ExpressionBuilder
{
    /// <summary>
    /// Class PropertySetReferenceNode.
    /// </summary>
    /// <seealso cref="Avalonia.Labs.ExpressionBuilder.ReferenceNode" />
    public class PropertySetReferenceNode : ReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySetReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="ps">The ps.</param>
        internal PropertySetReferenceNode(string? paramName, CompositionPropertySet? ps = null)
            : base(paramName, ps)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySetReferenceNode"/> class.
        /// </summary>
        // Needed for GetSpecializedReference<>()
        internal PropertySetReferenceNode()
            : base(null, null)
        {
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        internal CompositionPropertySet? Source { get; set; }

        /// <summary>
        /// Creates the target reference.
        /// </summary>
        /// <returns>PropertySetReferenceNode.</returns>
        internal static PropertySetReferenceNode CreateTargetReference()
        {
            var node = new PropertySetReferenceNode(null);
            node.NodeType = ExpressionNodeType.TargetReference;

            return node;
        }
    }
}
