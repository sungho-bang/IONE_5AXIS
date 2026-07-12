using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace FAFramework.ExtensionUIControls
{
    public enum ImageEffect
    {
        Blink,
        Hide,
        None
    }

    public class BlinkImage : Image
    {
        public class IntToDurationConverter : IValueConverter
        {
            public object Convert(object value, Type targetType,
                                  object parameter, CultureInfo culture)
            {
                try
                {
                    return new Duration(TimeSpan.FromMilliseconds((int)value));
                }
                catch
                {
                    return null;
                }
            }

            public object ConvertBack(object value, Type targetType,
                                      object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public static readonly DependencyProperty ImageEffectProperty = DependencyProperty.Register(
            "ImageEffect", typeof(ImageEffect), typeof(BlinkImage), new UIPropertyMetadata(ImageEffect.Blink));

        public ImageEffect ImageEffect
        {
            get { return (ImageEffect)this.GetValue(ImageEffectProperty); }
            set
            {
                this.SetValue(ImageEffectProperty, value);
            }
        }

        public static readonly DependencyProperty EffectOnProperty = DependencyProperty.Register(
            "EffectOn", typeof(bool), typeof(BlinkImage), new UIPropertyMetadata(false, EffectOnChanged));

        public bool EffectOn
        {
            get { return (bool)this.GetValue(EffectOnProperty); }
            set
            {
                this.SetValue(EffectOnProperty, value);
            }
        }

        public static readonly DependencyProperty BlinkIntervalProperty = DependencyProperty.Register(
            "BlinkInterval", typeof(int), typeof(BlinkImage), new UIPropertyMetadata(300, EffectIntervalChanged));

        public int BlinkInterval
        {
            get { return (int)this.GetValue(BlinkIntervalProperty); }
            set
            {
                this.SetValue(BlinkIntervalProperty, value);
            }
        }

        Storyboard _blinkStoryboard;
        object _lockObject = new object();

        public BlinkImage()
        {
        }

        private void StartBlink()
        {
            lock (_lockObject)
            {
                if (_blinkStoryboard == null)
                {
                    var animation = new DoubleAnimation();
                    animation.From = 0;
                    animation.To = 1;
                    _blinkStoryboard = new Storyboard();
                    _blinkStoryboard.AutoReverse = true;
                    _blinkStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                    _blinkStoryboard.Duration = new Duration(TimeSpan.FromMilliseconds(BlinkInterval));
                    _blinkStoryboard.Children.Add(animation);
                    Storyboard.SetTarget(animation, this);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
                    _blinkStoryboard.Begin();
                }
                else
                    _blinkStoryboard.Resume();
            }
        }

        private void StopBlink()
        {
            lock (_lockObject)
            {
                if (_blinkStoryboard != null)
                    _blinkStoryboard.Pause();
            }
        }

        private void SetBlinkInterval(int flickInterval)
        {
            if (_blinkStoryboard != null)
            {
                StopBlink();
                var currentTime = _blinkStoryboard.GetCurrentTime().TotalMilliseconds;
                var seekTime = TimeSpan.Zero;
                if (currentTime != 0)
                {
                    var ratio = flickInterval / BlinkInterval;
                    seekTime = TimeSpan.FromMilliseconds(currentTime * ratio);
                }

                _blinkStoryboard = null;
                StartBlink();
                _blinkStoryboard.Seek(seekTime);
            }
        }

        private static void EffectOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = d as BlinkImage;
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                    image.StartBlink();
                else
                    image.StopBlink();
            }
        }

        private static void EffectIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = d as BlinkImage;
            if (e.NewValue != e.OldValue)
            {
                image.SetBlinkInterval((int)e.NewValue);
            }
        }
    }
}
