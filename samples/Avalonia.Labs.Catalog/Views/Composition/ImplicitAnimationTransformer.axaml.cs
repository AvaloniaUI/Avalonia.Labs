//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;

namespace Avalonia.Labs.Catalog.Views
{
    /// <summary>
    /// Creates a number of visuals
    /// Creates ImplicitAnimationMaps and shares between the visuals
    /// Adds a group of animations to the ImplicitAnimationMaps
    /// Implicitly kicks off the animations on offset and scale changes
    /// Creates Circle/Spiral/Ellipse/Collapsed Layouts
    /// </summary>
    public partial class ImplicitAnimationTransformer : UserControl
    {
        // Composition
        private Compositor? _compositor;
        private CompositionContainerVisual? _root;
        private List<Bitmap>? _imageList;

        // Constants
        private const float _posX = 0;
        private const float _posY = 0;
        private const float _circleRadius = 300;
        private const float _ellipseRadiusX = 400;
        private const float _ellipseRadiusY = 200;
        private const double _spiralOrientation = 5;
        private const double _spiralTightness = 0.8;
        private const int _distance = 60;
        private const int _rowCount = 13;
        private const int _columnCount = 13;

        //Helper
        private readonly Random randomBrush = new Random();

        public ImplicitAnimationTransformer()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            // Retrieve an instance of the Compositor from the backing Visual of the Page
            _compositor = ElementComposition.GetElementVisual(this)!.Compositor;
            _root = (CompositionContainerVisual)ElementComposition.GetElementVisual(Placeholder)!;

            // Assign initial values to variables used to store updated offsets for the visuals          
            float posXUpdated = _posX;
            float posYUpdated = _posY;


            //Create a list of image brushes that can be applied to a visual
            string[] imageNames = { "60Banana.png", "60Lemon.png", "60Vanilla.png", "60Mint.png", "60Orange.png", "110Strawberry.png", "60SprinklesRainbow.png" };
            _imageList = new List<Bitmap>(10);
            for (int k = 0; k < imageNames.Length; k++)
            {
                var bitmap = new Bitmap(AssetLoader.Open(new Uri("avares://Avalonia.Labs.Catalog/Assets/Composition/" + imageNames[k])));
                _imageList.Add(bitmap);
            }

            // Create nxn matrix of visuals where n=row/ColumnCount-1 and passes random image brush to the function
            // that creates a visual
            for (int i = 1; i < _rowCount; i++)
            {
                posXUpdated = i * _distance;
                for (int j = 1; j < _columnCount; j++)
                {
                    var bitmap = _imageList[randomBrush.Next(_imageList.Count)];

                    posYUpdated = j * _distance;

                    var image = new Image
                    {
                        Source = bitmap,
                        Width = 50,
                        Height = 50,
                        RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
                    };
                    Placeholder.Children.Add(image);

                    var visual = ElementComposition.GetElementVisual(image);
                    visual.Offset = new Vector3(posXUpdated, posYUpdated, 0);
                }
            }

            // Update the default animation state
            UpdateAnimationState(true);
        }

        /// <summary>
        ///  This method implicitly animates the visual elements into a Grid layout
        ///  on offset change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridLayout(object sender, RoutedEventArgs e)
        {
            //get the position of the initial grid element
            float posXgrid = _posX;
            float posYgrid = _posY;

            List<Vector3> newOffset = new List<Vector3>();

            // Calculate the position for each visual in the grid layout
            for (int i = 1; i < _rowCount; i++)
            {
                posXgrid = i * _distance;
                for (int j = 1; j < _columnCount; j++)
                {
                    posYgrid = j * _distance;
                    newOffset.Add(new Vector3(posXgrid, posYgrid, 0));
                }
            }
            //counter for adding elements to the grid
            int k = 0;

            foreach (var child in _root.Children)
            {
                child.Offset = newOffset[k];
                k++;

            }
        }

        /// <summary>
        ///  This method implicitly animates the visual elements into a circle layout
        ///  on offset change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CircleLayout(object sender, RoutedEventArgs e)
        {
            //
            // Define initial angle of each element on the spiral
            //
            double theta = 0;
            double thetaRadians = 0;

            foreach (var child in _root.Children)
            {
                // Change the Offset property of the visual. This will trigger the implicit animation that is associated with the Offset change.
                // The position of the element on the circle is defined using parametric equation:
                child.Offset = new Vector3((float)(_circleRadius * Math.Cos(thetaRadians)) + _posX, (float)(_circleRadius * Math.Sin(thetaRadians) + _posY), 0);

                // Update the angle to be used for the next visual element
                theta += 2.5;
                thetaRadians = theta * Math.PI / 180F;
            }
        }

