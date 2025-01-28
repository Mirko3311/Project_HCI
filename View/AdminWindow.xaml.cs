using MySql.Data.MySqlClient;
using PrviProjektniZadatakHCI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
using System.Windows.Shapes;

namespace ASystem
{


    public partial class AdminWindow : Window
    {
       
        public static RoutedCommand CancelCommand = new RoutedCommand();
        public static RoutedCommand LogoutCommand = new RoutedCommand();
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;

        private Stack<Action> undoStack = new Stack<Action>();
        private Stack<Action> redoStack = new Stack<Action>();


        ObservableCollection<Predmet> predmeti = Predmet.pregledPredmeta();
        ObservableCollection<Student> studenti = Student.GetStudents();
        ObservableCollection<Profesor> profesori = Profesor.GetProfessors();
        public AdminWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.DataContext = this;
            cmbProfesori.ItemsSource = profesori;
            cmbPredmeti.ItemsSource = predmeti;
            cmbProfessors.ItemsSource = profesori;

            CommandBinding undoBinding = new CommandBinding(ApplicationCommands.Undo, Undo_Executed);
            this.CommandBindings.Add(undoBinding);

            CommandBinding cancelBinding = new CommandBinding(CancelCommand, Cancel_Click);
            this.CommandBindings.Add(cancelBinding);

