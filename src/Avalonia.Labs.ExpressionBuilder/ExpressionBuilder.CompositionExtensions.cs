///---------------------------------------------------------------------------------------------------------------------
/// <copyright company="Microsoft">
///     Copyright (c) Microsoft Corporation.  All rights reserved.
/// </copyright>
///---------------------------------------------------------------------------------------------------------------------

namespace ExpressionBuilder
{
    using Avalonia.Animation.Easings;
    using Avalonia.Rendering.Composition;
    using Avalonia.Rendering.Composition.Animations;

    using System.Numerics;

    ///---------------------------------------------------------------------------------------------------------------------
    /// 
    /// class CompositionExtensions
    ///    ToDo: Add description after docs written
    /// 
    ///---------------------------------------------------------------------------------------------------------------------

    public static class CompositionExtensions
    {
        /// <summary> Create an ExpressionNode reference to this CompositionObject. </summary>
#if AVALONIA_COMPOSITION_TODO
        public static AmbientLightReferenceNode       GetReference(this AmbientLight compObj)              { return new AmbientLightReferenceNode(null, compObj);        }
        public static ColorBrushReferenceNode         GetReference(this CompositionColorBrush compObj)     { return new ColorBrushReferenceNode(null, compObj);          }
        public static DistantLightReferenceNode       GetReference(this DistantLight compObj)              { return new DistantLightReferenceNode(null, compObj);        }
        public static DropShadowReferenceNode         GetReference(this DropShadow compObj)                { return new DropShadowReferenceNode(null, compObj);          }
        public static InsetClipReferenceNode          GetReference(this InsetClip compObj)                 { return new InsetClipReferenceNode(null, compObj);           }
        public static InteractionTrackerReferenceNode GetReference(this InteractionTracker compObj)        { return new InteractionTrackerReferenceNode(null, compObj);  }
        public static NineGridBrushReferenceNode      GetReference(this CompositionNineGridBrush compObj)  { return new NineGridBrushReferenceNode(null, compObj);       }
        public static PointLightReferenceNode         GetReference(this PointLight compObj)                { return new PointLightReferenceNode(null, compObj);          }
#endif
        public static PropertySetReferenceNode        GetReference(this CompositionPropertySet compObj)    { return new PropertySetReferenceNode(null, compObj);         }
#if AVALONIA_COMPOSITION_TODO
        public static SpotLightReferenceNode          GetReference(this SpotLight compObj)                 { return new SpotLightReferenceNode(null, compObj);           }
        public static SurfaceBrushReferenceNode       GetReference(this CompositionSurfaceBrush compObj)   { return new SurfaceBrushReferenceNode(null, compObj);        }
#endif
        public static VisualReferenceNode             GetReference(this CompositionVisual compObj)                    { return new VisualReferenceNode(null, compObj);              }

        /// <summary> Create an ExpressionNode reference to this specialized PropertySet. </summary>
        public static T GetSpecializedReference<T>(this CompositionPropertySet ps) where T : PropertySetReferenceNode
        {
            if (typeof(T) == typeof(ManipulationPropertySetReferenceNode))
            {
                return new ManipulationPropertySetReferenceNode(null, ps) as T;
            }
            else if (typeof(T) == typeof(PointerPositionPropertySetReferenceNode))
            {
                return new PointerPositionPropertySetReferenceNode(null, ps) as T;
            }
            else
            {
                throw new System.Exception("Invalid property set specialization");
            }
        }

        /// <summary> Connects the specified ExpressionNode with the specified property of the object. </summary>
        /// <param name="propertyName">The name of the property that the expression will target.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void StartAnimation(this CompositionObject compObject, string propertyName, ExpressionNode expressionNode)
        {
            compObject.StartAnimation(propertyName, CreateExpressionAnimationFromNode(compObject.Compositor, expressionNode));
        }

        /// <summary> Inserts a KeyFrame whose value is calculated using the specified ExpressionNode. </summary>
        /// <param name="normalizedProgressKey">The time the key frame should occur at, expressed as a percentage of the animation Duration. Allowed value is from 0.0 to 1.0.</param>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        /// <param name="easing">The easing function to use when interpolating between frames.</param>
        public static void InsertExpressionKeyFrame(this KeyFrameAnimation keyframeAnimation, float normalizedProgressKey, ExpressionNode expressionNode, Easing easing = null)
        {
            keyframeAnimation.InsertExpressionKeyFrame(normalizedProgressKey, expressionNode.ToExpressionString(), easing);

            expressionNode.SetAllParameters(keyframeAnimation);
        }
#if AVALONIA_COMPOSITION_TODO
        /// <summary> Use the value of specified ExpressionNode to determine if this inertia modifier should be chosen. </summary>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetCondition(this InteractionTrackerInertiaRestingValue modifier, ExpressionNode expressionNode)
        {
            modifier.Condition = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary> Use the value of specified ExpressionNode as the resting value for this inertia modifier. </summary>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetRestingValue(this InteractionTrackerInertiaRestingValue modifier, ExpressionNode expressionNode)
        {
            modifier.RestingValue = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary> Use the value of specified ExpressionNode to determine if this inertia modifier should be chosen. </summary>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetCondition(this InteractionTrackerInertiaMotion modifier, ExpressionNode expressionNode)
        {
            modifier.Condition = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary> Use the value of specified ExpressionNode to dictate the motion for this inertia modifier. </summary>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation.</param>
        public static void SetMotion(this InteractionTrackerInertiaMotion modifier, ExpressionNode expressionNode)
        {
            modifier.Motion = CreateExpressionAnimationFromNode(modifier.Compositor, expressionNode);
        }

        /// <summary> Tries to update the InteractionTracker's position by applying an animation. </summary>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation to apply to the InteractionTracker's position.</param>
        public static void TryUpdatePositionWithAnimation(this InteractionTracker tracker, ExpressionNode expressionNode)
        {
            tracker.TryUpdatePositionWithAnimation(CreateExpressionAnimationFromNode(tracker.Compositor, expressionNode));
        }

        /// <summary> Tries to update the InteractionTracker's scale by applying an animation. </summary>
        /// <param name="expressionNode">The root ExpressionNode that represents the ExpressionAnimation to apply to the InteractionTracker's scale.</param>
        /// <param name="centerPoint">The centerPoint to use when scaling.</param>
        public static void TryUpdateScaleWithAnimation(this InteractionTracker tracker, ExpressionNode expressionNode, Vector3 centerPoint)
        {
            tracker.TryUpdateScaleWithAnimation(CreateExpressionAnimationFromNode(tracker.Compositor, expressionNode), centerPoint);
        }
#endif
        
        //
        // Helper functions
        //
        
        private static ExpressionAnimation CreateExpressionAnimationFromNode(Compositor compositor, ExpressionNode expressionNode)
        {
            // Only create a new animation if this node hasn't already generated one before, so we don't have to re-parse the expression string.
            if (expressionNode._expressionAnimation == null)
            {
                expressionNode._expressionAnimation = compositor.CreateExpressionAnimation(expressionNode.ToExpressionString());
            }

            // We need to make sure all parameters are up to date, even if the animation already existed.
            expressionNode.SetAllParameters(expressionNode._expressionAnimation);

            return expressionNode._expressionAnimation;
        }
    }
}
