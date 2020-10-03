using System.Windows;
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    ///     Interaction logic for FileView.xaml
    /// </summary>
    public partial class FileView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
            typeof(FileViewModel), typeof(FileView), new PropertyMetadata(default(FileViewModel)));

        public FileView()
        {
            InitializeComponent();
        }

        public FileViewModel ViewModel
        {
            get => (FileViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}