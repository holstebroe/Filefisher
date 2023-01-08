using System.Windows;
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    /// Interaction logic for ScanView.xaml
    /// </summary>
    public partial class ScanView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ScanViewModel), typeof(ScanView), new PropertyMetadata(new ScanViewModel()));

        public ScanView()
        {
            InitializeComponent();
        }

        public ScanViewModel ViewModel

        {
            get => (ScanViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}