        /// <summary>
        /// This method implicitly animates the visual elements into a spiral layout
        /// on offset change 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpiralLayout(object sender, RoutedEventArgs e)
        {

            // Define initial angle of each element on the spiral
            double theta = 0;
            double thetaOrientationRadians = _spiralOrientation * Math.PI / 180F;

            foreach (var child in _root.Children)
            {
                // Change the Offset property of the visual. This will trigger the implicit animation that is associated with the Offset change.
                // Define the position of the visual on the spiral using parametric equation:
                // x = beta*cos(theta + alpha); y = beta*sin(theta + alpha ) 
                child.Offset = new Vector3((float)(_spiralTightness * theta * (Math.Cos(thetaOrientationRadians))) + _posX, (float)(_spiralTightness * theta * (Math.Sin(thetaOrientationRadians))) + _posY, 0);

                // Update the angle to be used for the next visual element
                theta += 4;
                thetaOrientationRadians = (theta + _spiralOrientation) * Math.PI / 180F;
            }
        }

        /// <summary>
        ///This method implicitly animates and rotates the visual elements into an ellipse layout
        /// on offset change  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EllipseLayout(object sender, RoutedEventArgs e)
        {
            // Define initial angle of each element on the ellipse
            double theta = 0;
            double thetaRadians = 0;

            foreach (var child in _root.Children)
            {
                // Change the Offset property of the visual. This will trigger the implicit animation that is associated with the Offset change.
                // The position of the element on the ellipse is defined using parametric equation:
                // x = alpha * cos(theta) ; y = beta*sin(theta)
                child.Offset = new Vector3((float)(_ellipseRadiusX * Math.Cos(thetaRadians)) + _posX, (float)(_ellipseRadiusY * Math.Sin(thetaRadians)) + _posY, 0);


                // Update the angle to be used for the next visual element
                theta += 2.5;
                thetaRadians = theta * Math.PI / 180F;
            }
        }

        /// <summary>
        /// This method animates collapsing all the visual elements into the same position
        /// and implicitly kicks off opacity animation on offset change 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollapseLayout(object sender, RoutedEventArgs e)
        {
            foreach (var child in _root.Children)
            {
                // Change the Offset property of the visual. This will trigger the implicit animation that is associated with the Offset change.
                // Define the same position for each visual
                child.Offset = new Vector3(_posX, _posY, 0);
            }
        }
        /// <summary>
        /// This method implictly kicks off animation on scale change 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Scale(object sender, RoutedEventArgs e)
        {
            foreach (var child in _root.Children)
            {
                if (child.Scale == Vector3.One)
                {
                    // Change the scale property of the visual will trigger the 
                    // implicit animation that is associated with the scale change
                    child.Scale = new Vector3(0.5f, 0.5f, 1.0f);
                }
                else
                {
                    child.Scale = Vector3.One;
                }
            }
        }

        /// <summary>
        /// Creates offset animation that can be applied to a visual
        /// </summary>
        Vector3KeyFrameAnimation CreateOffsetAnimation()
        {
            var _offsetKeyFrameAnimation = _compositor.CreateVector3KeyFrameAnimation();
            _offsetKeyFrameAnimation.Target = "Offset";
            
            // Final Value signifies the target value to which the visual will animate
            // in this case it will be defined by new offset
            _offsetKeyFrameAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            _offsetKeyFrameAnimation.Duration = TimeSpan.FromSeconds(3);

            return _offsetKeyFrameAnimation;
        }

        /// <summary>
        /// Creates scale animation that can be applied to a visual
        /// </summary>
        CompositionAnimationGroup CreateScaleAnimation()
        {
            var scaleKeyFrameAnimation = _compositor.CreateVector3KeyFrameAnimation();
            scaleKeyFrameAnimation.Target = "Scale";
            scaleKeyFrameAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
            scaleKeyFrameAnimation.Duration = TimeSpan.FromSeconds(3);

            var rotationAnimation = _compositor.CreateScalarKeyFrameAnimation();
            rotationAnimation.Target = "RotationAngle";
            rotationAnimation.InsertExpressionKeyFrame(1.0f, "this.StartingValue + 0.78539816");
            rotationAnimation.Duration = TimeSpan.FromSeconds(3);

            var animationGroup = _compositor.CreateAnimationGroup();

            // AnimationGroup associates the animations with the target
            animationGroup.Add(scaleKeyFrameAnimation);
            animationGroup.Add(rotationAnimation);
            
            return animationGroup;
        }

        private void UpdateAnimationState(bool animate)
        {
            if (animate)
            {
                ImplicitAnimationCollection implicitAnimationCollection = _compositor.CreateImplicitAnimationCollection();
                implicitAnimationCollection["Offset"] = CreateOffsetAnimation();
                implicitAnimationCollection["Scale"] = CreateScaleAnimation();
                foreach (var child in _root.Children)
                {
                    child.ImplicitAnimations = implicitAnimationCollection;
                }
            }
            else
            {
                foreach (var child in _root.Children)
                {
                    child.ImplicitAnimations = null;
                }
            }
        }

        private void ImplicitAnimationTransformer_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach(var surface in _imageList)
            {
                surface.Dispose();
            }
        }

        private void EnableAnimations_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
        {
            if (_compositor != null)
            {
                UpdateAnimationState(EnableAnimations.IsChecked ?? false);
            }
        }
    }
}
