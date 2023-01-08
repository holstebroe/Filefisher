using System.Windows;
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    ///     Interaction logic for BrowseView.xaml
    /// </summary>
    public partial class BrowseView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(BrowseViewModel), typeof(BrowseView), new PropertyMetadata(new BrowseViewModel()));

        public BrowseView()
        {
            InitializeComponent();
        }

        public BrowseViewModel ViewModel
        {
            get => (BrowseViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}