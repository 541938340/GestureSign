using System;
using System.Globalization;
using System.Windows.Data;
using GestureSign.Common.Applications;
using GestureSign.ControlPanel.Common;

namespace GestureSign.ControlPanel.Converters
{
    public class ListviewItem2TextBoxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values[0] == null || values[1] == null) return Binding.DoNothing;
            var matchUsing = (MatchUsing)values[0];
            ApplicationListViewItem applicationListViewItem = values[1] as ApplicationListViewItem;
            switch (matchUsing)
            {
                case MatchUsing.WindowClass:
                    return applicationListViewItem.WindowClass;

                case MatchUsing.WindowTitle:
                    return applicationListViewItem.WindowTitle;

                case MatchUsing.ExecutableFilename:
                    return applicationListViewItem.WindowFilename;
            }
            return Binding.DoNothing;
        }
        // ��Ϊ��ֻ������Դ��Ŀ�������Binding�����ԣ����������ԶҲ���ᱻ����
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { Binding.DoNothing, Binding.DoNothing };
        }
    }
}