using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Crypto.Signers;


namespace ASystem
{
    public class Predmet
    {

        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["MySql_hci"].ConnectionString;
        public int idPredmeta;
        public String naziv;
        public String opis;
        public int eCTS;

        public string DisplayText => Naziv;
        public int IdPredmeta { get => idPredmeta; set => idPredmeta = value; }
        public int ECTS { get => eCTS; set => eCTS = value; }
        public String Naziv { get => naziv; set => naziv = value; }
        public string Opis { get => opis; set => opis = value; }

        //izlistavanje svih predmeta na kojim je profesor zaduzen
        public static ObservableCollection<Predmet> izlistajPredmete(Profesor profesor)
        {

            ObservableCollection<Predmet> predmeti = new ObservableCollection<Predmet>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText =
                @"SELECT 
    p.idPredmeta,
        p.Naziv, 
    p.Opis, 
    p.ECTS
 /*   k.Ime AS Profesor_Ime, 
    k.Prezime AS Profesor_Prezime, 
    k.Email AS Profesor_Email,
    k.Username AS Profesor_Username,
    pr.Zvanje AS Profesor_Zvanje*/
FROM 
    Predmet p
JOIN 
    Predmet_Profesor pp ON p.idPredmeta = pp.Predmet_idPredmeta
JOIN 
    Profesor pr ON pp.Profesor_Korisnik_idKorisnik = pr.Korisnik_idKorisnik
JOIN 
    Korisnik k ON pr.Korisnik_idKorisnik = k.idKorisnik
WHERE 
    pr.Korisnik_idKorisnik = @ProfesorId";

            cmd.Parameters.AddWithValue("@ProfesorId", Profesor.GetProfesorId(profesor));
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Naziv predmeta u funkciji izlistaj predmete " + reader.GetString("Naziv"));
                predmeti.Add(new Predmet()
                {
                    idPredmeta = reader.GetInt32("idPredmeta"),
                    naziv = reader.GetString("Naziv"),
                    opis = reader.GetString("Opis"),
                    eCTS = reader.GetInt32("ECTS")
                });
            }
            conn.Close();

            return predmeti;
        }


        public static bool dodajPredmet(Predmet predmet)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT into Predmet(Naziv,Opis,ECTS,idPredmeta) values (@Naziv,@Opis,@ECTS,@Id)";
                command.Parameters.AddWithValue("@Naziv", predmet.Naziv);
                command.Parameters.AddWithValue("@Opis", predmet.Opis);
                command.Parameters.AddWithValue("@ECTS", predmet.ECTS);
                command.Parameters.AddWithValue("@Id", predmet.idPredmeta);

                int rowsAffected = command.ExecuteNonQuery();

                // Ako je broj pogođenih redova veći od 0, znači da je upit uspešno izvršen
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


        public bool ažurirajPredmet(Predmet predmet)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();


                command.CommandText = "UPDATE Predmet SET Naziv = @Naziv, Opis = @Opis, ECTS = @ECTS WHERE idPredmeta = @Id";

                command.Parameters.AddWithValue("@Naziv", predmet.Naziv);
                command.Parameters.AddWithValue("@Opis", predmet.Opis);
                command.Parameters.AddWithValue("@ECTS", predmet.ECTS);
                command.Parameters.AddWithValue("@Id", predmet.idPredmeta);

                int rowsAffected = command.ExecuteNonQuery();


