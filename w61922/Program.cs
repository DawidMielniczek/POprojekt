
using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Text.RegularExpressions;
namespace w61922
{
    class Użytkownik
    {
        public string Imię;
        public string Nazwisko;
        public string Pesel;
    }

    class Książka
    {
        public string Tytuł;
        public string Autor;
        public string Dostępność { get; set; }
    }
    class Wypożyczenia
    {
        public int id_użytkownika;
        public int id_książki;
        public DateTime data_wyp;
    }
    class Biblioteka
    {
        public int id_użytkownika;
        public string opnia;
    }
    class pracownicy
    {
        public string imie;
        public string nazwisko;
        public DateTime data_zatrudnienia;
        public string pesel;
    }
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data source= DESKTOP-57VIT9O;database=Biblioteka_szkolna;Trusted_Connection=True";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            var Ksiązki = new List<Książka>();
            List<Użytkownik> Użytkownicy  = new List<Użytkownik>();
            MenuGłowne();



            void MenuAdmin()
            {

                Console.WriteLine("Wybierz operację jaką chesz wykonać:");
                Console.WriteLine("[1] - Dodaj ksiązkę do biblioteki.");
                Console.WriteLine("[2] - Usuń książkę z biblioteki.");
                Console.WriteLine("[3] - Usuń czytelnika z biblioteki.");
                Console.WriteLine("[0] - Wyloguj się.");
                var wybor = Console.ReadLine();
                switch (wybor)
                {
                    case "1":
                        DodajKsiązke();
                        break;
                    case "2":
                        UsuńKsiążkę();
                        break;
                    case "3":
                        UsuńUżytkownika();
                        break;
                    case "0":
                        Console.Clear();
                        MenuGłowne();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Nierpawidłowy znak");
                        MenuAdmin();
                        break;

                }
            }
            void LogowanieA()
            {

                Console.Clear();
                Console.WriteLine("Podaj Login:");
                var LoginU = Console.ReadLine();
                Console.WriteLine("Podaj Hasło:");
                var HasloU = Console.ReadLine();

                SqlCommand sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = "SELECT imie,Pesel FROM pracownicy";
                SqlDataReader reader = sqlCommand.ExecuteReader();

                bool Poprawne = true;
                while (reader.Read())
                {

                    if (reader["imie"].ToString().Trim() == LoginU && reader["Pesel"].ToString().Trim() == HasloU)
                    {
                        reader.Close();
                        Console.Clear();
                        Console.WriteLine("Witaj na koncie Administratora biblioteki szkolnej! ");
                        Poprawne = false;
                        Console.Clear();
                        MenuAdmin();
                        break;
                    }

                }
                if (Poprawne)
                {
                    Console.Clear();
                    reader.Close();
                    Console.WriteLine("Podano błedne dane! Login lub hasło jest niepoprawne.");
                    Console.WriteLine("Jeśli chcesz wyjść wciśnij 0");
                    var Key = Console.ReadLine();
                    if (Key == "0")
                    {
                        Console.Clear();
                        MenuGłowne();
                    }
                    else
                    {
                        LogowanieA();
                    }
                }

            }
            void MenuU(string Haslou)
            {
                Console.WriteLine("Wybierz operację jaką chesz wykonać:");
                Console.WriteLine("[1] - Pokaż dostępność książek.");
                Console.WriteLine("[2] - Pokaż aktualną listę wypżyczeń.");
                Console.WriteLine("[3] - Dodaj opinie o bibliotece.");
                Console.WriteLine("[4] - Sprawdź aktualne kary za nieterminowe oddawanie książek.");
                Console.WriteLine("[5] - Wypożycz książkę.");
                Console.WriteLine("[6] - Oddaj książkę.");
                Console.WriteLine("[0] - Wyloguj się.");
               
                var wybor = Console.ReadLine();
                
                switch (wybor)
                {
                    case "1":
                        Ilosc(Haslou);
                        break;
                    case "2":
                        DostępnośćK(Haslou);
                        break;
                    case "3":
                        Opinia(Haslou, connection, Użytkownicy);
                        break;
                    case "4":
                        Kary(Haslou);
                        break;
                    case "5":
                        Wypożycz(Haslou);
                        break;
                    case "6":
                        oddaj(Haslou);
                        break;

                    case "0":
                        Console.Clear();
                        MenuGłowne();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Nierpawidłowy znak");
                        MenuU(Haslou);
                        break;
                }
            }
            void DostępnośćK(string Haslou)
            {
                
                SqlCommand sql = connection.CreateCommand();
                sql.CommandText = @"Select Tytuł From Książka, Wypożyczenia, użytkownik where użytkownik.nr_użytkownika = Wypożyczenia.id_użytkownika AND Książka.id_książki = Wypożyczenia.id_książki AND użytkownik.pesel = @pesel";
                sql.Parameters.AddWithValue("pesel", Haslou);
                SqlDataReader reader = sql.ExecuteReader();
                Console.Clear();
                Console.WriteLine("Oto Lista tytułów, które aktualnie wypożyczasz");

                while (reader.Read())
                {
                    
                    Console.WriteLine(reader["Tytuł"].ToString());

                }
                reader.Close(); 
                Console.WriteLine();
                MenuU(Haslou);


            }
            void Opinia(string Haslou, SqlConnection connection, List<Użytkownik> Użytkownicy)
            {
                SqlCommand sql1 = connection.CreateCommand();
                sql1.CommandText = @"SELECT nr_użytkownika from użytkownik where pesel = @pesel";
                sql1.Parameters.AddWithValue("@pesel", Haslou);
                SqlDataReader reader = sql1.ExecuteReader();
                reader.Read();
                var Nr = reader["nr_użytkownika"].ToString();
                reader.Close();


                Console.WriteLine("Wystaw opinie Bibiliotece (max 150 znaków)");
                var opinia = Console.ReadLine();
                if (opinia.Length < 150 && opinia.Length != 0)
                {
                    
                    SqlCommand sql = connection.CreateCommand();
                    sql.CommandText = @"INSERT INTO[dbo].[Biblioteka]
                    ([id_użytkownika]
                      ,[Opinia]
                       )
                    VALUES
                       (
                        @Id,
                        @Opinia
                        )";
                    sql.Parameters.AddWithValue("@Id", Nr);
                    sql.Parameters.AddWithValue("@Opinia", opinia);
                    var rslt = sql.ExecuteNonQuery();
                    Console.Clear();
                    Console.WriteLine("Dodano " + rslt + " opinie.");

                    MenuU(Haslou);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Przekroczyłęś limit 150 znaków lub zatwierdziłeś puste pole!");
                    Console.WriteLine();
                    MenuU(Haslou);
                }
            }
            void DodajKsiązke()
            {
                Console.Clear();
                Console.WriteLine("Podaj Tytuł ksiązki");
                var Tytuł = Console.ReadLine();
                Console.WriteLine("Podaj Autora ksiązki");
                var Autor = Console.ReadLine();
               
                SqlCommand sql = connection.CreateCommand();

                sql.CommandText = @"INSERT INTO[dbo].[Książka]
                ([Tytuł]
                  ,[Autor]
                  ,[Dostępność]
                   )
                VALUES
                   (
                    @Tytuł,
                    @Autor,
                    @Ilość
                    )";

                sql.Parameters.AddWithValue("@Tytuł", Tytuł);
                sql.Parameters.AddWithValue("@Autor", Autor);
                sql.Parameters.AddWithValue("@Ilość", "tak");
                Ksiązki.Add(new Książka()
                {
                    Tytuł = Tytuł,
                    Autor = Autor,
                    Dostępność = "tak"
                }) ;
                
                int reslt = sql.ExecuteNonQuery();
                Console.Clear();
                Console.WriteLine("Dodano " + reslt + " Książek");

                MenuAdmin();
            }
            void UsuńKsiążkę()
            {
                Console.Clear();
                Console.WriteLine("Podaj Identyfikator ksiązki, którą chcesz usunąć:");
                var DoDel = Console.ReadLine();
                if (!Regex.IsMatch(DoDel, @"^[0-9]{1,11}$"))
                    {
                    Console.Clear();
                    Console.WriteLine("Podałeś niepoprawne dane!");
                    MenuAdmin();
                }
                else
                {
                    SqlCommand sql = connection.CreateCommand();
                    sql.CommandText = @"Delete from Książka where id_książki= @Nid";
                    sql.Parameters.AddWithValue("@Nid", Convert.ToInt32(DoDel));
                    int reslt = sql.ExecuteNonQuery();
                    Console.Clear();
                    Console.WriteLine("Usunięto " + reslt + " Książek");
                    MenuAdmin();
                }
            }
            void UsuńUżytkownika()
            {
                Console.Clear();
                Console.WriteLine("Podaj numer Pesel użytkownika, którego chcesz usunąć z bazy:");
                var Do_del = Console.ReadLine();
                SqlCommand sql = connection.CreateCommand();
                sql.CommandText = @"Delete from użytkownik where Pesel= @Pesel";
                sql.Parameters.AddWithValue("@Pesel", Do_del);
                int reslt = sql.ExecuteNonQuery();
                Console.Clear();
                Console.WriteLine("Usunięto " + reslt + " użytkownika");
                MenuAdmin();
            }
            void MenuGłowne()
                
