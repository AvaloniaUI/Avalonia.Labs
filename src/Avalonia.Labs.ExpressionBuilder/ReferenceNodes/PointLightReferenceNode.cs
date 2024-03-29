#if AVALONIA_COMPOSITION_TODO
using Avalonia.Rendering.Composition;

namespace Avalonia.Labs.ExpressionBuilder
{
    /// <summary>
    /// Class PointLightReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Avalonia.Labs.ExpressionBuilder.ReferenceNode" />
    public sealed class PointLightReferenceNode : ReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointLightReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="light">The light.</param>
        internal PointLightReferenceNode(string paramName, PointLight light = null)
            : base(paramName, light)
        {
        }

        /// <summary>
        /// Creates the target reference.
        /// </summary>
        /// <returns>PointLightReferenceNode.</returns>
        internal static PointLightReferenceNode CreateTargetReference()
        {
            var node = new PointLightReferenceNode(null);
            node.NodeType = ExpressionNodeType.TargetReference;

            return node;
        }

        /// <summary>
        /// Gets the constant attenuation.
        /// </summary>
        /// <value>The constant attenuation.</value>
        public ScalarNode ConstantAttenuation
        {
            get { return ReferenceProperty<ScalarNode>("ConstantAttenuation"); }
        }

        /// <summary>
        /// Gets the linear attenuation.
        /// </summary>
        /// <value>The linear attenuation.</value>
        public ScalarNode LinearAttenuation
        {
            get { return ReferenceProperty<ScalarNode>("LinearAttenuation"); }
        }

        /// <summary>
        /// Gets the quadratic attenuation.
        /// </summary>
        /// <value>The quadratic attenuation.</value>
        public ScalarNode QuadraticAttentuation
        {
            get { return ReferenceProperty<ScalarNode>("QuadraticAttentuation"); }
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        public ColorNode Color
        {
            get { return ReferenceProperty<ColorNode>("Color"); }
        }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3Node Direction
        {
            get { return ReferenceProperty<Vector3Node>("Direction"); }
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Vector3Node Offset
        {
            get { return ReferenceProperty<Vector3Node>("Offset"); }
        }
    }
}
#endif
