using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace GI
{
    public class Anim
    {
        public static async Task FadeInAsync(UIElement element, double duration)
        {
            DoubleAnimation fadeIn = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(duration)
            };
            element.Visibility = Visibility.Visible;
            element.BeginAnimation(UIElement.OpacityProperty, fadeIn);

            await Task.Delay((int)(duration * 1000));
        }

        public static async Task FadeOutAsync(UIElement element, double duration)
        {
            DoubleAnimation fadeOut = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(duration)
            };
            element.BeginAnimation(UIElement.OpacityProperty, fadeOut);

            await Task.Delay((int)(duration * 1000));
            element.Visibility = Visibility.Collapsed;
        }

        public static async Task FadeOut2Async(UIElement element, double durationSeconds = 0.5)
        {
            if (element == null) return;

            var storyboard = new Storyboard();
            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(durationSeconds)
            };
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));

            storyboard.Completed += (s, e) => element.Visibility = Visibility.Collapsed;

            storyboard.Begin();

            await Task.Delay(TimeSpan.FromSeconds(durationSeconds));
        }
    }
}