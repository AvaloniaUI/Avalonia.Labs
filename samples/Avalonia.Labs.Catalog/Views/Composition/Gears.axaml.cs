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
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using ExpressionBuilder;

namespace Avalonia.Labs.Catalog.Views
{
    public partial class Gears : UserControl, INotifyPropertyChanged
    {
        private Compositor? _compositor;
        private List<CompositionVisual>? _gearVisuals;
        private ScalarKeyFrameAnimation? _gearMotionScalarAnimation;
        private double _x = 87, _y = 0d, _width = 100, _height = 100;
        private double _gearDimension = 87;
        private int _count;

        public new event PropertyChangedEventHandler? PropertyChanged;

        public Gears()
        {
            InitializeComponent();
            DataContext = this;

            SharedBitmap = new Bitmap(AssetLoader.Open(new Uri("avares://Avalonia.Labs.Catalog/Assets/Composition/Gear.png")));
        }

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChanged();
            }
        }

        public Bitmap SharedBitmap { get; }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            _compositor = ElementComposition.GetElementVisual(this)!.Compositor;
            Setup();
        }

        private void Setup()
        {
            var firstGearVisual = ElementComposition.GetElementVisual(FirstGear)!;
            firstGearVisual.Size = new Vector2((float)FirstGear.Bounds.Width, (float)FirstGear.Bounds.Height);
            firstGearVisual.AnchorPoint = new Vector2(0.5f, 0.5f);

            for (int i = Container.Children.Count - 1; i > 0; i--)
            {
                Container.Children.RemoveAt(i);
            }

            _x = 87;
            _y = 0d;
            _width = 100;
            _height = 100;
            _gearDimension = 87;

            Count = 1;
            _gearVisuals = new List<CompositionVisual>() { firstGearVisual };
        }

        private void AddGear_Click(object sender, RoutedEventArgs e)
        {
            // Create an image
            var image = new Image
            {
                Source = SharedBitmap,
                Width = _width,
                Height = _height,
                RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative)
            };

            // Set the coordinates of where the image should be
            Canvas.SetLeft(image, _x);
            Canvas.SetTop(image, _y);

            PerformLayoutCalculation();

            // Add the gear to the container
            Container.Children.Add(image);

            // Add a gear visual to the screen
            var gearVisual = AddGear(image);

            ConfigureGearAnimation(_gearVisuals![_gearVisuals.Count - 1], _gearVisuals[_gearVisuals.Count - 2]);
        }

        private CompositionVisual AddGear(Image gear)
        {
            // Create a visual based on the XAML object
            var visual = ElementComposition.GetElementVisual(gear)!;
            visual.Size = new Vector2((float)gear.Bounds.Width, (float)gear.Bounds.Height);
            visual.AnchorPoint = new Vector2(0.5f, 0.5f);
            _gearVisuals!.Add(visual);

            Count++;

            return visual;
        }

        private void ConfigureGearAnimation(CompositionVisual currentGear, CompositionVisual previousGear)
        {
            // If rotation expression is null then create an expression of a gear rotating the opposite direction

            var rotateExpression = -previousGear.GetReference().RotationAngle;

            // Start the animation based on the Rotation Angle.
            currentGear.StartAnimation("RotationAngle", rotateExpression);

            var rotationExpression = _compositor.CreateExpressionAnimation("-previousGear.RotationAngle");
            rotationExpression.SetReferenceParameter("previousGear", previousGear);
            currentGear.StartAnimation("RotationAngle", rotationExpression);

            // To add same animation with no angle offset
            if (_gearMotionScalarAnimation is not null)
            {
                //currentGear.StartAnimation("RotationAngle", _gearMotionScalarAnimation);
            }
        }

        private void StartGearMotor(double secondsPerRotation)
        {
            // Start the first gear (the red one)
            if (_gearMotionScalarAnimation == null)
            {
                _gearMotionScalarAnimation = _compositor!.CreateScalarKeyFrameAnimation();
                var linear = new LinearEasing();

                //_gearMotionScalarAnimation.InsertExpressionKeyFrame(0.0f, "this.StartingValue");
                //_gearMotionScalarAnimation.InsertExpressionKeyFrame(1.0f, "this.StartingValue + 6.28318530717959", linear);
                var startingValue = ExpressionValues.StartingValue.CreateScalarStartingValue();
                _gearMotionScalarAnimation.InsertExpressionKeyFrame(0.0f, startingValue);
                _gearMotionScalarAnimation.InsertExpressionKeyFrame(1.0f, startingValue + Math.PI * 2, linear);

                _gearMotionScalarAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
            }

            _gearMotionScalarAnimation.Duration = TimeSpan.FromSeconds(secondsPerRotation);
            _gearVisuals!.First().StartAnimation("RotationAngle", _gearMotionScalarAnimation);
        }

        private void AnimateFast_Click(object sender, RoutedEventArgs e)
        {
            // Setup and start the animation on the red gear.
            StartGearMotor(1);
        }

        private void AnimateSlow_Click(object sender, RoutedEventArgs e)
        {
            // Setup and start the animation on the red gear.
            StartGearMotor(5);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            // _gearVisuals.First().StopAnimation("RotationAngleInDegrees");
        }

        private void Reverse_Click(object sender, RoutedEventArgs e)
        {
            if (_gearMotionScalarAnimation!.Direction == PlaybackDirection.Normal)
            {
                _gearMotionScalarAnimation.Direction = PlaybackDirection.Reverse;
            }
            else
            {
                _gearMotionScalarAnimation.Direction = PlaybackDirection.Normal;
            }

            _gearVisuals!.First().StartAnimation("RotationAngle", _gearMotionScalarAnimation);
        }

        private void AddXGearsButton_Click(object sender, RoutedEventArgs e)
        {
            int gearsToAdd;

            if (int.TryParse(NumberOfGears.Text, out gearsToAdd))
            {
                int amount = gearsToAdd + _gearVisuals.Count - 1;
                Setup();

                var maxAreaPerTile = Math.Sqrt((Container.Bounds.Width * Container.Bounds.Height) / (amount + Container.Children.Count));

                if (maxAreaPerTile < _width)
                {
                    var wholeTilesHeight = Math.Floor(Container.Bounds.Height / maxAreaPerTile);
                    var wholeTileWidth = Math.Floor(Container.Bounds.Width / maxAreaPerTile);

                    FirstGear.Width = FirstGear.Height = maxAreaPerTile;
                    _width = _height = maxAreaPerTile;

                    _x = _gearDimension = _width * 0.87;
                }

                for (int i = 0; i < amount; i++)
                {
                    AddGear_Click(sender, e);
                }
            }
        }

        private void PerformLayoutCalculation()
        {
            if (
                ((_x + Container.Margin.Left + _width > Container.Bounds.Width) && _gearDimension > 0) ||
                (_x < Container.Margin.Left && _gearDimension < 0))
            {
                if (_gearDimension < 0)
                {
                    _y -= _gearDimension;
                }
                else
                {
                    _y += _gearDimension;
                }
                _gearDimension = -_gearDimension;
            }
            else
            {
                _x += _gearDimension;
            }
        }

        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
