using System.Windows;
using System.Windows.Controls;
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    /// Interaction logic for FileDescriptorView.xaml
    /// </summary>
    public partial class FileDescriptorView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(FileDescriptorViewModel), typeof(FileDescriptorView), new PropertyMetadata(default(FileDescriptorViewModel)));

        public FileDescriptorView()
        {
            InitializeComponent();
        }

        public FileDescriptorViewModel ViewModel
        {
            get => (FileDescriptorViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}
