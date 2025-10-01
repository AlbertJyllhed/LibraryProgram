namespace LibraryProgram
{
    internal class Program
    {
        public static string[] books = ["Pride and Prejudice", "The Great Gatsby", "Moby-Dick", "Les Miserables", "Frankenstein"];
        public static int[] bookAmounts = [6, 8, 5, 10, 13];
        //keeps track of all the books the library has loaned out
        public static int[][] loanedBooks = new int[books.Length][];

        public static string[] usernames = ["Anna", "Bob", "Cecilia", "David", "Eva"];
        public static int[] pins = [1234, 2345, 3456, 4567, 5678];
        //keeps track of all the books that the different users have in their possession
        public static int[][] userBookLoans = new int[usernames.Length][];
        //used to keep track of which user is currently logged in
        public static int currentUserIndex = -1;


        static void Main(string[] args)
        {
            //initialize the userBookLoans array
            for (int i = 0; i < userBookLoans.Length; i++)
            {
                userBookLoans[i] = [0, 0, 0, 0, 0];
            }

            currentUserIndex = LogIn();

            while (currentUserIndex > -1)
            {
                int input = DisplayMenu();
                Console.Clear();
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
                        Console.WriteLine("Felaktig inmatning.");
                        break;
                }

                Console.WriteLine("\nTryck Enter för att gå till huvudmenyn.");
                Console.ReadKey();
            }
        }


        //writes out all the standard menu options and returns the user input
        static int DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Visa böcker");
            Console.WriteLine("2. Låna bok");
            Console.WriteLine("3. Lämna tillbaka bok");
            Console.WriteLine("4. Mina lån");
            Console.WriteLine("5. Logga ut");

            return GetInputInt();
        }


        static void ShowBooks()
        {
            for (int i = 0; i < books.Length; i++)
            {
                Console.WriteLine($"{i}: {books[i]}, Tillgängliga exemplar: {bookAmounts[i]}");
            }
        }


        static void BorrowBook()
        {
            Console.WriteLine("Vilken bok vill du låna?");
            for (int i = 0; i < books.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {books[i]}");
            }

            int input = GetInputInt() - 1;
            if (bookAmounts[input] > 0)
            {
                userBookLoans[currentUserIndex][input] += 1;
                bookAmounts[input] -= 1;
                Console.WriteLine($"{usernames[currentUserIndex]} lånar en kopia av {books[input]}");
            }
            else
            {
                Console.WriteLine($"Det finns inga fler exemplar av {books[input]} att låna.");
            }
        }


        static void ReturnBook()
        {
            Console.WriteLine("Vilken bok vill du lämna tillbaka?");
            for (int i = 0; i < userBookLoans[currentUserIndex].Length; i++)
            {
                if (userBookLoans[currentUserIndex][i] > 0)
                {
                    Console.WriteLine($"{i + 1}. {books[i]}");
                }
            }
        }


        static void CheckBorrowedBooks()
        {
            Console.WriteLine($"{usernames[currentUserIndex]}s lån:");
            for (int i = 0; i < userBookLoans[currentUserIndex].Length; i++)
            {
                if (userBookLoans[currentUserIndex][i] > 0)
                {
                    Console.WriteLine($"{books[i]}: {userBookLoans[currentUserIndex][i]} lånade");
                }
            }
        }


        static int LogIn()
        {
            int tries = 0, userIndex = -1;
            while (tries < 3)
            {
                Console.Clear();
                Console.WriteLine("Välkommen till bibliotekets lånesystem!");
                Console.WriteLine("Skriv in ditt användarnamn:");
                string? username = Console.ReadLine();
                Console.WriteLine("Skriv in din PIN-kod:");
                int pin = 0;
                if (!int.TryParse(Console.ReadLine(), out pin))
                {
                    tries++;
                    continue;
                }

                for (int i = 0; i < usernames.Length; i++)
                {
                    if (username == usernames[i] && pin == pins[i])
                    {
                        userIndex = i;
                        return userIndex;
                    }
                }

                tries++;
            }

            return -1;
        }


        static void LogOut()
        {
            Console.WriteLine($"{usernames[currentUserIndex]} loggas ut.");
            Console.WriteLine("Tryck Enter för att fortsätta.");
            Console.ReadKey();
            currentUserIndex = LogIn();
        }


        static int GetInputInt()
        {
            int input = 0;
            while (!int.TryParse(Console.ReadLine(), out input))
            {
                Console.WriteLine($"Felaktig inmatning, försök igen.");
            }
            return input;
        }
    }
}
