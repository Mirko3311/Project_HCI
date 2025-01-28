using ASystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using PrviProjektniZadatakHCI;
using System.Collections.ObjectModel;

namespace ASystem
    {
        public partial class StudentWindow : Window
        {
        public static RoutedCommand CancelCommand = new RoutedCommand();
        public static RoutedCommand LogoutCommand = new RoutedCommand();

        private Student student;
        ObservableCollection<Student> studenti = new ObservableCollection<Student>();
        ObservableCollection<DomaciZadatak> domaciZadaci = new ObservableCollection<DomaciZadatak>();
        private Stack<Action> undoStack = new Stack<Action>();
        private Stack<Action> redoStack = new Stack<Action>();
        public StudentWindow(Student student)
        {
            this.student = student;
            InitializeComponent();
            cmbPredmeti.ItemsSource=Predmet.studentiSlusajuPredmet(student);
            cmbPredmetD.ItemsSource = Predmet.studentiSlusajuPredmet(student);
            cmbPredmetiIspit.ItemsSource= Predmet.studentiSlusajuPredmet(student);

            CommandBinding undoBinding = new CommandBinding(ApplicationCommands.Undo, Undo_Executed);
            this.CommandBindings.Add(undoBinding);

            

            CommandBinding logoutBinding = new CommandBinding(LogoutCommand, LogoutButton_Click);
            this.CommandBindings.Add(logoutBinding);
        }
        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Undo_Click(sender, e);
        }
        private void LightThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Light", student.username);
        }

        private void DarkThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Dark", student.username);
        }
        private void GreenThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Green", student.username);
        }

        private void dgPrisustvo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPredmeti.SelectedItem is Predmet selectedPredmet)
            {
                dgPrisustvo.ItemsSource = Prisustvo.pregledPrisustvaList(student.idKorisnika, selectedPredmet.idPredmeta);
            }

        }


        private void ChangeSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (cmbPredmetiIspit.SelectedItem is Predmet selectedPredmet)
            {
            
                dgIspiti.ItemsSource = Ispit.pregledIspita(selectedPredmet,student);
            }
        }
            private void LogoutButton_Click(object sender, RoutedEventArgs e)
            {

                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        
        private void cmbPredmet_SelectionChangedD(object sender, SelectionChangedEventArgs e)
            {

            if (cmbPredmetD.SelectedItem is Predmet selectedPredmet)
            {
                
                dgAktivnosti.ItemsSource = DomaciZadatak.pregledDomacegZadatkaPoPredmetu(selectedPredmet);
               
            }

        }

            private void dgIspiti_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
               
            }
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (undoStack.Count > 0)
            {
                var undoAction = undoStack.Pop();
                undoAction.Invoke();
                redoStack.Push(undoAction);
            }
            else
            {
                MessageBox.Show("Nema dostupnih operacija za poništavanje.", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
      
    }
    }

