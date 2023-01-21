using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FilefisherWpf.ViewModels;
using FileScanner;

namespace FilefisherWpf.Views
{
    /// <summary>
    ///     Interaction logic for FileSystemView.xaml
    /// </summary>
    public partial class FileSystemView
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel),
            typeof(FileSystemViewModel), typeof(FileSystemView),
            new PropertyMetadata(default(FileSystemViewModel), PropertyChangedCallback));

        public static readonly DependencyProperty SelectedDescriptorProperty = DependencyProperty.Register(nameof(SelectedDescriptor),
            typeof(FileDescriptor), typeof(FileSystemView),
            new PropertyMetadata(default(FileDescriptor), SelectedDescriptorChangedCallback));

        public FileSystemView()
        {
            InitializeComponent();
        }

        public FileSystemViewModel ViewModel
        {
            get => (FileSystemViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        
        public FileDescriptor SelectedDescriptor
        {
            get => (FileDescriptor) GetValue(SelectedDescriptorProperty);
            set => SetValue(SelectedDescriptorProperty, value);
        }
        
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (FileSystemView) d;
            if (e.OldValue is FileSystemViewModel oldViewModel) oldViewModel.PropertyChanged -= view.ViewModelPropertyChanged;
            if (e.NewValue is FileSystemViewModel newViewModel) newViewModel.PropertyChanged += view.ViewModelPropertyChanged;
        }
        private static void SelectedDescriptorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (FileSystemView) d;
            var descriptor = (FileDescriptor)e.NewValue;
            //public void Select(FileDescriptor descriptor)
            //{
            //    descriptorLookup.LookupByStat(descriptor).FirstOrDefault(x => x.FullPath == descriptor.FullPath);
            //}
            view.ViewModel.Select(descriptor);
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null) return;
            if (ViewModel?.SelectedDescriptor != treeView.SelectedItem && ViewModel?.SelectedDescriptor != null)
                if (treeView.ItemContainerGenerator.ContainerFromItem(ViewModel.SelectedDescriptor) is TreeViewItem tvi)
                    tvi.IsSelected = true;
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ViewModel == null) return;
            //SelectedDescriptor = ;
            ViewModel.SelectedDescriptor = (FileDescriptorViewModel)e.NewValue;
        }

        private void TreeView_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (ViewModel == null) return;
            if (e.Key == Key.Delete)
            {
                if (ViewModel.SelectedDescriptor is FileDescriptorViewModel fileDescriptorViewModel)
                {
                    var firstDuplicate = fileDescriptorViewModel.Duplicates.FirstOrDefault();
                    if (firstDuplicate != null)
                    {
                        if (firstDuplicate.DeleteCommand.CanExecute(null))
                        {
                            firstDuplicate.DeleteCommand.Execute(null);
                        }
                    }
                }
            }
        }

        private void TreeViewSelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is TreeViewItem item)) return;
            item.BringIntoView();
            e.Handled = true;
        }
    }
}