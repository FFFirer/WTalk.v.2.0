﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WTalk.Domain;

namespace WTalk.Client
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class WaitShowConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            if (TargetType != typeof(Visibility)) { return null; }
            int num = int.Parse(value.ToString());
            return (num == 0 ? Visibility.Collapsed : Visibility.Visible);

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Status), typeof(Visibility))]
    public class StatusShowConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            if (TargetType != typeof(Visibility)) { return null; }
            Status status = (Status)value;
            return (status == Status.Waiting ? Visibility.Visible : Visibility.Collapsed);

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }
    }
}
