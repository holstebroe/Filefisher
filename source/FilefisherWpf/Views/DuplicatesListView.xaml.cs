using System.Windows;
using System.Windows.Controls;
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    /// Interaction logic for DuplicatesListView.xaml
    /// </summary>
    public partial class DuplicatesListView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(FileViewModel), typeof(DuplicatesListView), new PropertyMetadata(default(FileViewModel)));

        public DuplicatesListView()
        {
            InitializeComponent();
        }

        public FileViewModel ViewModel
        {
            get => (FileViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}
