
using Avalonia.Rendering.Composition;

namespace Avalonia.Labs.ExpressionBuilder
{
    /// <summary>
    /// Class PointerPositionPropertySetReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Avalonia.Labs.ExpressionBuilder.PropertySetReferenceNode" />
    public sealed class PointerPositionPropertySetReferenceNode : PropertySetReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointerPositionPropertySetReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="ps">The ps.</param>
        internal PointerPositionPropertySetReferenceNode(string? paramName, CompositionPropertySet? ps = null)
            : base(paramName, ps)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointerPositionPropertySetReferenceNode"/> class.
        /// Needed for GetSpecializedReference
        /// </summary>
        internal PointerPositionPropertySetReferenceNode()
            : base(null, null)
        {
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3Node Position
        {
            get { return ReferenceProperty<Vector3Node>("Position"); }
        }
    }
}
