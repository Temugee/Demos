using System;
using System.Collections.Generic; // Add this using directive
using System.ComponentModel;
using DevExpress.Maui.CollectionView;


namespace mauiworkshop
{
    public class EmployeeDataViewModel : INotifyPropertyChanged
    {
        readonly ItemData data;

        public event PropertyChangedEventHandler PropertyChanged;
        public IReadOnlyList<Item> Employees { get => data.Itemees; }

        public EmployeeDataViewModel()
        {
            data = new ItemData();
        }

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
