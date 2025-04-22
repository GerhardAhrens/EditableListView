namespace EditableListView.Core
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class DemoData
    {
        public ObservableCollection<ViewItem> Items
        {
            get
            {
                return this.LoadItems();
            }
        }

        public IEnumerable<string> Columns
        {
            get
            {
                return from prop in typeof(ViewItem).GetProperties() select prop.Name;
            }
        }

        public IEnumerable<string> AvailableDevelopment
        {
            get
            {
                return (from developer in this.Items select developer.Developer).Distinct();
            }
        }

        private ObservableCollection<ViewItem> LoadItems()
        {
            ObservableCollection<ViewItem> items = new ObservableCollection<ViewItem>();

            items.Add(new ViewItem { Id = "1", Name = "Horst", Developer = "WPF", Gehalt = 50000.20f, Status = true });
            items.Add(new ViewItem { Id = "2", Name = "Horst", Developer = "ASP.NET", Gehalt = 89000.20f, Status = true });
            items.Add(new ViewItem { Id = "3", Name = "Gerhard", Developer = "ASP.NET", Gehalt = 95000.20f, Status = true });
            items.Add(new ViewItem { Id = "4", Name = "Kunal", Developer = "Silverlight", Gehalt = 26000.20f , Status = false });
            items.Add(new ViewItem { Id = "5", Name = "Hanselman", Developer = "ASP.NET", Gehalt = 78000.20f , Status = true });
            items.Add(new ViewItem { Id = "6", Name = "Peter", Developer = "WPF", Gehalt = 37000.20f , Status = true });
            items.Add(new ViewItem { Id = "7", Name = "Tim", Developer = "Silverlight", Gehalt = 45000.20f , Status = false });
            items.Add(new ViewItem { Id = "8", Name = "John", Developer = "ASP.NET", Gehalt = 70000.20f , Status = true });
            items.Add(new ViewItem { Id = "9", Name = "Jamal", Developer = "ASP.NET", Gehalt = 40000.20f , Status = false });
            items.Add(new ViewItem { Id = "10", Name = "Gerhard", Developer = "C#", Gehalt = 40000.20f , Status = true });
            items.Add(new ViewItem { Id = "11", Name = "Gerhard", Developer = "WPF", Gehalt = 40000.20f , Status = true });
            items.Add(new ViewItem { Id = "12", Name = "Peter", Developer = "C#", Gehalt = 37000.20f, Status = true });
            items.Add(new ViewItem { Id = "13", Name = "Peter", Developer = "ASP.Net", Gehalt = 37000.20f, Status = true });
            items.Add(new ViewItem { Id = "14", Name = "Gerhard", Developer = "Fortran", Gehalt = 40000.20f, Status = true });
            items.Add(new ViewItem { Id = "15", Name = "Gerhard", Developer = "VB.NET", Gehalt = 40000.20f, Status = false });
            items.Add(new ViewItem { Id = "16", Name = "Charlie", Developer = "Fortran", Gehalt = 40000.20f, Status = true });
            items.Add(new ViewItem { Id = "17", Name = "Charlie", Developer = "VB.NET", Gehalt = 40000.20f, Status = false });
            items.Add(new ViewItem { Id = "18", Name = "Stefan", Developer = "C#", Gehalt = 40000.20f, Status = true });
            items.Add(new ViewItem { Id = "19", Name = "Stefan", Developer = "WPF", Gehalt = 40000.20f, Status = true });

            return items;
        }
    }
    public class ViewItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _Id;
        public string Id
        {
            get => this._Id;
            set
            {
                if (this._Id != value)
                {
                    this._Id = value;
                }
            }
        }

        private string _Name;
        public string Name
        {
            get => this._Name;
            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                }
            }
        }

        private string _Developer;
        public string Developer
        {
            get => this._Developer;
            set
            {
                if (this._Developer != value)
                {
                    this._Developer = value;
                }
            }
        }

        private float _Gehalt;
        public float Gehalt
        {
            get => this._Gehalt;
            set
            {
                if (this._Gehalt != value)
                {
                    this._Gehalt = value;
                }
            }
        }

        private bool _Status;
        public bool Status
        {
            get => this._Status;
            set
            {
                if (this._Status != value)
                {
                    this._Status = value;
                }
            }
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

    public class ViewItemCT : IChangeTracking
    {
        private string _Id;
        public string Id
        {
            get => this._Id;
            set
            {
                if (this._Id != value)
                {
                    this._Id = value;
                    this.IsChanged = true;
                }
            }
        }

        private string _Name;
        public string Name
        {
            get => this._Name;
            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                    this.IsChanged = true;
                }
            }
        }

        private string _Developer;
        public string Developer
        {
            get => this._Developer;
            set
            {
                if (this._Developer != value)
                {
                    this._Developer = value;
                    this.IsChanged = true;
                }
            }
        }

        private float _Gehalt;
        public float Gehalt
        {
            get => this._Gehalt;
            set
            {
                if (this._Gehalt != value)
                {
                    this._Gehalt = value;
                    this.IsChanged = true;
                }
            }
        }

        private bool _Status;
        public bool Status
        {
            get => this._Status;
            set
            {
                if (this._Status != value)
                {
                    this._Status = value;
                    this.IsChanged = true;
                }
            }
        }

        public bool IsChanged { get; private set; }
        public void AcceptChanges() => IsChanged = false;
    }

    public class ViewItemTracking : IRevertibleChangeTracking
    {
        Dictionary<string, object> _Values = new Dictionary<string, object>();

        private string _Id;
        public string Id
        {
            get => this._Id;
            set
            {
                if (this._Id != value)
                {
                    this._Id = value;
                    this.IsChanged = true;
                }
            }
        }

        private string _Name;
        public string Name
        {
            get => this._Name;
            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                    this.IsChanged = true;
                }
            }
        }

        private string _Developer;
        public string Developer
        {
            get => this._Developer;
            set
            {
                if (this._Developer != value)
                {
                    this._Developer = value;
                    this.IsChanged = true;
                }
            }
        }

        private float _Gehalt;
        public float Gehalt
        {
            get => this._Gehalt;
            set
            {
                if (this._Gehalt != value)
                {
                    this._Gehalt = value;
                    this.IsChanged = true;
                }
            }
        }

        private bool _Status;
        public bool Status
        {
            get => this._Status;
            set
            {
                if (this._Status != value)
                {
                    this._Status = value;
                    this.IsChanged = true;
                }
            }
        }

        public bool IsChanged { get; private set; }

        public void RejectChanges()
        {
            foreach (var property in _Values)
            {
                this.GetType().GetRuntimeProperty(property.Key).SetValue(this, property.Value);
            }

            this.AcceptChanges();
        }

        public void AcceptChanges()
        {
            this._Values.Clear();
            this.IsChanged = false;
        }
    }
}
