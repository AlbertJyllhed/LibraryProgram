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
            //initialize both of the jagged arrays
            InitializeJaggedArray(loanedBooks);
            InitializeJaggedArray(userBookLoans);

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
                int availableBooks = bookAmounts[i] - loanedBooks[i][i];
                Console.WriteLine($"{i}: {books[i]}, Tillgängliga exemplar: {bookAmounts[i]}");
            }
        }


        static void BorrowBook()
        {
            Console.WriteLine("Vilken bok vill du låna?");
            for (int i = 0; i < books.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {books[i]}, Tillgängliga exemplar: {bookAmounts[i]}");
            }

            int input = GetInputInt() - 1;
            if (bookAmounts[input] > 0)
            {
                loanedBooks[currentUserIndex][input]++;
                userBookLoans[currentUserIndex][input]++;
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

            int input = GetInputInt() - 1;
            if (userBookLoans[currentUserIndex][input] > 0)
            {
                loanedBooks[currentUserIndex][input]--;
                userBookLoans[currentUserIndex][input]--;
                Console.WriteLine($"{usernames[currentUserIndex]} lämnar tillbaka en kopia av {books[input]}");
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


        //handles logging in to the library system
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


        //displays a logout message and then takes the user back to the login screen
        static void LogOut()
        {
            Console.WriteLine($"{usernames[currentUserIndex]} loggas ut.");
            Console.WriteLine("Tryck Enter för att fortsätta.");
            Console.ReadKey();
            currentUserIndex = LogIn();
        }


        //reusable method for input
        static int GetInputInt()
        {
            int input = 0;
            while (!int.TryParse(Console.ReadLine(), out input))
            {
                Console.WriteLine($"Felaktig inmatning, försök igen.");
            }
            return input;
        }


        static int[][] InitializeJaggedArray(int[][] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = [0, 0, 0, 0, 0];
            }

            return array;
        }
    }
}
