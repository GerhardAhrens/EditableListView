# EditableListView

![NET](https://img.shields.io/badge/NET-8.0-green.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![VS2022](https://img.shields.io/badge/Visual%20Studio-2022-white.svg)
![Version](https://img.shields.io/badge/Version-1.0.2025.0-yellow.svg)]

ListView mit editierbare Columns (wie TextBox, CheckBox, ComboBox, usw.). Grunds�tzlich ist das ListView nur zur Darstellung von Daten ausgelegt. Durch Verwendung von **DataTemplate** k�nnen editierbare Control eingesetzt werden.</br>
In dem Beispiel werden folgende Features gezeigt:
- Filtern von Daten
- Gruppieren von Daten
- Spaltensortierung
- Lesen und speicben des Inhalts in eine JSON Datei
- Navigation Innerhalb des ListView
- Benachrichtigung von �nderungen in einer Zelle 

<img src="MainDialog.png" style="width:750px;"/></br>


Die Spalten des Listview werden mit einem Binding zwischen dem DateTemplate und einer Liste (als ObservableCollection) behandelt. Das Binding erfolgt �ber das Interface *ICollectionView* 
und der Klasse *CollectionViewSource* an dem eine Liste wird als eine Ableitung einer **ObservableCollection** => **NotifyObservableCollection** �bergeben wird. Diese Eigenimplemntierung dient nicht nur der �berwachung was mit einer Row passiert, sondern stellt zus�tzlich die �berwachung der Spalten zur Verf�gung.</br>
So kann eben auch die �nderung des Zelleninhalts direkt dargestellt werden.

## Filtern

Ein Filter f�r ein ListeView kann als Lamda-Ausdruck erstellt, oder innerhalb einer Methode implemeniert werden.
Es kann auch eine Art *Google-Filter* erstellt werden, in dem die Werte alle Properties in einem zu durchsuchenden String zusammengef�gt und dargestellt werden.

```csharp
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
```
Da die Suchen mit *Contains()* erstellt wurde, k�nnen auch Teilstrings gesucht werden. Gru�- oder Kleinschreibung wird ignoriert.
</br>
<img src="Func-Suchen.png" style="width:750px;"/></br>
</br>
## Gruppierung

Mit dem ListView Control ist es auch m�glich, eine Liste nach verschiedenen Kriterien zu gruppieren.</br>
```csharp
private void OnClickGroup(object sender, RoutedEventArgs e)
{
    this.ListViewSource.GroupDescriptions.Clear();

    PropertyInfo pinfo = typeof(ViewItem).GetProperty(cmbGroups.Text);
    if (pinfo != null)
    {
        this.ListViewSource.GroupDescriptions.Add(new PropertyGroupDescription(pinfo.Name));
    }
}
```
Die Gruppierung wird direkt nach dem erstellen angezeigt.
</br>
<img src="Func-Gruppieren.png" style="width:750px;"/></br>
</br>

## Navigation
Eine List ist �ber das Interface *ICollectionView* an das Listview gebunden.

```csharp
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

    this.lvItems.ScrollIntoView(this.ListViewSource.CurrentItem);
}
```
</br>
<img src="Func-Navigation.png" style="width:550px;"/></br>
</br>


