using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace PL
{
    public sealed class ConverterDoubleToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Double val = (Double)value;
            return (int)val;

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
    public sealed class ConverterLatitude : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Location val = (BO.Location)value;
            return $"({val })";

        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
    public sealed class BatteryToProgressBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (double)value * 100;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
    internal class BatteryToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (double)value switch
            {
                < 10 => System.Windows.Media.Brushes.DarkRed,
                < 20 => Brushes.Red,
                < 40 => Brushes.Yellow,
                < 60 => Brushes.GreenYellow,
                _ => Brushes.Green
            };
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
    internal class KilometerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Double val = (Double)value;
            if (val == 0)
                return "(no next stop scheduled yet)";
            return $"({val.ToString()[0]}Km to next stop)";

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();
    }
    internal class LocationtoMapLocation : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Location val = (BO.Location)value;
            Pushpin pin = new();
            pin.Location.Latitude = val.Latitude;
            pin.Location.Longitude = val.Longitude;
            return val;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();
    }
    internal class DeliveryIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((int)value == 0)
            {
                return "no next parcel scheduled yet";

            }
            return $"next parcel is:{(int)value}";

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();
    }
    public enum DroneStatuses { Vacant, Maintenance, InDelivery }
    internal class statustoBoutton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((string)value == "Vacant")
            {
                return "Send to charge";

            }
            else if ((string)value == "Maintenance")
            {
                return "Release to charge";
            }
            else
            {
                return "Impossible to charge";
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();
    }
    public class ItemToColorConvert : IValueConverter
    {
        //this converts the item from your data source to the color brush
        //of the background of the row
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //cast the value parameter to the type of item in your data source
            Random RAND = new();
           double r = RAND.NextDouble();
            if (r>0.1) //some condition you want to use to choose the color
            {
                //highlight
                return Brushes.Red;
            }
            else if (r>0.2)
            {
                //leave no background
                return Brushes.Blue;
            }
            else if (r > 0.3)
            {
                //leave no background
                return Brushes.Yellow;
            }
            else if (r > 0.5)
            {
                //leave no background
                return Brushes.Green;
            }
            else if (r > 0.7)
            {
                //leave no background
                return Brushes.Beige;
            }
            else if (r > 0.9)
            {
                //leave no background
                return Brushes.Turquoise;
            }
            else
            {
                //leave no background
                return Brushes.Transparent;
            }
   
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();

    }
}
