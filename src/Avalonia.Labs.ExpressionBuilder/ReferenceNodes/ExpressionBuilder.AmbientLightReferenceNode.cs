
using Avalonia.Rendering.Composition;
///---------------------------------------------------------------------------------------------------------------------
/// <copyright company="Microsoft">
///     Copyright (c) Microsoft Corporation.  All rights reserved.
/// </copyright>
///---------------------------------------------------------------------------------------------------------------------
namespace ExpressionBuilder
{


    public sealed class AmbientLightReferenceNode : ReferenceNode
    {
        internal AmbientLightReferenceNode(string paramName, CompositionObject light = null) : base(paramName, light)
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