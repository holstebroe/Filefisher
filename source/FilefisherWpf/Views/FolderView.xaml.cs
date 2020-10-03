using System.Windows;
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    ///     Interaction logic for FolderView.xaml
    /// </summary>
    public partial class FolderView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
            typeof(FolderViewModel), typeof(FolderView), new PropertyMetadata(default(FolderViewModel)));

        public FolderView()
        {
            InitializeComponent();
        }

        public FolderViewModel ViewModel
        {
            get => (FolderViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}