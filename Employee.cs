using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace bios_vil3
{
    public class Employee : INotifyPropertyChanged
    {
        private int id;
        private string imie;
        private string nazwisko;
        private int wiek;
        private string pracapracajaniebyctakiork;

        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }

        public string Imie
        {
            get => imie;
            set { imie = value; OnPropertyChanged(); }
        }

        public string Nazwisko
        {
            get => nazwisko;
            set { nazwisko = value; OnPropertyChanged(); }
        }

        public int Wiek
        {
            get => wiek;
            set { wiek = value; OnPropertyChanged(); }
        }

        public string Pracapracajaniebyctakiork
        {
            get => pracapracajaniebyctakiork;
            set { pracapracajaniebyctakiork = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}