                return rowsAffected > 0;
            }
            catch (Exception ex)
            {

                Console.WriteLine("Došlo je do greške: " + ex.Message);
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

        public static ObservableCollection<string> GetPredmeti(int profesorId)
        {
            ObservableCollection<string> predmeti = new ObservableCollection<string>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand command = conn.CreateCommand();
                    command.CommandText = @"
                SELECT p.Naziv
FROM Predmet p
INNER JOIN Predmet_Profesor pp ON pp.Predmet_idPredmeta = p.idPredmeta
  WHERE pp.Profesor_Korisnik_idKorisnik = @ProfesorId";
                    command.Parameters.AddWithValue("@ProfesorId", profesorId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["Naziv"].ToString());
                            predmeti.Add(reader["Naziv"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Došlo je do greške: " + ex.Message);
                }
            }
            return predmeti;
        }


        public static ObservableCollection<Predmet> pregledPredmeta()
        {
            ObservableCollection<Predmet> predmeti = new ObservableCollection<Predmet>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = @"select  p.idPredmeta, p.Naziv, p.Opis, p.ECTS from predmet p;";

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Predmet predmet = new Predmet
                    {
                        idPredmeta = reader.GetInt32("idPredmeta"),
                        Naziv = reader.GetString("Naziv"),
                        Opis = reader.GetString("Opis"),
                        ECTS = reader.GetInt32("ECTS")
                    };

                    predmeti.Add(predmet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Došlo je do greške: " + ex.Message);

            }
            finally
            {

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return predmeti;
        }
        public static Boolean jeZaduzen(Profesor profesor, Predmet predmet)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"
            SELECT EXISTS (
                SELECT 1
                FROM mydb.Predmet_Profesor pp
                WHERE pp.Profesor_Korisnik_idKorisnik = @filter1
                AND pp.Predmet_idPredmeta = @filter2
            ) AS ProfesorZaduzenZaPredmet";


                cmd.Parameters.AddWithValue("@filter1", profesor.idKorisnika);
                cmd.Parameters.AddWithValue("@filter2", predmet.idPredmeta);


                MySqlDataReader reader = cmd.ExecuteReader();


                if (reader.Read())
                {

                    return reader.GetBoolean("ProfesorZaduzenZaPredmet");
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool profesorPredmet(Profesor profesor, Predmet predmet)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "INSERT into Predmet_Profesor(Predmet_idPredmeta,Profesor_Korisnik_idKorisnik) values (@idPredmeta,@idKorisnika)";
                command.Parameters.AddWithValue("@idPredmeta", predmet.idPredmeta);
                command.Parameters.AddWithValue("@idKorisnika", profesor.idKorisnika);

                int rowsAffected = command.ExecuteNonQuery();
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

        public static bool razduzi(Profesor profesor, Predmet predmet)
        {

            MySqlConnection conn = new MySqlConnection(connectionString);
            try
            {
                MessageBox.Show("Identifikator profesora:" + profesor.idKorisnika + "Identifikator predmeta: " + predmet.idPredmeta);
                conn.Open();
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "DELETE from Predmet_Profesor WHERE Predmet_idPredmeta=@idPredmeta AND Profesor_Korisnik_idKorisnik=@idKorisnika";
                command.Parameters.AddWithValue("@idPredmeta", predmet.idPredmeta);
                command.Parameters.AddWithValue("@idKorisnika", profesor.idKorisnika);

                int rowsAffected = command.ExecuteNonQuery();
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

        public static bool ObrisiPredmet(int idPredmeta)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (MySqlCommand command = conn.CreateCommand())
                        {
                            command.Transaction = transaction;
                            command.CommandText = "DELETE FROM Predmet_Profesor WHERE Predmet_idPredmeta = @IdPredmeta";
                            command.Parameters.AddWithValue("@IdPredmeta", idPredmeta);
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();

                     
                            command.CommandText = "DELETE FROM Predmet_Student WHERE Predmet_idPredmeta = @IdPredmeta";
                            command.Parameters.AddWithValue("@IdPredmeta", idPredmeta);
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();

                        
                            command.CommandText = "DELETE FROM DomaciZadatak WHERE Predmet_idPredmeta = @IdPredmeta";
                            command.Parameters.AddWithValue("@IdPredmeta", idPredmeta);
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();

                          
                            command.CommandText = "DELETE FROM Prisustvo WHERE Predmet_idPredmeta = @IdPredmeta";
                            command.Parameters.AddWithValue("@IdPredmeta", idPredmeta);
                            command.ExecuteNonQuery();
                            command.Parameters.Clear();

                         
                            command.CommandText = "DELETE FROM Predmet WHERE idPredmeta = @IdPredmeta";
                            command.Parameters.AddWithValue("@IdPredmeta", idPredmeta);
                            int rowsAffected = command.ExecuteNonQuery();
                            transaction.Commit();

                            return rowsAffected > 0;
                        }
                    }
                    catch (Exception ex)
                    {
   
                        transaction.Rollback();
                        MessageBox.Show("Došlo je do greške: " + ex.Message);
                        return false;
                    }
                }
            }
        }



        public static List<Predmet> GetPredmeti(string filter)
        {
            List<Predmet> predmeti = new List<Predmet>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.CommandText= @"SELECT * FROM Predmet WHERE Naziv LIKE @str OR idPredmeta Like @str";
            cmd.Parameters.AddWithValue("@str", filter + "%");
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Predmet predmet = new Predmet
                {
                    idPredmeta = reader.GetInt32("idPredmeta"),  
                    Naziv = reader.GetString("Naziv"),
                    Opis = reader.GetString("Opis"),
                    ECTS = reader.GetInt32("ECTS")
                };
                predmeti.Add(predmet);
            }
            reader.Close();
            conn.Close();
            return predmeti;

        }


        public static ObservableCollection<Student> studentiSlusaju(Predmet predmet)
        {
            ObservableCollection<Student> studenti = new ObservableCollection<Student>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand cmd = conn.CreateCommand(); //BrojIndeksa, GodinaStudija
            cmd.CommandText= @"select idKorisnik, ime, prezime,email,username, password, s.BrojIndeksa, s.GodinaStudija
from Korisnik k
inner join Student s on k.idKorisnik = s. Korisnik_idKorisnik 
inner join Predmet_Student ps on ps.Student_Korisnik_idKorisnik = s.Korisnik_idKorisnik
where Predmet_idPredmeta = @str";
            cmd.Parameters.AddWithValue("@str", predmet.idPredmeta);
          
            MySqlDataReader reader = cmd.ExecuteReader();
          
            while (reader.Read())
            {
              //  int idKorisnik = reader["idKorisnik"] != DBNull.Value ? Convert.ToInt32(reader["idKorisnik"]) : 0;
                Student student = new Student(
                        (int)reader["idKorisnik"],
                        reader.GetString("ime"),
                        reader.GetString("prezime"),
                        reader.GetString("email"),
                        reader.GetString("username"),
                        reader.GetString("password"),
                        "student",
                        reader.GetString("BrojIndeksa"),
                        reader.GetInt32("GodinaStudija")
                    );

                studenti.Add(student);
             
                }
            reader.Close();
            conn.Close();
            return studenti;

        }


        public static List<Predmet> studentiSlusajuPredmet(Student student)
        {
         
            List<Predmet> predmeti = new List<Predmet>();
            MySqlConnection conn = new MySqlConnection(connectionString);
            conn.Open();
            MySqlCommand command = conn.CreateCommand();
            command.CommandText = @"select Naziv, Opis, ECTS, idPredmeta
from Predmet p
inner join Predmet_Student ps  ON ps.Predmet_idPredmeta = p.idPredmeta
inner join Student s on s.Korisnik_idKorisnik = ps.Student_Korisnik_idKorisnik
where  Student_Korisnik_idKorisnik =" + student.idKorisnika;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                predmeti.Add(new Predmet()
                {
                    idPredmeta = reader.GetInt32("idPredmeta"),
                    naziv = reader.GetString("Naziv"),
                    opis = reader.GetString("Opis"),
                    eCTS = reader.GetInt32("ECTS")
                }
            ); }
            reader.Close();
            conn.Close();
            return predmeti;
        }

        public static bool AzurirajPredmet(Predmet predmet)
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
                            command.CommandText = @"
                        UPDATE Predmet 
                        SET naziv = @Naziv, ECTS = @BrojECTS, Opis = @Opis
                        WHERE idPredmeta = @PredmetId";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@Naziv", predmet.Naziv);
                            command.Parameters.AddWithValue("@BrojECTS", predmet.eCTS);
                            command.Parameters.AddWithValue("@Opis", predmet.opis);
         
                            command.Parameters.AddWithValue("@PredmetId", predmet.idPredmeta);

                            command.ExecuteNonQuery();

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                           
                            transaction.Rollback();
                            Console.WriteLine("Došlo je do greške prilikom ažuriranja predmeta: " + ex.Message);
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
             
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

    }
}

