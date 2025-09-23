namespace LibraryProgram
{
    internal class Program
    {
        public static string[] books = ["Pride and Prejudice", "The Great Gatsby", "Moby-Dick", "Les Miserables", "Frankenstein"];
        public static int[] bookAmounts = [6, 8, 5, 10, 13];


        static void Main(string[] args)
        {
            string[] usernamnes = ["Anna", "Bob", "Cecilia", "David", "Eva"];
            int[] pins = [1234, 2345, 3456, 4567, 5678];
            bool loggedIn = false;

            Console.WriteLine("Välkommen till bibliotekets lånesystem!");

            int tries = 0;
            while (tries < 3 && !loggedIn)
            {
                Console.WriteLine("Skriv in ditt användarnamn:");
                string? username = Console.ReadLine();
                Console.WriteLine("Skriv in din PIN-kod:");
                int pin = int.Parse(Console.ReadLine());

                for (int i = 0; i < usernamnes.Length; i++)
                {
                    if (username == usernamnes[i] && pin == pins[i])
                    {
                        loggedIn = true;
                        break;
                    }
                }

                tries++;
            }

            if (loggedIn)
            {
                DisplayMenu();
            }
        }


        static void DisplayMenu()
        {
            Console.WriteLine("1. Visa böcker");
            Console.WriteLine("2. Låna bok");
            Console.WriteLine("3. Lämna tillbaka bok");
            Console.WriteLine("4. Mina lån");
            Console.WriteLine("5. Logga ut");

            int input = 0;
            bool success = int.TryParse(Console.ReadLine(), out input);
            switch (input)
            {
                case 1:
                    ShowBooks();
                    break;
                case 2:
                    BorrowBook();
                    break;
                case 3:
                    ReturnBook();
                    break;
                case 4:
                    CheckBorrowedBooks();
                    break;
                case 5:
                    LogOut();
                    break;
                default:
                    Console.WriteLine("Felaktig inmatning, försök igen.");
                    break;
            }
        }


        static void ShowBooks()
        {
            for (int i = 0;i < books.Length; i++)
            {
                Console.WriteLine($"{i}: {books[i]}\nTillgängliga exemplar: {bookAmounts[i]}");
            }
        }


        static void BorrowBook()
        {

        }


        static void ReturnBook()
        {

        }


        static void CheckBorrowedBooks()
        {

        }


        static void LogOut()
        {

        }
    }
}
