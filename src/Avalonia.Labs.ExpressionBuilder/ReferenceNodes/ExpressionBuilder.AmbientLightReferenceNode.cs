///---------------------------------------------------------------------------------------------------------------------
/// <copyright company="Microsoft">
///     Copyright (c) Microsoft Corporation.  All rights reserved.
/// </copyright>
///---------------------------------------------------------------------------------------------------------------------
#if AVALONIA_COMPOSITION_TODO
namespace ExpressionBuilder
{
    using Avalonia.Rendering.Composition;

    public sealed class AmbientLightReferenceNode : ReferenceNode
    {
        internal AmbientLightReferenceNode(string paramName, AmbientLight light = null) : base(paramName, light)
        {
            throw new System.NotSupportedException("AmbientLightReferenceNode");
        }
        
        internal static AmbientLightReferenceNode CreateTargetReference()
        {
            var node = new AmbientLightReferenceNode(null);
            node._nodeType = ExpressionNodeType.TargetReference;

            return node;
        }
        
        // Animatable properties
        public ColorNode Color { get { return ReferenceProperty<ColorNode>("Color"); } }
    }
}
#endif
