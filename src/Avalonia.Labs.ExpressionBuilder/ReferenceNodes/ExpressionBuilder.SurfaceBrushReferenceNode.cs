///---------------------------------------------------------------------------------------------------------------------
/// <copyright company="Microsoft">
///     Copyright (c) Microsoft Corporation.  All rights reserved.
/// </copyright>
///---------------------------------------------------------------------------------------------------------------------

namespace Avalonia.Labs.ExpressionBuilder
{
    using Avalonia.Rendering.Composition;

    public sealed class SurfaceVisualReferenceNode : ReferenceNode
    {
        internal SurfaceVisualReferenceNode(string paramName, CompositionSurface brush = null) : base(paramName, brush) { }
        
        internal static SurfaceVisualReferenceNode CreateTargetReference()
        {
            var node = new SurfaceVisualReferenceNode(null);
            node._nodeType = ExpressionNodeType.TargetReference;

            return node;
        }

        // Animatable properties
#if AVALONIA_COMPOSITION_TODO
        public ScalarNode    HorizontalAlignmentRatio { get { return ReferenceProperty<ScalarNode>("HorizontalAlignmentRatio"); } }
        public ScalarNode    VerticalAlignmentRatio   { get { return ReferenceProperty<ScalarNode>("VerticalAlignmentRatio");   } }

        public ScalarNode    BottomInset              { get { return ReferenceProperty<ScalarNode>("BottomInset");              } }
        public ScalarNode    LeftInset                { get { return ReferenceProperty<ScalarNode>("LeftInset");                } }
        public ScalarNode    RightInset               { get { return ReferenceProperty<ScalarNode>("RightInset");               } }
        public ScalarNode    TopInset                 { get { return ReferenceProperty<ScalarNode>("TopInset");                 } }
#endif

        public ScalarNode    RotationAngle            { get { return ReferenceProperty<ScalarNode>("RotationAngle");            } }
#if AVALONIA_COMPOSITION_TODO
        public ScalarNode    RotationAngleInDegrees   { get { return ReferenceProperty<ScalarNode>("RotationAngleInDegrees");   } }
#endif
        public Vector2Node   AnchorPoint              { get { return ReferenceProperty<Vector2Node>("AnchorPoint");             } }
        public Vector2Node   CenterPoint              { get { return ReferenceProperty<Vector2Node>("CenterPoint");             } }
        public Vector2Node   Offset                   { get { return ReferenceProperty<Vector2Node>("Offset");                  } }
        public Vector2Node   Scale                    { get { return ReferenceProperty<Vector2Node>("Scale");                   } }

        public Matrix3x2Node TransformMatrix          { get { return ReferenceProperty<Matrix3x2Node>("TransformMatrix");       } }
    }
}
