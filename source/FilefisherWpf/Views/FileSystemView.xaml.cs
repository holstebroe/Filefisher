using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using FilefisherWpf.ViewModels;

namespace FilefisherWpf.Views
{
    /// <summary>
    ///     Interaction logic for FileSystemView.xaml
    /// </summary>
    public partial class FileSystemView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel",
            typeof(FileSystemViewModel), typeof(FileSystemView),
            new PropertyMetadata(default(FileSystemViewModel), PropertyChangedCallback));

        public FileSystemView()
        {
            InitializeComponent();
        }

        public FileSystemViewModel ViewModel
        {
            get => (FileSystemViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (FileSystemView) d;
            var oldViewModel = e.OldValue as FileSystemViewModel;
            var newViewModel = e.NewValue as FileSystemViewModel;
            if (oldViewModel != null) oldViewModel.PropertyChanged -= view.ViewModelPropertyChanged;
            if (newViewModel != null) newViewModel.PropertyChanged += view.ViewModelPropertyChanged;
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;
            if (ViewModel?.SelectedDescriptor != treeView.SelectedItem && ViewModel.SelectedDescriptor != null)
                if (treeView.ItemContainerGenerator.ContainerFromItem(ViewModel.SelectedDescriptor) is TreeViewItem tvi)
                    tvi.IsSelected = true;
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ViewModel == null) return;
            ViewModel.SelectedDescriptor = (FileDescriptorViewModel) e.NewValue;
        }
    }
}