            {

                Console.WriteLine("[1] Logowanie jako Administrator");
                Console.WriteLine("[2] Logowanie jako Użytkownik");
                Console.WriteLine("[3] Rejestracja");
                Console.WriteLine("[0] - Wyjscie z programu.");
                var wybor = Console.ReadLine();
                switch (wybor)
                {
                    case "1":
                        LogowanieA();
                        break;


                    case "2":
                        LogowanieU();
                        break;

                    case "3":
                        Rejestracja();
                        break;
                    case "0":
                        
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Nierpawidłowy znak");
                        MenuGłowne();
                        break;
                }
            }
            void Rejestracja()
            {
                Console.WriteLine("Podaj Imie:");
                var imie = Console.ReadLine();
                Console.WriteLine("Podaj Nazwisko:");
                var nazwisko = Console.ReadLine();
                Console.WriteLine("Podaj Pesel");
                var pesel = Console.ReadLine();
                bool Poprawnie = true;
                if (!Regex.IsMatch(imie, @"^\p{Lu}\p{Ll}{1,12}$") ||
                 !Regex.IsMatch(nazwisko, @"^\p{Lu}\p{Ll}{1,12}$") ||
                 !Regex.IsMatch(pesel, @"^[0-9]{11,11}$"))
                {
                    Console.Clear();
                    Console.WriteLine("Nieprawidłowe dane");
                    MenuGłowne();
                    Poprawnie = false;
                    
                }
                else
                {
                    SqlCommand sql1 = connection.CreateCommand();
                    sql1.CommandText = @"SELECT Pesel From użytkownik";
                    SqlDataReader reader = sql1.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["Pesel"].ToString() == pesel)
                        {
                            Console.Clear();
                            reader.Close();
                            Console.WriteLine("Podany Pesel jest już przypisany do danego konta lub jest za krótki.");
                            MenuGłowne();
                            Poprawnie = false;
                            break;
                        }


                    }
                    reader.Close();
                }
                if (Poprawnie)
                {
                   
                    Użytkownicy.Add(new Użytkownik()
                    {
                        Imię = imie,
                        Nazwisko = nazwisko,
                        Pesel = pesel
                    });



                    SqlCommand sql = connection.CreateCommand();
                    sql.CommandText = @"INSERT INTO [dbo].[użytkownik]
                   ([Imie],
                    [Nazwisko],
                    [Pesel]
                   )
                VALUES
                   (
                    @imie,
                    @nazwisko,
                    @pesel
                    )";
                    sql.Parameters.AddWithValue("Imie", imie);
                    sql.Parameters.AddWithValue("Nazwisko", nazwisko);
                    sql.Parameters.AddWithValue("Pesel", pesel);

                    sql.ExecuteNonQuery();
                    Console.WriteLine("Konto założone! Możesz się logować!");
                    MenuGłowne();
                }
            }
            void LogowanieU()
            {
                
                Console.WriteLine("Podaj Login:");
                var LoginU = Console.ReadLine();
                Console.WriteLine("Podaj Hasło:");
                var HasloU = Console.ReadLine();

                SqlCommand sqlCommand = connection.CreateCommand();
                sqlCommand.CommandText = "SELECT imie,pesel FROM użytkownik";
                SqlDataReader reader = sqlCommand.ExecuteReader();
              
                bool Poprawne = true;
                while (reader.Read())
                {

                    if (reader["imie"].ToString().Trim() == LoginU && reader["pesel"].ToString().Trim() == HasloU)
                    {
                        reader.Close();
                        Console.Clear();
                        Console.WriteLine("Witaj na koncie Użytkownika biblioteki szkolnej! ");
                        Poprawne = false;
                        MenuU(HasloU);
                        break;
                    }
                   
                }
                if (Poprawne)
                {
                    Console.Clear();
                    Console.WriteLine("Podano błedne dane! Login lub hasło jest niepoprawne.");
                    reader.Close();
                    MenuGłowne();
                    
                }
            }
            
            void Ilosc(string haslou)
            {
                Console.Clear();
                Console.WriteLine("Podaj tytuł ksiązki, której chcesz sprawdzić ilość dostępnych sztuk.");
                var Wybor = Console.ReadLine();
                SqlCommand sql = connection.CreateCommand();
                sql.CommandText = @"Select Count(Tytuł) From Książka where Tytuł = @tytul";
                sql.Parameters.AddWithValue("@tytul", Wybor);
                var rslt = (int)sql.ExecuteScalar();
                Console.Clear();
                Console.WriteLine("Biblioteka posiada " + rslt + " sztuk podanej książki.");
                Console.WriteLine();
                MenuU(haslou);
                
            }
            void Kary(string haslou)
            {
                Console.Clear();
                SqlCommand sql = connection.CreateCommand();
                sql.CommandText = @"select pesel, DATEDIFF(DAY, data_wyp, GETDATE()) as 'Kara', Tytuł  from Wypożyczenia, użytkownik, Książka where  Wypożyczenia.id_użytkownika = użytkownik.nr_użytkownika AND Wypożyczenia.id_książki = Książka.id_książki
";              bool Jest = true;
                SqlDataReader reader = sql.ExecuteReader();
                
                while (reader.Read())
                {
                    if (reader["pesel"].ToString() == haslou)
                    {
                        
                        Jest = false;
                        var Ksiązka = reader["Tytuł"];
                        Console.WriteLine("Od twojego wypożyczenia ksiązki " + Ksiązka +  " mineło " + reader["Kara"].ToString() + " dni!" + " Po 30 dniach, po każdym dniu zwłoki naliczany jest koszt 2zł");
                        
                        if( Convert.ToInt32( reader["Kara"] ) > 30)
                        {
                            Console.WriteLine("Twoja aktualnie naliczona kara wynosi: " + ((Convert.ToInt32(reader["Kara"]) - 30)) *2 + " zł") ;
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine(" Pozostało Ci jeszcze " + (30 - (Convert.ToInt32(reader["Kara"])) + " dni do oddania ksiązki "));
                            Console.WriteLine();
                        }    
                    }
                }
                if (Jest)
                {
                    Console.Clear();
                    Console.WriteLine("Nie masz żadnych kar.");
                    Console.WriteLine();
                }
                
                reader.Close();
                MenuU(haslou);


            }
            void Wypożycz(string haslou)
            {
                Console.Clear();
                SqlCommand sql1 = connection.CreateCommand();
                sql1.CommandText = @"select * from użytkownik where pesel= @pesel";
                sql1.Parameters.AddWithValue("@pesel", haslou);

                SqlDataReader reader1 = sql1.ExecuteReader();
                
                reader1.Read();
                    var nrU = reader1["nr_użytkownika"].ToString();
                reader1.Close();

                Console.WriteLine("Podaj tytuł książki:");
                var Wybór = Console.ReadLine();

                
                SqlCommand sql = connection.CreateCommand();
                sql.CommandText = @"select id_książki from Książka where Tytuł = @tytul AND Dostępność = 'tak'";
                sql.Parameters.AddWithValue("@tytul", Wybór);
                SqlDataReader sqlR = sql.ExecuteReader();

                if (sqlR.Read())
                {
                    var NrK = sqlR["id_książki"];
                    sqlR.Close();

                    SqlCommand Sql2 = connection.CreateCommand();
                    Sql2.CommandText = @"Update Książka set Dostępność = 'nie' where id_książki = @NrK";
                    Sql2.Parameters.AddWithValue("NrK", NrK);
                    Sql2.ExecuteNonQuery();

                    SqlCommand sql3 = connection.CreateCommand();
                    sql3.CommandText = @"INSERT INTO[dbo].[Wypożyczenia]
                ([id_użytkownika]
                  ,[id_książki]
                  ,[data_wyp]
                   )
                VALUES
                   (
                    @NrU,
                    @NrK,
                    @data_wyp
                    )";

                    var Data = DateTime.Now;
                    sql3.Parameters.AddWithValue("NrU", nrU);
                    sql3.Parameters.AddWithValue("NrK", NrK);
                    sql3.Parameters.AddWithValue("data_wyp", Data);
                    sql3.ExecuteNonQuery();
                    Console.WriteLine("Wypożyczyłeś ksiązkę " + Wybór);
                    MenuU(haslou);
                }
                else
                {
                    sqlR.Close();
                    Console.Clear();
                    Console.WriteLine("Nie ma takiej ksiązki w bazie lub jest niedostępna.");
                    MenuU(haslou);
                }
            }
            void oddaj(string haslou)
            {
                Console.Clear();
                SqlCommand sql1 = connection.CreateCommand();
                sql1.CommandText = @"select nr_użytkownika from użytkownik where pesel= @pesel";
                sql1.Parameters.AddWithValue("@pesel", haslou);

                SqlDataReader reader1 = sql1.ExecuteReader();

                reader1.Read();
                var NrU = reader1["nr_użytkownika"].ToString();
                reader1.Close();




                Console.WriteLine("Podaj tytuł książki, którą chcesz oddać: ");
                var Wybór = Console.ReadLine();

                SqlCommand sql2 = connection.CreateCommand();
                sql2.CommandText = @"Select Wypożyczenia.id_książki  from Książka, Wypożyczenia where Wypożyczenia.id_książki  = Książka.id_książki AND Książka.Tytuł = @Wybór and Wypożyczenia.id_użytkownika = @NrU";
                sql2.Parameters.AddWithValue("@Wybór", Wybór);
                sql2.Parameters.AddWithValue("@NrU", NrU);
                SqlDataReader reader = sql2.ExecuteReader();
                if (reader.Read())
                {
                    var IdK = reader["id_książki"];
                    reader.Close();
                    SqlCommand sql3 = connection.CreateCommand();
                    sql3.CommandText = @"Update Książka set Dostępność = 'tak' where id_książki = @IdK";
                    sql3.Parameters.AddWithValue("@IdK", IdK);
                    sql3.ExecuteNonQuery();

                    SqlCommand sql4 = connection.CreateCommand();
                    sql4.CommandText = @"Delete from Wypożyczenia where id_książki = @IdK";
                    sql4.Parameters.AddWithValue("@IdK", IdK);
                    sql4.ExecuteNonQuery();

                    Console.Clear();
                    Console.WriteLine("Oddałeś ksiązkę " + Wybór);
                    MenuU(haslou);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Podałeś błedny tytuł lub nie masz wypożyczonej ksiązki o podanej nazwie.");
                    reader.Close();
                    MenuU(haslou);
                }
            }
        }
    }
}
