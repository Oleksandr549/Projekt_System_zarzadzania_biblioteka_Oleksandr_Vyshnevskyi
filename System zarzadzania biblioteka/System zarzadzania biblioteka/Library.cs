using System;
using System.Data;
using System.Data.SqlClient;

namespace LibraryManagementSystem
{
    public class Library
    {
        private readonly string connectionString;

        public Library()
        {
            connectionString = "Server=OLEKSANDRVY\\OLEKSANDRVY;Database=LibraryDB;Trusted_Connection=True;";
        }

        public void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("System Zarządzania Biblioteką");
                Console.WriteLine("1. Zarządzanie Książkami");
                Console.WriteLine("2. Zarządzanie Czytelnikami");
                Console.WriteLine("3. Wypożycz Książkę");
                Console.WriteLine("4. Zwróć Książkę");
                Console.WriteLine("5. Wyjdź");
                Console.Write("Wybierz opcję: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ManageBooks();
                        break;
                    case "2":
                        ManageReaders();
                        break;
                    case "3":
                        IssueBook();
                        break;
                    case "4":
                        ReturnBook();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Nieprawidłowa opcja! Naciśnij Enter, aby kontynuować.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void ManageBooks()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Zarządzanie Książkami");
                Console.WriteLine("1. Dodaj Książkę");
                Console.WriteLine("2. Wyświetl Książki");
                Console.WriteLine("3. Powrót do Menu Głównego");
                Console.Write("Wybierz opcję: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        ViewBooks();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Nieprawidłowa opcja! Naciśnij Enter, aby kontynuować.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void AddBook()
        {
            Console.Write("Wpisz tytuł książki: ");
            string title = Console.ReadLine();
            Console.Write("Wpisz autora książki: ");
            string author = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Books (Title, Author) VALUES (@Title, @Author)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Author", author);

                connection.Open();
                command.ExecuteNonQuery();
                Console.WriteLine("Książka została dodana pomyślnie! Naciśnij Enter, aby kontynuować.");
                Console.ReadLine();
            }
        }

        private void ViewBooks()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Books";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Książki:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]}: {reader["Title"]} autorstwa {reader["Author"]} - {((bool)reader["IsAvailable"] ? "Dostępna" : "Wypożyczona")}");
                    }
                }
            }
            Console.WriteLine("Naciśnij Enter, aby wrócić.");
            Console.ReadLine();
        }

        private void ManageReaders()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Zarządzanie Czytelnikami");
                Console.WriteLine("1. Zarejestruj Czytelnika");
                Console.WriteLine("2. Wyświetl Czytelników");
                Console.WriteLine("3. Powrót do Menu Głównego");
                Console.Write("Wybierz opcję: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterReader();
                        break;
                    case "2":
                        ViewReaders();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Nieprawidłowa opcja! Naciśnij Enter, aby kontynuować.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void RegisterReader()
        {
            Console.Write("Wpisz imię czytelnika: ");
            string name = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Readers (Name) VALUES (@Name)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);

                connection.Open();
                command.ExecuteNonQuery();
                Console.WriteLine("Czytelnik został zarejestrowany pomyślnie! Naciśnij Enter, aby kontynuować.");
                Console.ReadLine();
            }
        }

        private void ViewReaders()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Readers";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Czytelnicy:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Id"]}: {reader["Name"]}");
                    }
                }
            }
            Console.WriteLine("Naciśnij Enter, aby wrócić.");
            Console.ReadLine();
        }

        private void IssueBook()
        {
            Console.Write("Wpisz ID książki do wypożyczenia: ");
            int bookId = int.Parse(Console.ReadLine());
            Console.Write("Wpisz ID czytelnika: ");
            int readerId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string checkQuery = "SELECT IsAvailable FROM Books WHERE Id = @BookId";
                SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@BookId", bookId);

                connection.Open();
                var isAvailable = (bool?)checkCommand.ExecuteScalar();

                if (isAvailable == true)
                {
                    string issueQuery = "INSERT INTO Operations (BookId, ReaderId, IsReturn) VALUES (@BookId, @ReaderId, 0);" +
                                        "UPDATE Books SET IsAvailable = 0 WHERE Id = @BookId";
                    SqlCommand issueCommand = new SqlCommand(issueQuery, connection);
                    issueCommand.Parameters.AddWithValue("@BookId", bookId);
                    issueCommand.Parameters.AddWithValue("@ReaderId", readerId);

                    issueCommand.ExecuteNonQuery();
                    Console.WriteLine("Książka została wypożyczona pomyślnie! Naciśnij Enter, aby kontynuować.");
                }
                else
                {
                    Console.WriteLine("Książka nie jest dostępna! Naciśnij Enter, aby kontynuować.");
                }
            }
            Console.ReadLine();
        }

        private void ReturnBook()
        {
            Console.Write("Wpisz ID książki do zwrotu: ");
            int bookId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string returnQuery = "INSERT INTO Operations (BookId, ReaderId, IsReturn) VALUES (@BookId, NULL, 1);" +
                                     "UPDATE Books SET IsAvailable = 1 WHERE Id = @BookId";
                SqlCommand returnCommand = new SqlCommand(returnQuery, connection);
                returnCommand.Parameters.AddWithValue("@BookId", bookId);

                connection.Open();
                returnCommand.ExecuteNonQuery();
                Console.WriteLine("Książka została zwrócona pomyślnie! Naciśnij Enter, aby kontynuować.");
            }
            Console.ReadLine();
        }
    }
}
