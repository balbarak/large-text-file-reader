using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ltfr.App
{
    /// <summary>
    /// Interaction logic for FileReaderView.xaml
    /// </summary>
    public partial class FileReaderView : UserControl
    {
        private bool _autoScroll;

        private ScrollViewer _scrollViewer;
        public FileReaderView()
        {
            InitializeComponent();

            var viewModel = DataContext as FileReaderViewModel;

            viewModel.Lines.CollectionChanged += OnTextLinesChanged;
        }


        private void OnTextLinesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (_autoScroll)
            {
                _scrollViewer.ScrollToBottom();
            }

           
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0)
            {
                if (e.VerticalOffset == _scrollViewer.ScrollableHeight - e.ExtentHeightChange)
                {
                    _autoScroll = true;
                }
                else
                {
                    _autoScroll = false;
                }
            }
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = (Border)VisualTreeHelper.GetChild(listBox, 0);
            _scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
        }
    }
}