            CommandBinding logoutBinding = new CommandBinding(LogoutCommand, LogoutButton_Click);
            this.CommandBindings.Add(logoutBinding);
        }
        private void Cancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Cancel_Click(sender, e); 
        }
        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Undo_Click(sender, e);
        }

        private void SubjectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProfessors.SelectedItem is Profesor selectedProfessor)
            {
                cmbSubjects.ItemsSource = Profesor.profesorPredaje(selectedProfessor);

            }
        }

        private void AddProfessor_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProfessorName.Text) ||
                string.IsNullOrWhiteSpace(txtProfessorSurname.Text) ||
                string.IsNullOrWhiteSpace(email.Text) ||
                string.IsNullOrWhiteSpace(username.Text) ||
                string.IsNullOrWhiteSpace(password.Text) ||
                string.IsNullOrWhiteSpace(titule.Text))
            {
                MessageBox.Show("Molimo vas da popunite sva polja.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(id.Text.Trim(), out int parsedId))
            {
                MessageBox.Show("Identifikator mora da bude cjelobrojna vrijednost!");
                return;
            }
            if (profesori.Any(p => p.idKorisnika == parsedId))
            {

                MessageBox.Show("Identifikator već postoji. Molimo unesite jedinstven identifikator.");
                return;
            }

            Profesor profesor = new Profesor(
       idKorisnik: parsedId,
       ime: txtProfessorName.Text,
       prezime: txtProfessorSurname.Text,
       email: email.Text,
       username: username.Text,
       password: password.Text,
       tipKorisnika: "profesor",
       zvanje: titule.Text
   );
          
            Profesor.dodajProfesora(profesor);
            undoStack.Push(() =>
            {
                Profesor.obrisiProfesora(profesor);
                MessageBox.Show($"Undo: Profesor {profesor.ime} {profesor.prezime} je obrisan.");
            });

            redoStack.Push(() =>
            {
                Profesor.dodajProfesora(profesor);
                MessageBox.Show($"Redo: Profesor {profesor.ime} {profesor.prezime} je ponovo dodat.");
            });
            redoStack.Clear();

            RefreshProfessors();

            MessageBox.Show("Profesor je uspješno dodat!", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);

            txtProfessorName.Text = "";
            txtProfessorSurname.Text = "";
            email.Text = "";
            username.Text = "";
            password.Text = "";
            titule.Text = "";
            id.Text = "";

        }

        private void cmbAddChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAddChoice.SelectedItem is ComboBoxItem selectedItem)
            {
                string choice = selectedItem.Tag?.ToString();
                addProfessorForm.Visibility = Visibility.Collapsed;
                addStudentForm.Visibility = Visibility.Collapsed;
                addSubjectForm.Visibility = Visibility.Collapsed;

                switch (choice)
                {
                    case "Professor":
                        addProfessorForm.Visibility = Visibility.Visible;
                        break;
                    case "Student":
                        addStudentForm.Visibility = Visibility.Visible;
                        break;
                    case "Subject":
                        addSubjectForm.Visibility = Visibility.Visible;
                        break;
                }
            }
        }


        private void RefreshProfessors()
        {
            profesori.Clear(); 
            var refreshedProfessors = Profesor.GetProfessors(); 
            foreach (var professor in refreshedProfessors)
            {
                profesori.Add(professor); 
            }
        }

        private void RefreshStudents()
        {
            studenti.Clear();
            var refreshedStudents = Student.GetStudents();         
           foreach (var student in refreshedStudents)
           {
                studenti.Add(student);
           }
        }


        private object selectedItem = null;

        private void cmbDeleteChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDeleteChoice.SelectedItem is ComboBoxItem selectedItem)
            {
                string choice = selectedItem.Tag?.ToString();

                lstDeleteItems.ItemsSource = null; 

                DataTemplate itemTemplate = new DataTemplate();

                switch (choice)
                {
                    case "Professor":
                        lstDeleteItems.ItemsSource = profesori;
                        itemTemplate = CreateDataTemplate("ime", "prezime", "Zvanje");
                        break;

                    case "Student":
                        lstDeleteItems.ItemsSource = studenti;
                        itemTemplate = CreateDataTemplate("ime", "prezime", "BrojIndeksa");
                        break;

                    case "Subject":
                        lstDeleteItems.ItemsSource = predmeti;
                   
                        itemTemplate = CreateDataTemplate("Naziv");
                        break;
                }

               
                lstDeleteItems.ItemTemplate = itemTemplate;
            }
        }


        private void cmbUpdateE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbUpdate.SelectedItem != null)
            {
               
                var selektovaniEntitet = cmbUpdate.SelectedItem;

               
                contentPanel.Content = selektovaniEntitet;
                if (selektovaniEntitet is Profesor profesor)
                {
                    contentPanel.DataContext = profesor;
                }
                else if (selektovaniEntitet is Student student)
                {
                    contentPanel.DataContext = student;
                }
                else if (selektovaniEntitet is Predmet predmet)
                {
                    contentPanel.DataContext = predmet;
                }
            }
        }

        private void CmbUpdateChoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmbUpdateChoice.SelectedItem is ComboBoxItem selectedItem)
            {
                string choice = selectedItem.Tag?.ToString();
                switch (choice)
                {
                    case "Professor":
                        contentPanel.ContentTemplate = (DataTemplate)Resources["ProfesorTemplate"];
                        cmbUpdate.ItemsSource = profesori;

                        break;

                    case "Student":
                        contentPanel.ContentTemplate = (DataTemplate)Resources["StudentTemplate"];
                        cmbUpdate.ItemsSource = studenti;
                        break;

                    case "Subject":
                        contentPanel.ContentTemplate = (DataTemplate)Resources["SubjectTemplate"];

                        cmbUpdate.ItemsSource = predmeti;
                        break;
                }
            }
        }




        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (lstDeleteItems.SelectedItem != null)
            {
                string choice = ((ComboBoxItem)cmbDeleteChoice.SelectedItem).Content.ToString();

                switch (choice)
                {
                    case "Profesor":
                        var selectedProfesor = (Profesor)lstDeleteItems.SelectedItem;

                        MessageBox.Show("Profesor:" + selectedProfesor.ime + " " + selectedProfesor.prezime);
                        undoStack.Push(() =>
                        {
                            Profesor.dodajProfesora(selectedProfesor);
                            MessageBox.Show($"Undo: Profesor {selectedProfesor.ime} {selectedProfesor.prezime} je vraćen.");
                        });
                        redoStack.Push(() =>
                        {
                            Profesor.obrisiProfesora(selectedProfesor);
                            MessageBox.Show($"Redo: Profesor {selectedProfesor.ime} {selectedProfesor.prezime} je obrisan.");
                        });
                        if (Profesor.obrisiProfesora(selectedProfesor))
                        {
                            MessageBox.Show("Profesor je obrisan!");


                            RefreshProfessors();
                        }

                        break;

                    case "Student":

                        var selectedStudent = (Student)lstDeleteItems.SelectedItem;

                        undoStack.Push(() =>
                        {
                            Student.dodajStudenta(selectedStudent);
                            MessageBox.Show($"Undo: Profesor {selectedStudent.ime} {selectedStudent.prezime} je vraćen.");
                        });
                        redoStack.Push(() =>
                        {
                            Student.obrisiStudenta(selectedStudent.idKorisnika);
                            MessageBox.Show($"Redo: Student: {selectedStudent.ime} {selectedStudent.prezime} je obrisan.");
                        });
                        if (Student.obrisiStudenta(selectedStudent.idKorisnika))
                        {
                            MessageBox.Show("Student je obrisan!");
                            RefreshStudents();

                        }

                        break;


                    case "Predmeti":
                        var selectedPredmet = (Predmet)lstDeleteItems.SelectedItem;
                        undoStack.Push(() =>
                       {
                           Predmet.dodajPredmet(selectedPredmet);
                           MessageBox.Show($"Undo: Predmet {selectedPredmet.naziv} je vraćen.");
                       });
                        redoStack.Push(() =>
                        {
                            Predmet.ObrisiPredmet(selectedPredmet.idPredmeta); // Obriši predmet
                            MessageBox.Show($"Redo: Predmet {selectedPredmet.naziv} je obrisan.");
                        });
                        if (Predmet.ObrisiPredmet(selectedPredmet.idPredmeta))
                        { 
                            predmeti.Remove(selectedPredmet);
                            lstDeleteItems.Items.Refresh();
                        }

                        break;
                }
                cmbDeleteChoice_SelectionChanged(null, null);
            }
            else
            {
                MessageBox.Show("Molimo izaberite stavku za brisanje.", "Upozorenje", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private DataTemplate CreateDataTemplate(params string[] propertyNames)
        {
            var template = new DataTemplate();
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            foreach (var propertyName in propertyNames)
            {
                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding(propertyName));
                textBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(5, 0, 0, 0));
                stackPanelFactory.AppendChild(textBlockFactory);
            }
   
            template.VisualTree = stackPanelFactory;

            return template;
        }


        private void lstDeleteItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItem = lstDeleteItems.SelectedItem;
            btnDelete.IsEnabled = selectedItem != null;
        }


        private void txtStudentName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtStudentName.Text.Length < 3)
            {
                txtStudentName.BorderBrush = Brushes.Red;
            }
            else
            {
                txtStudentName.BorderBrush = Brushes.Green;
            }
        }



        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtStudentName.Text) ||
                string.IsNullOrWhiteSpace(txtStudentSurname.Text) ||
                string.IsNullOrWhiteSpace(emailS.Text) ||
                string.IsNullOrWhiteSpace(usernameS.Text) ||
                string.IsNullOrWhiteSpace(passwordS.Text) ||
                string.IsNullOrWhiteSpace(grade.Text) ||
                string.IsNullOrWhiteSpace(idS.Text) ||
                string.IsNullOrWhiteSpace(index.Text))
            {
                MessageBox.Show("Molimo vas da popunite sva polja.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
    
             if (!int.TryParse(grade.Text.Trim(), out int parseGrade))
            {
                MessageBox.Show("Identifikator studenta mora da bude cjelobrojna vrijednost!");
                return;
            }
            if (!int.TryParse(idS.Text.Trim(), out int parseIds))
            {
                MessageBox.Show("Identifikator studenta mora da bude cjelobrojna vrijednost!");
                return;
            }
          
            if (studenti.Any(p => p.idKorisnika == parseIds))
            {
                MessageBox.Show("Identifikator već postoji. Molimo unesite jedinstven identifikator za studenta.");
                return;
            }
            int.TryParse(grade.Text, out parseGrade);
            int.TryParse(idS.Text, out parseIds);

            if (parseIds <= 0 || parseGrade <= 0)
            {
                MessageBox.Show("Nevalidan unos za ID ili godinu studija.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Student student = new Student(
                parseIds,
                txtStudentName.Text,
                txtStudentSurname.Text,
                emailS.Text,
                usernameS.Text,
                passwordS.Text,
                "student",
                index.Text,
                parseGrade
            );
            if (Student.dodajStudenta(student))
            {
                MessageBox.Show("Student je uspešno dodat!", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                studenti = Student.GetStudents();             
                cmbPredmeti.ItemsSource = Student.GetStudents();
                lstDeleteItems.ItemsSource = Student.GetStudents();
            }
                undoStack.Push(() =>
                {
                    Student.obrisiStudenta(student.idKorisnika);
                    MessageBox.Show($"Undo: Student sa ID-jem {student.idKorisnika} je uklonjen.");
                });
                redoStack.Clear();


            
            txtStudentName.Clear();
            txtStudentSurname.Clear();
            emailS.Clear();
            usernameS.Clear();
            passwordS.Clear();
            idS.Clear();
            grade.Clear();
            lstStudentSubjects.UnselectAll();
            index.Clear();
        }

        private void LightThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Light","admin");
        }

        private void DarkThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Dark","admin");
        }
        private void GreenThemeClick(object sender, RoutedEventArgs e)
        {
            ChangerThemes.ChangeTheme("Green", "admin");
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

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (redoStack.Count > 0)
            {
                var redoAction = redoStack.Pop();
                redoAction.Invoke();
                undoStack.Push(redoAction);
            }
            else
            {
                MessageBox.Show("Nema dostupnih operacija za ponovno izvršenje.", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubjectName.Text) ||
                string.IsNullOrWhiteSpace(characteristic.Text) ||
                string.IsNullOrWhiteSpace(ects.Text) ||
                string.IsNullOrWhiteSpace(identifikator.Text))
            {
                MessageBox.Show("Molimo vas da popunite sva polja.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(identifikator.Text.Trim(), out int id))
            {
                MessageBox.Show("Identifikator mora da bude cjelobrojna vrijednost!");
                return;
            }


            if (!int.TryParse(ects.Text.Trim(), out int ECTS))
            {
                MessageBox.Show("ECTS mora da bude cjelobrojna vrijednost!");
                return;
            }
    
            if (predmeti.Any(p => p.IdPredmeta == id))
            {
                MessageBox.Show("Identifikator već postoji. Molimo unesite jedinstven identifikator.");
                return; 
            }
            Predmet predmet = new Predmet
            {


                naziv = txtSubjectName.Text,
                opis = characteristic.Text,
                eCTS = ECTS,
                idPredmeta = id
            };

            if(Predmet.dodajPredmet(predmet))
            {
                predmeti = Predmet.pregledPredmeta();
                cmbPredmeti.ItemsSource = Predmet.pregledPredmeta();
                lstDeleteItems.ItemsSource = Predmet.pregledPredmeta();
              //  contentPanel.DataContext = Predmet.pregledPredmeta();
                MessageBox.Show("Predmet je uspješno dodat!", "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                txtSubjectName.Clear();
                characteristic.Clear();
                ects.Clear();
                identifikator.Clear();
            }
            else
            {
                MessageBox.Show("Došlo je do greške prilikom dodavanja predmeta.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            undoStack.Push(() =>
            {
                Predmet.ObrisiPredmet(predmet.idPredmeta);
                MessageBox.Show($"Undo: Predmet {predmet.Naziv} je obrisan.");
            });

            redoStack.Push(() =>
            {
                Predmet.dodajPredmet(predmet);
                MessageBox.Show($"Redo: Predmet {predmet.Naziv} je ponovo dodat.");
            });
       
            txtSubjectName.Clear();
            characteristic.Clear();
            ects.Clear();
            identifikator.Clear();
        }

        private void UpdateProfessor_Click(object sender, RoutedEventArgs e)
        {
          
            Profesor selektovaniProfesor = contentPanel.DataContext as Profesor;
            if (selektovaniProfesor != null)
            {



                Profesor stariProfesor = new Profesor
                {
                    idKorisnika = selektovaniProfesor.idKorisnika,
                    ime = selektovaniProfesor.ime,
                    prezime = selektovaniProfesor.prezime,
                    email = selektovaniProfesor.email,
                    username = selektovaniProfesor.username,
                    password = selektovaniProfesor.password,
                    tipKorisnika = selektovaniProfesor.tipKorisnika,
                    Zvanje = selektovaniProfesor.Zvanje
                };
                undoStack.Push(() =>
                {               
                    Profesor.AzurirajProfesora(stariProfesor);
                    MessageBox.Show($"Undo: Profesor {stariProfesor.ime} {stariProfesor.prezime} je vraćen.");
                });
                redoStack.Push(() =>
                {
                    Profesor.AzurirajProfesora(selektovaniProfesor);
                    MessageBox.Show($"Redo: Profesor {selektovaniProfesor.ime} {selektovaniProfesor.prezime} je ponovo ažuriran.");
                });
                bool success = Profesor.AzurirajProfesora(selektovaniProfesor);

              
                if (success)
                {
                    MessageBox.Show("Profesor je uspešno ažuriran.");
                    txtProfessorName.Clear();
                    txtProfessorSurname.Clear();
                    email.Clear();
                    titule.Clear();
                    
                    

                }
                else
                {
                    MessageBox.Show("Došlo je do greške prilikom ažuriranja profesora.");
                }
            }
        }

       private void UpdateStudent_Click(object sender, RoutedEventArgs e)
        {
           
            Student selektovaniStudent = contentPanel.DataContext as Student;
        
            if (selektovaniStudent != null)
            {
               
                bool success = Student.AzurirajStudenta(selektovaniStudent);
                if (success)
                {
                    MessageBox.Show("Student je uspešno ažuriran.");
                    txtStudentName.Clear();
                    txtStudentSurname.Clear();
                    emailS.Clear();
                    index.Clear();
                    grade.Clear();                                
                }
                else
                {
                    MessageBox.Show("Došlo je do greške prilikom ažuriranja studenta.");
                }
            }
        }

        private void RestartField()
        {

            if (contentPanel.DataContext is Student student)
            {
                student.ime = string.Empty;
                student.prezime = string.Empty;
                student.email = string.Empty;
                student.GodinaStudija = 0;
                student.BrojIndeksa = string.Empty;
            }


            contentPanel.DataContext = null;
            contentPanel.DataContext = new StudentView();
        }

        private void UpdateSubject_Click(object sender, RoutedEventArgs e)
        {
            Predmet selektovaniPredmet = contentPanel.DataContext as Predmet;
            if (selektovaniPredmet != null)
            {
            
                bool success = Predmet.AzurirajPredmet(selektovaniPredmet);
                if (success)
                {
                    MessageBox.Show("Predmet je uspešno ažuriran.");
                }
                else
                {
                    MessageBox.Show("Došlo je do greške prilikom ažuriranja predmeta.");
                }
            }
        }


        private void InsertProfSub(object sender, RoutedEventArgs e)
        {
           
            var selektovaniProfesor = cmbProfesori.SelectedItem as Profesor; 
            var selektovaniPredmet = cmbPredmeti.SelectedItem as Predmet;
            if (selektovaniProfesor != null && selektovaniPredmet != null)
            {
                Predmet.profesorPredmet(selektovaniProfesor, selektovaniPredmet);
                cmbPredmeti.SelectedIndex=-1;
                cmbProfesori.SelectedIndex = -1;
                MessageBox.Show($"Selektovani profesor: {selektovaniProfesor.DisplayText}\n" +
                                $"Selektovani predmet: {selektovaniPredmet.DisplayText}");
            }
        }

        private void DeleteProfSub(object sender, RoutedEventArgs e)
        {
          
            var selektovaniProfesor = cmbProfessors.SelectedItem as Profesor;
            var selektovaniPredmet = cmbSubjects.SelectedItem as Predmet;

            if (selektovaniProfesor == null)
            {
                MessageBox.Show("Nije selektovan profesor.");
            }
            if (selektovaniPredmet == null)
            {
                MessageBox.Show("Nije selektovan predmet.");
            }

            if (selektovaniProfesor != null && selektovaniPredmet != null)
            {
                if (Predmet.razduzi(selektovaniProfesor, selektovaniPredmet))
                {
                    cmbSubjects.SelectedIndex = -1;
                    cmbProfessors.SelectedIndex = -1;
                    predmeti.Remove(selektovaniPredmet);
                }
                MessageBox.Show($"Selektovani profesor: {selektovaniProfesor.DisplayText}\n" +
                                $"Selektovani predmet: {selektovaniPredmet.DisplayText}");
            }
        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            username.Clear();
            password.Clear();
            id.Clear();
            txtProfessorName.Clear();
            txtProfessorSurname.Clear();
            titule.Clear();
            txtStudentName.Clear();
            txtStudentSurname.Clear();
            txtSubjectName.Clear();
            emailS.Clear();
            usernameS.Clear();
            passwordS.Clear();
            idS.Clear();
            index.Clear();
            grade.Clear();
            txtSubjectName.Clear();
            characteristic.Clear();
            ects.Clear();
            identifikator.Clear();
            email.Clear();
        }
    }
}
   



