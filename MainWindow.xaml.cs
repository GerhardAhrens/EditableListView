namespace EditableListView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;

    using EditableListView.Core;
    using EditableListView.Extension;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private IEnumerable<string> developerList = null;
        private ICollectionView listviewSource = null;

        public MainWindow()
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnFilter, "Click", this.OnClickFilter);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnClearFilter, "Click", this.OnClickClearFilter);

            this.DataModel = new DemoData();
        }

        private DemoData DataModel { get; set; }

        public ICollectionView ListViewSource
        {
            get { return this.listviewSource; }
            set
            {
                if (this.listviewSource != value)
                {
                    this.listviewSource = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public IEnumerable<string> DeveloperList
        {
            get { return this.developerList; }
            set
            {
                if (this.developerList != value)
                {
                    this.developerList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DataTable dt = this.ListViewSource.ToDataTable<ViewItem>();
            List<ViewItem> aa = new List<ViewItem>();
            IEnumerable<ViewItemExport> ignor = aa.Select(s => new ViewItemExport { Id = s.Id, Name = s.Name });
            //this.lvItems.Focus();
        }

        private void OnClickFilter(object sender, RoutedEventArgs e)
        {
        }

        private void OnClickClearFilter(object sender, RoutedEventArgs e)
        {
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }
    }
}