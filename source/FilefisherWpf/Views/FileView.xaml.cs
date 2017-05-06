using System.Windows;
using System.Windows.Controls;
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    /// Interaction logic for FileView.xaml
    /// </summary>
    public partial class FileView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(FileViewModel), typeof(FileView), new PropertyMetadata(default(FileViewModel)));

        public FileView()
        {
            InitializeComponent();
        }

        public FileViewModel ViewModel
        {
            get { return (FileViewModel) GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
