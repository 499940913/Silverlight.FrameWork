using System;
using System.Windows;
namespace BaseTool
{
    public class ThicknessWrapper : FrameworkElement
    {

        public FrameworkElement Target
        {
            get
            {
                return (FrameworkElement)GetValue(TargetProperty);
            }
            set
            {
                SetValue(TargetProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(ThicknessWrapper), new PropertyMetadata(null, OnTargetChanged));


        static void OnTargetChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var sender = (ThicknessWrapper)source;
            sender.UpdateMargin();
        }




        public String PropertyName
        {
            get
            {
                return (String)GetValue(PropertyNameProperty);
            }
            set
            {
                SetValue(PropertyNameProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(String), typeof(ThicknessWrapper), new PropertyMetadata("Margin"));



        public Side Side
        {
            get
            {
                return (Side)GetValue(SideProperty);
            }
            set
            {
                SetValue(SideProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Side.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SideProperty =
            DependencyProperty.Register("Side", typeof(Side), typeof(ThicknessWrapper), new PropertyMetadata(Side.Left, OnSideChanged));


        static void OnSideChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var sender = (ThicknessWrapper)source;
            sender.UpdateMargin();
        }


        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ThicknessWrapper), new PropertyMetadata(0.0, OnValueChanged));


        static void OnValueChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var sender = (ThicknessWrapper)source;
            sender.UpdateMargin();
        }

        private void UpdateMargin()
        {
            if (Target != null)
            {
                var thicknessProperty = Target.GetType().GetProperty(PropertyName);
                var currentThickness = (Thickness)thicknessProperty.GetValue(Target, null);
                var nextThickness = new Thickness(
                    CalculateThickness(Side.Left, currentThickness.Left),
                    CalculateThickness(Side.Top, currentThickness.Top),
                    CalculateThickness(Side.Right, currentThickness.Right),
                    CalculateThickness(Side.Bottom, currentThickness.Bottom)
                    );

                thicknessProperty.SetValue(Target, nextThickness, null);
            }
        }

        private double CalculateThickness(Side sideToCalculate, double currentValue)
        {
            return (Side & sideToCalculate) == sideToCalculate ? Value : currentValue;
        }


    }

    [Flags]
    public enum Side
    {
        Left = 1,
        Top = 2,
        Right = 4,
        Bottom = 8,
        All = 15
    }
}
