using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Controls;
using System.Windows;
using System.Security.Permissions;
using System.Collections.ObjectModel;

namespace ASystem
{
    public class Ispit
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;
        public double bodovi { get; set; }
        public int ocjena { get; set; }
        public DateTime datumIspita { get; set; }
        public Student Student { get; set; }
        public int studentId { get; set; }       
        public int predmetId { get; set; }

        public Ispit(Student student, double bodovii, int ocjena, DateTime datum)
        {
            Student = student;
            bodovi = bodovii;
            datumIspita = datum;
        }
        public Ispit() { }
     
        public static bool sacuvajIspit(double bodovi, int ocjena, DateTime datum,Predmet predmet, Student student)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MessageBox.Show("Sacuvaj ispit:" + bodovi + " " + ocjena + " " + predmet.idPredmeta + " " + student.idKorisnika);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT into Ispit_Student(Bodovi,Ocjena,Ispit_DatumIspita,Student_Korisnik_idKorisnik,Predmet_idPredmeta) values (@Bodovi,@Ocjena,@Datum,@Id,@Predmet)";
                command.Parameters.AddWithValue("@Bodovi", bodovi);
                command.Parameters.AddWithValue("@Ocjena", ocjena);
                command.Parameters.AddWithValue("@Datum", datum);
                command.Parameters.AddWithValue("@Id", student.idKorisnika);
                command.Parameters.AddWithValue("@Predmet",predmet.idPredmeta);
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
               MessageBox.Show("Došlo je do greške: " + ex.Message);
                return false;
            }
            finally
            {
                // Zatvaramo vezu sa bazom (ako nije već zatvorena)
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public static ObservableCollection<Ispit> pregledIspita(Predmet predmet, Student student)
        {
            ObservableCollection<Ispit> ispiti = new ObservableCollection<Ispit>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = " SELECT Ocjena, Bodovi, Ispit_DatumIspita, p.idPredmeta, s.Korisnik_idKorisnik " +
                    " from Ispit_Student ip  inner join Student s on s.Korisnik_idKorisnik = ip.Student_Korisnik_idKorisnik" +
                    "  inner join Predmet p on p.idPredmeta = ip.Predmet_idPredmeta  where Predmet_idPredmeta = @parametarPredmet  and  Korisnik_idKorisnik = @parametarStudent";
             
                command.Parameters.AddWithValue("@parametarPredmet", predmet.idPredmeta);
                command.Parameters.AddWithValue("@parametarStudent", student.idKorisnika);

                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    
                    Ispit ispit = new Ispit
                    {
                        bodovi = reader.GetDouble("Bodovi"),
                        ocjena = reader.GetInt32("Ocjena"),
                        datumIspita = reader.GetDateTime("Ispit_DatumIspita"),
                        Student=student,
                        predmetId = predmet.idPredmeta
                    };
                    ispiti.Add(ispit);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Doslo je do greske:" + ex.Message);

            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
    
            return ispiti;
        }
        }
    }