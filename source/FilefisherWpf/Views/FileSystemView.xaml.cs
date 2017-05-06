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
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    /// Interaction logic for FileSystemView.xaml
    /// </summary>
    public partial class FileSystemView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(FileSystemViewModel), typeof(FileSystemView), new PropertyMetadata(default(FileSystemViewModel)));

        public FileSystemView()
        {
            InitializeComponent();
        }

        public FileSystemViewModel ViewModel
        {
            get { return (FileSystemViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ViewModel == null) return;
            ViewModel.SelectedDescriptor = (FileDescriptorViewModel) e.NewValue;
        }


    }
}
