using System;
using System.Globalization;
using System.Windows.Data;
using GestureSign.Common.Applications;
using GestureSign.ControlPanel.Common;

namespace GestureSign.ControlPanel.Converters
{
    public class SelectedItemConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool isExisting = values != null && (bool)values[0];
            IApplication existingApplication = values[1] as IApplication;
            ApplicationListViewItem applicationListViewItem = values[2] as ApplicationListViewItem;
            if (!isExisting)
                return applicationListViewItem == null
                    ? Binding.DoNothing
                    : applicationListViewItem.WindowTitle;
            return existingApplication != null ? existingApplication.Name : Binding.DoNothing;
        }
        // ��Ϊ��ֻ������Դ��Ŀ�������Binding�����ԣ����������ԶҲ���ᱻ����
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { Binding.DoNothing, Binding.DoNothing, Binding.DoNothing };
        }
    }
}