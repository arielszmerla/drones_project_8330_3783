using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace PL
{

    /// <summary>
    /// converters double to int
    /// </summary>
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
    /// <summary>
    /// converters Location to nice string
    /// </summary>
    public sealed class ConverterLatitude : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BO.Location val = (BO.Location)value;
            return $"({val})";

        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// converters Battery to "num %"
    /// </summary>
    public sealed class BatteryToProgressBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Double val = (Double)value;
            return $"{ (int)val} %";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    /// <summary>
    /// set colour upon value of baterry
    /// </summary>
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
    /// <summary>
    /// show km to next stop if any 
    /// </summary>
    internal class KilometerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            Double val = (Double)value;
            if (val == 0)
                return "(no next stop scheduled yet)";
            return $"({val.ToString()[0]} Km to next stop)";

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();
    }
    /// <summary>
    /// set visibilty of label upon valiue i=of delivery item
    /// </summary>
    internal class Deliveryidconvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int val = (int)value;
            if (val == 0)
                return Visibility.Collapsed;
            return Visibility.Visible ;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();
    }
    /// <summary>
    /// set location on map
    /// </summary>
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
    /// <summary>
    /// nice string show of 
    /// </summary>delivery parcel id if any
    internal class DeliveryIdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null )
                return "no parcel";
            else if( (int)value == 0)
                 return $"no parcel assigned";
            else return $"Parcel: {(int)value}";

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
                throw new NotImplementedException();
    }
    public enum DroneStatuses { Vacant, Maintenance, InDelivery }
    /// <summary>
    /// set content of a button upon drone status
    /// </summary>
    internal class statustoBoutton : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((string)value == "Vacant")
            {
                return "Recharge";

            }
            else if ((string)value == "Maintenance")
            {
                return "Release";
            }
            else
            {
                return "Impossible";
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
