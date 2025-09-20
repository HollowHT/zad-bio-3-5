using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalonia.Media;

namespace bios_vil3
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Employee> Employees { get; } = new ObservableCollection<Employee>();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var secondWindow = new SecondWindow();
            var result = await secondWindow.ShowDialog<Employee>(this);
        
            if (result is Employee newEmployee)
            {
                newEmployee.Id = Employees.Count + 1;
                Employees.Add(newEmployee);
                System.Diagnostics.Debug.WriteLine($"Added new employee: {newEmployee.Imie} {newEmployee.Nazwisko}");
                System.Diagnostics.Debug.WriteLine($"Employees count: {Employees.Count}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No employee was added");
            }
        }

        private void RemoveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItem is Employee selectedEmployee)
            {
                Employees.Remove(selectedEmployee);
            }
        }

        private async void SaveCsvButton_Click(object? sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filters.Add(new FileDialogFilter() { Name = "CSV Files", Extensions = { "csv" } });

            var result = await saveFileDialog.ShowAsync(this);
            if (result != null)
            {
                await SaveToCsvAsync(result);
            }
        }

        private async void LoadCsvButton_Click(object? sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filters.Add(new FileDialogFilter() { Name = "CSV Files", Extensions = { "csv" } });

            var result = await openFileDialog.ShowAsync(this);
            if (result != null && result.Length > 0)
            {
                await LoadFromCsvAsync(result[0]);
            }
        }

        private async void SaveXmlButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Zapisz plik XML",
                Filters = new List<FileDialogFilter>
                    { new FileDialogFilter { Name = "XML files", Extensions = { "xml" } } }
            };

            var result = await saveFileDialog.ShowAsync(this);

            if (!string.IsNullOrEmpty(result))
            {
                var osoby = Employees.Select(emp => new Osoba
                {
                    Id = emp.Id,
                    Imie = emp.Nazwisko,
                    Nazwisko = emp.Imie,
                    Wiek = emp.Wiek,
                    Pracapracajaniebyctakiork = emp.Pracapracajaniebyctakiork
                }).ToList();

                var serializer = new XmlSerializer(typeof(List<Osoba>));
                using (var writer = new StreamWriter(result))
                {
                    serializer.Serialize(writer, osoby);
                }
            }
        }

        private async void SaveJSONButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Zapisz plik JSON",
                Filters = new List<FileDialogFilter> { new FileDialogFilter { Name = "JSON files", Extensions = { "json" } } }
            };

            var result = await saveFileDialog.ShowAsync(this);

            if (!string.IsNullOrEmpty(result))
            {
                var osoby = Employees.Select(emp => new Osoba
                {
                    Id = emp.Id,
                    Imie = emp.Imie,
                    Nazwisko = emp.Nazwisko,
                    Wiek = emp.Wiek,
                    Pracapracajaniebyctakiork = emp.Pracapracajaniebyctakiork
                }).ToList();

                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(osoby, options);
        
                await File.WriteAllTextAsync(result, jsonString);
            }
        }

        private async Task SaveToCsvAsync(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                await writer.WriteLineAsync("Id,FirstName,LastName,Age,Position");
                foreach (var employee in Employees)
                {
                    await writer.WriteLineAsync($"{employee.Id},{employee.Imie},{employee.Nazwisko},{employee.Wiek},{employee.Pracapracajaniebyctakiork}");
                }
            }
        }

        private async Task LoadFromCsvAsync(string filePath)
        {
            Employees.Clear();
            using (var reader = new StreamReader(filePath))
            {
                await reader.ReadLineAsync(); // Skip header
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var values = line.Split(',');
                    if (values.Length == 5 && int.TryParse(values[0], out int id) && int.TryParse(values[3], out int age))
                    {
                        Employees.Add(new Employee
                        {
                            Id = id,
                            Imie = values[1],
                            Nazwisko = values[2],
                            Wiek = age,
                            Pracapracajaniebyctakiork = values[4]
                        });
                    }
                }
            }
        }

        private async void LoadJsonButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Wczytaj plik JSON", Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "JSON files", Extensions = { "json" } }
                }, AllowMultiple = false
            }; 
            var result = await openFileDialog.ShowAsync(this); 
            if (result != null && result.Length > 0) { string filePath = result[0];
                try
                {
                    string jsonString = await File.ReadAllTextAsync(filePath);
                    var osoby = JsonSerializer.Deserialize<List<Osoba>>(jsonString);
                    if (osoby != null)
                    {
                        Employees.Clear();
                        foreach (var osoba in osoby)
                        {
                            Employees.Add(new Employee
                            {
                                Id = osoba.Id, Imie = osoba.Imie, Nazwisko = osoba.Nazwisko, Wiek = osoba.Wiek,
                                Pracapracajaniebyctakiork = osoba.Pracapracajaniebyctakiork
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoadJSONButton.Background=Brushes.Red;
                } }
        }

        private async void LoadXmlButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Wczytaj plik XML", Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "XML files", Extensions = { "xml" } }
                }, AllowMultiple = false
            }; var result = await openFileDialog.ShowAsync(this); 
            if (result != null && result.Length > 0) { string filePath = result[0];
                try
                {
                    var serializer = new XmlSerializer(typeof(List<Osoba>));
                    using (var reader = new StreamReader(filePath))
                    {
                        var osoby = (List<Osoba>)serializer.Deserialize(reader);
                        if (osoby != null)
                        {
                            Employees.Clear();
                            foreach (var osoba in osoby)
                            {
                                Employees.Add(new Employee
                                {
                                    Id = osoba.Id, Imie = osoba.Imie, Nazwisko = osoba.Nazwisko,
                                    Wiek = osoba.Wiek, Pracapracajaniebyctakiork = osoba.Pracapracajaniebyctakiork
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoadXmlButton.Background = Brushes.Red;
                } }
        }
    }
}