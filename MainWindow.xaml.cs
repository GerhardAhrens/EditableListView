namespace EditableListView
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text.Json;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;

    using EditableListView.Core;
    using EditableListView.Extension;

    using Microsoft.VisualBasic;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const string DateFormat = "dd.MM.yyyy HH:mm";
        public event PropertyChangedEventHandler PropertyChanged;
        private DispatcherTimer statusBarDate = null;
        private IEnumerable<string> developerList = null;
        private IEnumerable<string> columns = null;
        private ICollectionView listviewSource = null;

        public MainWindow()
        {
            this.InitializeComponent();
            this.InitTimer();

            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);

            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnFilter, "Click", this.OnClickFilter);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnClearFilter, "Click", this.OnClickClearFilter);

            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnGroup, "Click", this.OnClickGroup);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnClearGroup, "Click", this.OnClickClearGroup);

            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnEvaluate, "Click", this.OnCurrentListViewItemClick);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnFirst, "Click", this.OnNavigationClick);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnBefore, "Click", this.OnNavigationClick);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnNext, "Click", this.OnNavigationClick);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnLast, "Click", this.OnNavigationClick);

            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnOpen, "Click", this.OnOpenClick);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.btnSave, "Click", this.OnSaveClick);

            WeakEventManager<MenuItem, RoutedEventArgs>.AddHandler(this.mnuCurrentRow, "Click", this.OnCurrentListViewItemClick);

            this.DataContext = this;
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

        public IEnumerable<string> Columns
        {
            get { return this.columns; }
            set
            {
                if (this.columns != value)
                {
                    this.columns = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.lvItems.Focus();

            this.DataModel = new DemoData();
            this.Columns = this.DataModel.Columns;
            this.ListViewSource = CollectionViewSource.GetDefaultView(this.DataModel.Items);
            this.DeveloperList = this.DataModel.AvailableDevelopment;

            int count = 0;
            if (ListViewSource != null)
            {
                count = this.ListViewSource.Cast<object>().Count();
            }

            StatusbarMain.Statusbar.SetNotification($"Bereit: Anzahl: {count}");
        }

        private void OnListViewHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader currentHeader = e.OriginalSource as GridViewColumnHeader;
            if (currentHeader != null && currentHeader.Role != GridViewColumnHeaderRole.Padding)
            {
                using (this.ListViewSource.DeferRefresh())
                {
                    Func<SortDescription, bool> lamda = item => item.PropertyName.Equals(currentHeader.Column.Header.ToString());
                    if (this.ListViewSource.SortDescriptions.Count(lamda) > 0)
                    {
                        SortDescription currentSortDescription = this.ListViewSource.SortDescriptions.First(lamda);
                        ListSortDirection sortDescription = currentSortDescription.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;


                        currentHeader.Column.HeaderTemplate = currentSortDescription.Direction == ListSortDirection.Ascending ?
                            this.Resources["HeaderTemplateArrowDown"] as DataTemplate : this.Resources["HeaderTemplateArrowUp"] as DataTemplate;

                        this.ListViewSource.SortDescriptions.Remove(currentSortDescription);
                        this.ListViewSource.SortDescriptions.Insert(0, new SortDescription(currentHeader.Column.Header.ToString(), sortDescription));
                    }
                    else
                        this.ListViewSource.SortDescriptions.Add(new SortDescription(currentHeader.Column.Header.ToString(), ListSortDirection.Ascending));
                }
            }
        }

        private void OnClickFilter(object sender, RoutedEventArgs e)
        {
            this.ListViewSource.Filter = item =>
            {
                ViewItem vitem = item as ViewItem;
                if (vitem == null)
                {
                    return false;
                }

                PropertyInfo info = item.GetType().GetProperty(cmbProperty.Text);
                if (info == null)
                {
                    return false;
                }

                return info.GetValue(vitem, null).ToString().ToLower().Contains(this.txtFilter.Text.ToLower());
            };

            this.ListViewSource.Refresh();
            int count = this.ListViewSource.Cast<object>().Count();
            StatusbarMain.Statusbar.SetNotification($"Bereit: Anzahl: {count}");
        }

        private void OnClickClearFilter(object sender, RoutedEventArgs e)
        {
            this.ListViewSource.Filter = item => true;
            this.ListViewSource.Refresh();
            int count = this.ListViewSource.Cast<object>().Count();
            StatusbarMain.Statusbar.SetNotification($"Bereit: Anzahl: {count}");
        }

        private void OnClickGroup(object sender, RoutedEventArgs e)
        {
            this.ListViewSource.GroupDescriptions.Clear();

            PropertyInfo pinfo = typeof(ViewItem).GetProperty(cmbGroups.Text);
            if (pinfo != null)
            {
                this.ListViewSource.GroupDescriptions.Add(new PropertyGroupDescription(pinfo.Name));
            }
        }

        private void OnClickClearGroup(object sender, RoutedEventArgs e)
        {
            this.ListViewSource.GroupDescriptions.Clear();
        }

        private void OnMouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            ListViewItem lvItem = sender as ListViewItem;
            ViewItem item = lvItem.Content as ViewItem;
            string msg = $"Hallo {item.Name}, Developer für {item.Developer} mit Gehalt von {item.Gehalt.ToString("C2")}; Status={item.Status}";
            MessageBox.Show(msg);
        }

        private void OnCurrentListViewItemClick(object sender, RoutedEventArgs e)
        {
            ViewItem item = this.lvItems.SelectedItem as ViewItem;

            if (item != null)
            {
                string msg = $"Hallo {item.Name}, Developer für {item.Developer} mit Gehalt von {item.Gehalt.ToString("C2")}; Status={item.Status}";
                MessageBox.Show(msg);
            }
        }

        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists("ListViewDemo.json") == true)
            {
                string jsonText = File.ReadAllText("ListViewDemo.json");
                ObservableCollection<ViewItem> list = JsonSerializer.Deserialize<ObservableCollection<ViewItem>>(jsonText);
                if (this.ListViewSource != null)
                {
                    using (this.ListViewSource.DeferRefresh())
                    {
                        this.lvItems.ItemsSource = null;
                        this.lvItems.Items.Clear();

                        this.ListViewSource = null;
                        this.ListViewSource = CollectionViewSource.GetDefaultView(list);
                        this.ListViewSource.Refresh();
                        this.lvItems.ItemsSource = this.ListViewSource;

                        int count = this.ListViewSource.Cast<object>().Count();
                        StatusbarMain.Statusbar.SetNotification($"Bereit: Anzahl: {count}");
                    }
                }
            }
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            string jsonText = JsonSerializer.Serialize<ICollectionView>(this.ListViewSource);
            File.WriteAllText("ListViewDemo.json", jsonText);
            MessageBox.Show("Aktuelle Ansich nach 'ListViewDemo.json' gespeichert", "Aktuelle Ansicht");
        }

        private void OnNavigationClick(object sender, RoutedEventArgs e)
        {
            Button CurrentButton = sender as Button;

            switch (CurrentButton.Tag.ToString())
            {
                case "0":
                    this.ListViewSource.MoveCurrentToFirst();
                    break;
                case "1":
                    this.ListViewSource.MoveCurrentToPrevious();
                    break;
                case "2":
                    this.ListViewSource.MoveCurrentToNext();
                    break;
                case "3":
                    this.ListViewSource.MoveCurrentToLast();
                    break;
            }
        }

        private void InitTimer()
        {
            this.statusBarDate = new DispatcherTimer();
            this.statusBarDate.Interval = new TimeSpan(0, 0, 1);
            this.statusBarDate.Start();
            this.statusBarDate.Tick += new EventHandler(
                delegate (object s, EventArgs a)
                {
                    this.dtStatusBarDate.Text = DateTime.Now.ToString(DateFormat);
                });
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