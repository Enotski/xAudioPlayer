using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace xAudioPlayer.Services {
    public static class AnimationExtensions {
        public static Task<bool> ChangeBackgroundColorTo(this Button self, Color newColor, uint length = 250, Easing easing = null) {
            Task<bool> ret = new Task<bool>(() => false);

            if (!self.AnimationIsRunning(nameof(ChangeBackgroundColorTo))) {
                Color fromColor = self.BackgroundColor;

                try {
                    Func<double, Color> transform = (t) =>
                      Color.FromRgba(fromColor.R + t * (newColor.R - fromColor.R),
                                     fromColor.G + t * (newColor.G - fromColor.G),
                                     fromColor.B + t * (newColor.B - fromColor.B),
                                     fromColor.A + t * (newColor.A - fromColor.A));

                    ret = TransmuteBackgroundColorAnimation(self, nameof(ChangeBackgroundColorTo), transform, length, easing);
                } catch (Exception ex) {
                    // to supress animation overlapping errors 
                    self.BackgroundColor = fromColor;
                }
            }

            return ret;
        }
        public static Task<bool> ChangeTextColorTo(this Button self, Color newColor, uint length = 250, Easing easing = null) {
            Task<bool> ret = new Task<bool>(() => false);

            if (!self.AnimationIsRunning(nameof(ChangeTextColorTo))) {
                Color fromColor = self.TextColor;

                try {
                    Func<double, Color> transform = (t) =>
                      Color.FromRgba(fromColor.R + t * (newColor.R - fromColor.R),
                                     fromColor.G + t * (newColor.G - fromColor.G),
                                     fromColor.B + t * (newColor.B - fromColor.B),
                                     fromColor.A + t * (newColor.A - fromColor.A));

                    ret = TransmuteTextColorAnimation(self, nameof(ChangeTextColorTo), transform, length, easing);
                } catch (Exception ex) {
                    // to supress animation overlapping errors 
                    self.TextColor = fromColor;
                }
            }

            return ret;
        }
        private static Task<bool> TransmuteBackgroundColorAnimation(Button button, string name, Func<double, Color> transform, uint length, Easing easing) {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            button.Animate(name, transform, (color) => { button.BackgroundColor = color; }, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
            return taskCompletionSource.Task;
        }
        private static Task<bool> TransmuteTextColorAnimation(Button button, string name, Func<double, Color> transform, uint length, Easing easing) {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            button.Animate(name, transform, (color) => { button.TextColor = color; }, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
            return taskCompletionSource.Task;
        }
    }
}
