using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;

namespace ASystem
{
    public class Student : Korisnik
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;
        public string BrojIndeksa { get; set; }
        public int GodinaStudija { get; set; }
       
      


        public bool IsPrisutanChecked { get; set; }
        public bool IsOdsutanChecked { get; set; }
        public Student(int idKorisnika, string ime, string prezime, string email, string username, string password, string tipKorisnika, string brojIndeksa, int godinaStudija)
            : base(idKorisnika,ime, prezime, email, username, password, tipKorisnika)  
        {
            this.BrojIndeksa = brojIndeksa;
            this.GodinaStudija = godinaStudija;
        }

        public static ObservableCollection<Student> GetStudents()
        {
            ObservableCollection<Student> result = new ObservableCollection<Student>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
        SELECT k.idKorisnik, k.ime, k.prezime, k.username, k.email, s.brojIndeksa, s.godinaStudija
        FROM Student s
        JOIN Korisnik k ON s.Korisnik_idKorisnik = k.idKorisnik
        WHERE k.Tip_Korisnika = 'student'";

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Student student = new Student(
                    reader.GetInt32("idKorisnik"),
                    reader.GetString("ime"),
                    reader.GetString("prezime"),
                    reader.GetString("email"),
                    reader.GetString("username"),
                    "",
                    "student",
                    reader.GetString("brojIndeksa"),
                    reader.GetInt32("godinaStudija")
                );

                result.Add(student);
            }

            reader.Close();
            conn.Close();

