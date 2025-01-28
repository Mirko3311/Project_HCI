using PrviProjektniZadatakHCI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ASystem
{
    public partial class ProfessorWindow : Window
    {
        public static RoutedCommand LogoutCommand = new RoutedCommand();

        private Profesor profesor;
        private Stack<Action> undoStack = new Stack<Action>();
        private Stack<Action> redoStack = new Stack<Action>();
        ObservableCollection<Student> studenti = new ObservableCollection<Student>();
        public ProfessorWindow(Profesor profesor)
        {

           
            this.profesor = profesor;
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            ObservableCollection<String> predmeti = new ObservableCollection<String>();
            ObservableCollection<Predmet> listaPredmeta = new ObservableCollection<Predmet>();
            listaPredmeta = Predmet.izlistajPredmete(profesor);
            ObservableCollection<Student> listaStudenata = Student.GetStudents();
            predmeti = Predmet.GetPredmeti(Profesor.GetProfesorId(profesor));
            homeworkDataGrid.ItemsSource = DomaciZadatak.pregledDomacegPoProfesoru(profesor);
            cmbStudenti.ItemsSource = listaStudenata;
            cmbStudentsGrade.ItemsSource = listaStudenata;
            cmbSubjectsForGrade.ItemsSource = listaPredmeta;
            lvDataBinding.ItemsSource = listaPredmeta;
            cmbPredmeti.ItemsSource = listaPredmeta;
            cmbSubjects.ItemsSource = listaPredmeta;
            this.DataContext = listaPredmeta;
            cmbPredmet.ItemsSource = listaPredmeta;
            cmbPredmetOcjena.ItemsSource = listaPredmeta;

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
            ChangerThemes.ChangeTheme("Light", profesor.username);
        }

        private void DarkThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Dark", profesor.username);
        }

        private void GreenThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Green", profesor.username);
        }
        private void cmbPredmet_SelectionChangedOcjene(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPredmetOcjena.SelectedItem is Predmet selectedPredmet)
            {
               
                studenti = Predmet.studentiSlusaju(selectedPredmet);

                foreach (Student s in studenti)
                {
                    Console.WriteLine("Student:" + s.getIdKorisnika(s));

                }

                if (studenti != null && studenti.Count > 0)
                {
                    cmbStudents.ItemsSource = studenti;
                }
                else
                {
                    MessageBox.Show("Nema studenata za odabrani predmet.");
                    cmbStudents.ItemsSource = null;
                }
            }
        }

        private void cmbSubject_SelectionChanged_Grade(object sender, SelectionChangedEventArgs e)
        {

            if (cmbStudentsGrade.SelectedItem is Student student)
            {
                if (cmbSubjectsForGrade.SelectedItem is Predmet predmet)
                {
                    ListGrade.ItemsSource = Ispit.pregledIspita(predmet, student);
                }
                else
                {
                    MessageBox.Show("Molimo izaberite predmet.");
                }
            }
            else
            {
                MessageBox.Show("Molimo izaberite studenta.");
            }
        }
        private void cmbStudent_SelectionChanged_Grade(object sender, SelectionChangedEventArgs e)
        {
           
            if (cmbStudentsGrade.SelectedItem is Student student)
            {
                if (cmbSubjectsForGrade.SelectedItem is Predmet predmet)
                {
                    ListGrade.ItemsSource = Ispit.pregledIspita(predmet, student);
                }
                else
                {       
                    MessageBox.Show("Molimo izaberite predmet.");
                }
            }
            else
            {
                MessageBox.Show("Molimo izaberite studenta.");
            }
        }


        private void cmbPredmet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPredmet.SelectedItem is Predmet selectedPredmet)
            {
                studenti = Predmet.studentiSlusaju(selectedPredmet);
                if (studenti != null && studenti.Count > 0)
                {
                    AttendanceDataGrid.ItemsSource = studenti;
                }
                else
                {
                    MessageBox.Show("Nema studenata za odabrani predmet.");
                    AttendanceDataGrid.ItemsSource = null;
                }
            }
        }
    
        private void cmbSubject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmbStudenti.SelectedItem is Student selectedStudent && cmbSubjects.SelectedItem is Predmet selectedPredmet)
            {
                Attendance.ItemsSource = Prisustvo.PregledPrisustva(selectedStudent, selectedPredmet, profesor);
            }

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

        private void SaveAttendance(object sender, RoutedEventArgs e)
        {
            Predmet predmet = cmbPredmet.SelectedItem as Predmet;
            DateTime? vrijeme = AttendanceDate.SelectedDate;

            if (predmet == null || !vrijeme.HasValue)
            {
                MessageBox.Show("Molimo vas da izaberete predmet i datum.");
                return;
            }

            ObservableCollection<Student> selektovaniStudenti = new ObservableCollection<Student>();

            foreach (var item in AttendanceDataGrid.Items)
            {
                var dataRow = item as Student;
                if (dataRow != null)
                {
                   
                    bool uspjeh = Prisustvo.UnesiPrisustvo(vrijeme.Value, predmet, dataRow);

                    if (uspjeh /*&& dataRow.IsChecked*/)
                    {
                        selektovaniStudenti.Add(dataRow);
                    }
                }
            }

            if (selektovaniStudenti.Count > 0)
            {
                MessageBox.Show($"Prisustvo evidentirano za {selektovaniStudenti.Count} studenata na predmetu {predmet.Naziv} na datum {vrijeme.Value:dd.MM.yyyy}.");
            }
            else
            {
                MessageBox.Show("Nijedan student nije selektovan.");
            }
            foreach (var item in AttendanceDataGrid.Items)
            {
                var dataRow = item as Student;
                if (dataRow != null)
                {
                   
                }
            }

            AttendanceDataGrid.Items.Refresh();
        }


        private void AzurirajPodatke(object sender, RoutedEventArgs e)
        {   var selektovaniPredmet = lvDataBinding.SelectedItem as Predmet;

            if (selektovaniPredmet != null)
            {
                MessageBox.Show($"Ažuriranje podataka za predmet: {selektovaniPredmet.Naziv}");
            }
            else
            {
               
                MessageBox.Show("Nema selektovanog predmeta za ažuriranje.");
            }
        }



        private void PregledStudenata(object sender, RoutedEventArgs e)
        {

            if (lvDataBinding.SelectedItem is Predmet selektovaniPredmet)
            {
                ObservableCollection<Student> listaStudenata = Predmet.studentiSlusaju(selektovaniPredmet);
                StudentView studentView = new StudentView(listaStudenata);
                studentView.ShowDialog();
            }
            else
            {
                MessageBox.Show("Nema selektovanog predmeta.");
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
           
            CheckBox checkBox = sender as CheckBox;
          
        }
        private void SaveGrade(object sender, RoutedEventArgs e)
        {
         
            if (cmbPredmetOcjena.SelectedItem is not Predmet predmet)
            {
                MessageBox.Show("Molimo izaberite predmet.");
                return;
            }
            if (ExamDate.SelectedDate is not DateTime datum)
            {
                MessageBox.Show("Molimo izaberite datum ispita.");
                return;
            }
            if (!double.TryParse(txtBodovi.Text, out double bodovi) || bodovi < 0 || bodovi > 100)
            {
                MessageBox.Show("Molimo unesite validan broj bodova (0-100).");
                return;
            }
            if (!int.TryParse(txtOcjena.Text, out int ocjena) || ocjena < 5 || ocjena > 10)
            {
                MessageBox.Show("Molimo unesite validnu ocjenu (5-10).");
                return;
            }
            if (cmbStudents.SelectedItem is not Student student)
            {
                MessageBox.Show("Molimo izaberite studenta.");
                return;
            }
            try
            {
                
                Ispit.sacuvajIspit(bodovi, ocjena, datum, predmet, student);
                cmbPredmetOcjena.SelectedIndex=-1;
                ExamDate.SelectedDate = null;
                cmbStudents.SelectedIndex = -1;
                txtOcjena.Clear();
                txtBodovi.Clear();
                txtOcjena.Clear();
                MessageBox.Show("Ispit je uspješno sačuvan!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Došlo je do greške prilikom čuvanja ispita: {ex.Message}");
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            CheckBox checkBox = sender as CheckBox;

        }
        private void Dodaj(object sender, RoutedEventArgs e)
        {

            Predmet predmet = cmbPredmeti.SelectedItem as Predmet;

            if (predmet == null)
            {
                MessageBox.Show("Predmet nije odabran.");
                return; 
            }
            string predmetNaziv = predmet.Naziv;

            string tipZadatka = TaskTypeSelector.SelectedItem is ComboBoxItem tipItem ? tipItem.Content.ToString() : "Nije odabran";
            string naziv = ime.Text;
            string opis = taskDescription.Text;
            string sifra = taskCode.Text;
            DateTime? rok = TaskDeadline.SelectedDate;
            string maksimalniBrojBodova = MaxPoints.Text;


            if (string.IsNullOrEmpty(naziv) || string.IsNullOrEmpty(opis) || string.IsNullOrEmpty(sifra) || !rok.HasValue || string.IsNullOrEmpty(maksimalniBrojBodova))
            {

                MessageBox.Show("Sva polja moraju biti popunjena.");
                return; 
            }

            ObservableCollection<Student> studenti = Predmet.studentiSlusaju(predmet);


            if (DomaciZadatak.dodajDZadatak(naziv, opis, (DateTime)rok, sifra, predmet, profesor))
            {
               
                cmbPredmeti.SelectedItem = null;
                TaskTypeSelector.SelectedItem = null;
                ime.Text = string.Empty;
                taskDescription.Text = string.Empty;
                taskCode.Text = string.Empty;
                MaxPoints.Text = string.Empty;
                TaskDeadline.SelectedDate = null;

                homeworkDataGrid.ItemsSource = DomaciZadatak.pregledDomacegPoProfesoru(profesor);
            }
            undoStack.Push(() =>
            {
                DomaciZadatak.obrisiDZadatak(sifra);
                MessageBox.Show($"Undo: Domaci zadatak je obrisan.");
            });
            redoStack.Push(() =>
            {
            DomaciZadatak.dodajDZadatak(naziv, opis, (DateTime)rok, sifra, predmet, profesor);
                MessageBox.Show($"Redo: Predmet {predmet.Naziv} je ponovo dodat.");
        });
        

        }
    
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show(); 
            this.Close();
        }

    }
}