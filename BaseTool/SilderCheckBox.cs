using System.Windows;
using System.Windows.Controls;

namespace BaseTool
{
    public class SilderCheckBox:CheckBox
    {
        public SilderCheckBox()
        {
            DefaultStyleKey = typeof(SilderCheckBox);

        }

        public static DependencyProperty EllipseDiameterProperty =
             DependencyProperty.Register("EllipseDiameter", typeof(double), typeof(SilderCheckBox), null);

        public double EllipseDiameter
        {
            get
            {
                return (double)GetValue(EllipseDiameterProperty);
            }
            set
            {
                SetValue(EllipseDiameterProperty, value);
            }
        }

        public static DependencyProperty MoveToProperty =
             DependencyProperty.Register("MoveTo", typeof(double), typeof(SilderCheckBox), null);

        public double MoveTo
        {
            get
            {
                return (double)GetValue(MoveToProperty);
            }
            set
            {
                SetValue(MoveToProperty, value);
            }
        }


        public static DependencyProperty RectRadiusProperty =
             DependencyProperty.Register("RectRadius", typeof(double), typeof(SilderCheckBox), null);

        public double RectRadius
        {
            get
            {
                return (double)GetValue(RectRadiusProperty);
            }
            set
            {
                SetValue(RectRadiusProperty, value);
            }
        }

        //Radius
    }
}