            return result;
        }




        public static List<Student> GetStudents(string filter)
        {

            List<Student> result = new List<Student>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();

           
            cmd.CommandText = @"
        SELECT k.idKorisnik, k.ime, k.prezime, k.username, k.email, s.brojIndeksa, s.godinaStudija
        FROM Student s
        JOIN Korisnik k ON s.Korisnik_idKorisnik = k.idKorisnik
        WHERE k.Tip_Korisnika = 'student'
        AND (k.ime LIKE @filter 
             OR k.prezime LIKE @filter 
             OR k.prezime LIKE @filter 
             OR s.brojIndeksa LIKE @filter 
             OR s.godinaStudija LIKE @filter 
             OR k.username LIKE @filter)";
             

            cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");

            
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new Student(
                    reader.GetInt32("idKorisnik"),
                    reader.GetString("ime"),
                        reader.GetString("prezime"),
                        reader.GetString("email"),
                        reader.GetString("username"),
                        "",
                        "student",
                        reader.GetString("brojIndeksa"),
                        reader.GetInt32("godinaStudija")));
            }


            reader.Close();
            conn.Close();
            return result;
        }

        public static bool dodajStudenta(Student student)
        {
          
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand commandKorisnik = conn.CreateCommand();
                commandKorisnik.CommandText = @"
            INSERT INTO Korisnik (Ime, Prezime, Email, Username, Password, Tip_Korisnika,idKorisnik) 
            VALUES (@Ime, @Prezime, @Email, @Username, @Password, @TipKorisnika,@IdKorisnika);";  

                commandKorisnik.Parameters.AddWithValue("@Ime", student.ime);
                commandKorisnik.Parameters.AddWithValue("@Prezime", student.prezime);
                commandKorisnik.Parameters.AddWithValue("@Email", student.email);
                commandKorisnik.Parameters.AddWithValue("@Username", student.username);
                commandKorisnik.Parameters.AddWithValue("@Password", student.password);
                commandKorisnik.Parameters.AddWithValue("@TipKorisnika", "student");
                commandKorisnik.Parameters.AddWithValue("@IdKorisnika", student.idKorisnika);

                int korisnikRowsAffected = commandKorisnik.ExecuteNonQuery();
                if (korisnikRowsAffected == 0)
                {
                    throw new Exception("Unos u tabelu Korisnik nije uspešan.");
                }
                //   int korisnikId = Convert.ToInt32(commandKorisnik.ExecuteScalar());


                MySqlCommand commandStudent = conn.CreateCommand();
                commandStudent.CommandText = @"
            INSERT INTO Student (Korisnik_idKorisnik, BrojIndeksa, GodinaStudija)
            VALUES (@KorisnikId, @BrojIndeksa, @GodinaStudija)";

                commandStudent.Parameters.AddWithValue("@KorisnikId", student.idKorisnika);
                commandStudent.Parameters.AddWithValue("@BrojIndeksa", student.BrojIndeksa);
                commandStudent.Parameters.AddWithValue("@GodinaStudija", student.GodinaStudija);
                int rowsAffected = commandStudent.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške: " + ex.Message);
                return false;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
    public static Student GetStudent(string username)
        {
            Student student = null;
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                MySqlCommand commandKorisnik = conn.CreateCommand();
                commandKorisnik.CommandText = @"
            SELECT k.idKorisnik, k.Ime, k.Prezime, k.Username, k.Email, k.Tip_korisnika, k.Password,s.BrojIndeksa, s.GodinaStudija
            FROM korisnik k
            INNER JOIN Student s ON s.Korisnik_idKorisnik = k.idKorisnik
            WHERE k.Username = @Username";
                commandKorisnik.Parameters.AddWithValue("@Username", username);

                using (MySqlDataReader reader = commandKorisnik.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idKorisnik = reader["idKorisnik"] != DBNull.Value ? Convert.ToInt32(reader["idKorisnik"]) : 0;
                        int godinaStudija = reader["GodinaStudija"] != DBNull.Value ? Convert.ToInt32(reader["GodinaStudija"]) : 0;
                        student = new Student(
                    idKorisnik,
                    reader["Ime"].ToString(),
                    reader["Prezime"].ToString(),
                    reader["Email"].ToString(),
                    reader["Username"].ToString(),
                    reader["Password"].ToString(),
                    reader["Tip_korisnika"].ToString(),
                    reader["BrojIndeksa"].ToString(),
                    godinaStudija);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("MySQL greška: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return student;
        }

        public static bool obrisiStudenta(int idStudenta)
        {
            Console.WriteLine("Poziva se funkcija za brisanje studenta!");
            Console.WriteLine("Identifikator studenta: " + idStudenta);

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                using (MySqlCommand command = conn.CreateCommand())
                {
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            command.Transaction = transaction;

                            command.CommandText = "DELETE FROM Ispit_Student WHERE Student_Korisnik_idKorisnik = @StudentId";
                            command.Parameters.AddWithValue("@StudentId", idStudenta);
                            command.ExecuteNonQuery();

                            command.Parameters.Clear();
                            command.CommandText = "DELETE FROM DomaciZadatak_Student WHERE Student_Korisnik_idKorisnik = @StudentId";
                            command.Parameters.AddWithValue("@StudentId", idStudenta);
                            command.ExecuteNonQuery();

                            command.Parameters.Clear();
                            command.CommandText = "DELETE FROM Predmet_Student WHERE Student_Korisnik_idKorisnik = @StudentId";
                            command.Parameters.AddWithValue("@StudentId", idStudenta);
                            command.ExecuteNonQuery();

                            command.Parameters.Clear();
                            command.CommandText = "DELETE FROM Student WHERE Korisnik_idKorisnik = @StudentId";
                            command.Parameters.AddWithValue("@StudentId", idStudenta);
                            command.ExecuteNonQuery();

                            command.Parameters.Clear();
                            command.CommandText = "DELETE FROM Korisnik WHERE idKorisnik = @StudentId";
                            command.Parameters.AddWithValue("@StudentId", idStudenta);
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine("Došlo je do greške: " + ex.Message);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške pri povezivanju sa bazом: " + ex.Message);
                return false;
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }



        public static bool AzurirajStudenta(Student student)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                using (MySqlCommand command = conn.CreateCommand())
                {
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            command.Transaction = transaction;

                            // Ažuriranje podataka u tabeli Korisnik
                            command.CommandText = @"
                        UPDATE Korisnik 
                        SET Ime = @Ime, Prezime = @Prezime, Email = @Email
                        WHERE idKorisnik = @KorisnikId";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@Ime", student.ime);
                            command.Parameters.AddWithValue("@Prezime", student.prezime);
                            command.Parameters.AddWithValue("@Email", student.email);
                          
                            command.Parameters.AddWithValue("@KorisnikId", student.idKorisnika);
                            command.ExecuteNonQuery();

                            // Ažuriranje podataka u tabeli Student
                            command.CommandText = @"
                        UPDATE Student 
                        SET BrojIndeksa = @BrojIndeksa, GodinaStudija = @GodinaStudija 
                        WHERE Korisnik_idKorisnik = @KorisnikId";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@BrojIndeksa", student.BrojIndeksa);
                            command.Parameters.AddWithValue("@GodinaStudija", student.GodinaStudija);
                            command.Parameters.AddWithValue("@KorisnikId", student.idKorisnika);
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            return true;
                         
                        }
                        catch (Exception ex)
                        {
                            // Poništavanje transakcije u slučaju greške
                            transaction.Rollback();
                            Console.WriteLine("Došlo je do greške prilikom ažuriranja: " + ex.Message);
                            return false;
                           
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške pri povezivanju sa bazom: " + ex.Message);
                return false;
            }
            finally
            {
                // Zatvaranje konekcije
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


    }

}
