using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using MaterialDesignThemes.Wpf;

namespace Stutton.DocumentCreator.Views
{
    public static class ShadowAnimationAssist
    {
        public static readonly DependencyProperty AnimateShadowProperty =
            DependencyProperty.RegisterAttached("AnimateShadow", typeof(bool), typeof(ShadowAnimationAssist),
                new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsRender,
                    AnimateShadowPropertyChangedCallback));

        public static readonly DependencyProperty AnimateToShadowProperty =
            DependencyProperty.RegisterAttached("AnimateToShadow", typeof(ShadowDepth), typeof(ShadowAnimationAssist),
                new FrameworkPropertyMetadata(default(ShadowDepth), FrameworkPropertyMetadataOptions.AffectsRender));

        private static readonly DependencyPropertyKey LocalAnimationInfoPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("LocalAnimationInfo", typeof(ShadowAnimationLocalInfo),
                typeof(ShadowAnimationAssist), new PropertyMetadata(default(ShadowAnimationLocalInfo)));

        private static readonly Dictionary<ShadowDepth, DropShadowEffect> ShadowDictionary =
            new Dictionary<ShadowDepth, DropShadowEffect>
            {
                {ShadowDepth.Depth0, new DropShadowEffect {ShadowDepth = 0, BlurRadius = 0}},
                {ShadowDepth.Depth1, new DropShadowEffect {ShadowDepth = 1, BlurRadius = 5}},
                {ShadowDepth.Depth2, new DropShadowEffect {ShadowDepth = 1.5, BlurRadius = 8}},
                {ShadowDepth.Depth3, new DropShadowEffect {ShadowDepth = 4.5, BlurRadius = 14}},
                {ShadowDepth.Depth4, new DropShadowEffect {ShadowDepth = 8, BlurRadius = 25}},
                {ShadowDepth.Depth5, new DropShadowEffect {ShadowDepth = 13, BlurRadius = 35}}
            };

        public static bool GetAnimateShadow(DependencyObject element)
        {
            return (bool) element.GetValue(AnimateShadowProperty);
        }

        public static ShadowDepth GetAnimateToShadow(DependencyObject obj)
        {
            return (ShadowDepth) obj.GetValue(AnimateToShadowProperty);
        }

        public static void SetAnimateShadow(DependencyObject element, bool value)
        {
            element.SetValue(AnimateShadowProperty, value);
        }

        public static void SetAnimateToShadow(DependencyObject obj, ShadowDepth value)
        {
            obj.SetValue(AnimateToShadowProperty, value);
        }

        private static void AnimateShadowPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var uiElement = dependencyObject as UIElement;
            var dropShadowEffect = uiElement?.Effect as DropShadowEffect;

            if (dropShadowEffect == null) return;

            var toShadowDepth = GetAnimateToShadow(dependencyObject);
            var toDropShadowEffect = ShadowDictionary[toShadowDepth];

            if ((bool) dependencyPropertyChangedEventArgs.NewValue)
            {
                SetLocalAnimationInfo(dependencyObject,
                    new ShadowAnimationLocalInfo(dropShadowEffect.ShadowDepth, dropShadowEffect.BlurRadius));
                var shadowDepthAnimation = new DoubleAnimation(toDropShadowEffect.ShadowDepth,
                    new Duration(TimeSpan.FromMilliseconds(350)))
                {
                    FillBehavior = FillBehavior.HoldEnd
                };
                var blurRadiusAnimation = new DoubleAnimation(toDropShadowEffect.BlurRadius,
                    new Duration(TimeSpan.FromMilliseconds(350)))
                {
                    FillBehavior = FillBehavior.HoldEnd
                };
                dropShadowEffect.BeginAnimation(DropShadowEffect.ShadowDepthProperty, shadowDepthAnimation);
                dropShadowEffect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, blurRadiusAnimation);
            }
            else
            {
                var shadowAnimationLocalInfo = GetLocalAnimationInfo(dependencyObject);
                if (shadowAnimationLocalInfo == null) return;

                var shadowDepthAnimation = new DoubleAnimation(shadowAnimationLocalInfo.ShadowDepth,
                    new Duration(TimeSpan.FromMilliseconds(350)))
                {
                    FillBehavior = FillBehavior.HoldEnd
                };
                var blurRadiusAnimation = new DoubleAnimation(shadowAnimationLocalInfo.BlurRadius,
                    new Duration(TimeSpan.FromMilliseconds(350)))
                {
                    FillBehavior = FillBehavior.HoldEnd
                };
                dropShadowEffect.BeginAnimation(DropShadowEffect.ShadowDepthProperty, shadowDepthAnimation);
                dropShadowEffect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, blurRadiusAnimation);
            }
        }

        private static ShadowAnimationLocalInfo GetLocalAnimationInfo(DependencyObject element)
        {
            return (ShadowAnimationLocalInfo) element.GetValue(LocalAnimationInfoPropertyKey.DependencyProperty);
        }

        private static void SetLocalAnimationInfo(DependencyObject element, ShadowAnimationLocalInfo value)
        {
            element.SetValue(LocalAnimationInfoPropertyKey, value);
        }
    }

    internal class ShadowAnimationLocalInfo
    {
        public ShadowAnimationLocalInfo(double shadowDepth, double blurRadius)
        {
            ShadowDepth = shadowDepth;
            BlurRadius = blurRadius;
        }

        public double BlurRadius { get; }
        public double ShadowDepth { get; }
    }
}