using System.Linq.Expressions;

namespace LibraryProgram
{
    internal class Program
    {
        public static string[] books = ["Pride and Prejudice", "The Great Gatsby", "Moby-Dick", "Les Miserables", "Frankenstein"];
        public static int[] bookAmounts = [6, 8, 5, 10, 13];
        //keeps track of all the books the library has loaned out
        public static int[] loanedBooks = new int[books.Length];

        public static string[] usernames = ["Anna", "Bob", "Cecilia", "David", "Eva"];
        public static int[] pins = [1234, 2345, 3456, 4567, 5678];
        public static bool[] admin = [true, false, true, false, false];
        //keeps track of all the books that the different users have in their possession
        public static string[][] userLoans = new string[usernames.Length][];
        public static DateTime[][] returnDates = new DateTime[usernames.Length][];
        public static int maxLoans = 10;
        //used to keep track of which user is currently logged in
        public static int userIndex = -1;


        static void Main(string[] args)
        {
            Console.Title = "LibraryProgram";
            SetupLibrary();
            userIndex = LogIn();

            while (userIndex > -1)
            {
                int input = DisplayMenu();
                Console.Clear();
                switch (input)
                {
                    case 1:
                        ShowBooks();
                        break;
                    case 2:
                        SearchForBook();
                        break;
                    case 3:
                        BorrowBook();
                        break;
                    case 4:
                        ReturnBook();
                        break;
                    case 5:
                        CheckBorrowedBooks();
                        break;
                    case 6:
                        AddBook();
                        break;
                    case 7:
                        AddUser();
                        break;
                    case 8:
                        RemoveUser();
                        break;
                    case 9:
                        userIndex = LogIn();
                        break;
                    case 0:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Felaktig inmatning.");
                        break;
                }

                Console.WriteLine("\nTryck Enter för att gå till huvudmenyn.");
                Console.ReadKey();
            }
        }


        //initialize userBookLoans array and dates
        static void SetupLibrary()
        {
            string[][] tempUserLoans = new string[books.Length][];
            DateTime[][] tempDates = new DateTime[books.Length][];
            for (int i = 0; i < usernames.Length; i++)
            {
                tempUserLoans[i] = new string[maxLoans];
                tempDates[i] = new DateTime[maxLoans];
            }
            userLoans = tempUserLoans;
            returnDates = tempDates;
        }


        //writes out all the standard menu options and returns the user input
        static int DisplayMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Visa böcker");
            Console.WriteLine("2. Sök efter bok");
            Console.WriteLine("3. Låna bok");
            Console.WriteLine("4. Lämna tillbaka bok");
            Console.WriteLine("5. Mina lån");

            if (admin[userIndex])
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("6. Lägg till bok");
                Console.WriteLine("7. Lägg till användare");
                Console.WriteLine("8. Ta bort användare");
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("9. Logga ut");
            Console.WriteLine("0. Avsluta");
            Console.ForegroundColor = ConsoleColor.White;

            return GetInputInt();
        }


        static void ShowBooks()
        {
            for (int i = 0; i < books.Length; i++)
            {
                int availableBooks = bookAmounts[i] - loanedBooks[i];
                Console.WriteLine($"{i + 1}: {books[i]}, Tillgängliga exemplar: {availableBooks}");
            }
        }


        static void SearchForBook()
        {
            Console.WriteLine("Skriv en boktitel för att söka efter den.");
            string? input = Console.ReadLine();
            int bookIndex = -1;
            //compares every book in the library to the user input to see if it exists
            for (int i = 0; i < books.Length; i++)
            {
                if (books[i] == input)
                {
                    bookIndex = i;
                }
            }

            if (bookIndex >= 0)
            {
                int availableBooks = bookAmounts[bookIndex] - loanedBooks[bookIndex];
                Console.WriteLine($"{bookIndex + 1}: {books[bookIndex]}, Tillgängliga exemplar: {availableBooks}");
            }
            else
            {
                //if bookIndex is still -1 as declared the book was never found
                Console.WriteLine($"Kunde inte hitta någon bok med titeln {input}.");
            }
        }


        static void BorrowBook()
        {
            if (IsUserLoansFull())
            {
                Console.WriteLine("Din lånelista är full.");
                return;
            }

            Console.WriteLine("Vilken bok vill du låna?");
            int availableBooks = 0;
            for (int i = 0; i < books.Length; i++)
            {
                availableBooks = bookAmounts[i] - loanedBooks[i];
                Console.WriteLine($"{i + 1}. {books[i]}, Tillgängliga exemplar: {availableBooks}");
            }

            int input = GetInputInt() - 1;
            availableBooks = bookAmounts[input] - loanedBooks[input];
            if (availableBooks > 0)
            {
                loanedBooks[input]++;
                string returnDate = "";
                for (int i = 0; i < userLoans[userIndex].Length; i++)
                {
                    if (string.IsNullOrEmpty(userLoans[userIndex][i]))
                    {
                        userLoans[userIndex][i] = books[input];
                        returnDates[userIndex][i] = DateTime.Now.AddDays(7);
                        returnDate = returnDates[userIndex][i].ToLongDateString();
                        break;
                    }
                }
                Console.WriteLine($"{usernames[userIndex]} lånar en kopia av {books[input]}, åter {returnDate}");
                SaveData("..\\..\\..\\data.txt");
            }
            else
            {
                Console.WriteLine($"Det finns inga fler exemplar av {books[input]} att låna.");
            }
        }


        static bool IsUserLoansFull()
        {
            foreach (string book in userLoans[userIndex])
            {
                if (string.IsNullOrEmpty(book))
                {
                    return false;
                }
            }
            return true;
        }


        static void ReturnBook()
        {
            if (!HasBorrowedBooks())
            {
                Console.WriteLine("Du har inga lånade böcker.");
                return;
            }

            Console.WriteLine("Vilken bok vill du lämna tillbaka?");
            for (int i = 0; i < userLoans[userIndex].Length; i++)
            {
                if (!string.IsNullOrEmpty(userLoans[userIndex][i]))
                {
                    string returnDate = returnDates[userIndex][i].ToLongDateString();
                    Console.WriteLine($"{i + 1}. {userLoans[userIndex][i]}, åter {returnDate}");
                }
            }

            int input = GetInputInt() - 1;
            if (!string.IsNullOrEmpty(userLoans[userIndex][input]))
            {
                for (int i = 0; i < books.Length; i++)
                {
                    if (books[i] == userLoans[userIndex][input])
                    {
                        loanedBooks[i]--;
                        break;
                    }
                }
                Console.WriteLine($"{usernames[userIndex]} lämnar tillbaka en kopia av {userLoans[userIndex][input]}");
                userLoans[userIndex][input] = "";
                SortUserLoans();
            }
            SaveData("..\\..\\..\\data.txt");
        }


        static void SortUserLoans()
        {
            string[][] tempUserLoans = new string[userLoans.Length][];
            int count = 0;
            for (int i = 0; i < userLoans.Length; i++)
            {
                tempUserLoans[i] = new string[maxLoans];
                foreach (string book in userLoans[i])
                {
                    if (!string.IsNullOrEmpty(book))
                    {
                        tempUserLoans[i][count] = book;
                        count++;
                    }
                }
            }
            userLoans = tempUserLoans;
        }


        static void CheckBorrowedBooks()
        {
            if (!HasBorrowedBooks())
            {
                Console.WriteLine("Du har inga lånade böcker.");
                return;
            }

            Console.WriteLine($"{usernames[userIndex]}s lån:");
            for (int i = 0; i < userLoans[userIndex].Length; i++)
            {
                if (!string.IsNullOrEmpty(userLoans[userIndex][i]))
                {
                    string returnDate = returnDates[userIndex][i].ToLongDateString();
                    Console.WriteLine($"{i + 1}. {userLoans[userIndex][i]}, åter {returnDate}");
                }
            }
        }


        //checks if the current user has any borrowed books
        static bool HasBorrowedBooks()
        {
            for (int i = 0; i < userLoans[userIndex].Length; i++)
            {
                if (!string.IsNullOrEmpty(userLoans[userIndex][i]))
                {
                    return true;
                }
            }

            return false;
        }


        //handles logging in to the library system
        static int LogIn()
        {
            int tries = 0;
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
                        return i;
                    }
                }

                tries++;
            }

            return -1;
        }


        static void AddBook()
        {
            if (!admin[userIndex])
            {
                Console.WriteLine("Du har inte tillgång till den här funktionen");
                return;
            }

            Console.WriteLine("Skriv en boktitel du vill lägga till i biblioteket.");
            string? newBook = Console.ReadLine();
            if (newBook == "" || newBook == null)
            {
                Console.WriteLine("Ogiltig boktitel");
                return;
            }

            Console.WriteLine($"Hur många kopior av {newBook} vill du lägga till?");
            int bookAmount = GetInputInt();

            books = AddToArray(books, newBook);
            bookAmounts = AddToArray(bookAmounts, bookAmount);
            loanedBooks = AddToArray(loanedBooks);

            Console.WriteLine($"{bookAmount} kopior av {newBook} lades till i biblioteket.");
            SaveData("..\\..\\..\\data.txt");
        }


        static void AddUser()
        {
            if (!admin[userIndex])
            {
                Console.WriteLine("Du har inte tillgång till den här funktionen");
                return;
            }

            Console.WriteLine("Skriv ett nytt användarnamn.");
            string? newUser = Console.ReadLine();
            if (newUser == null)
            {
                Console.WriteLine("Ogiltigt användarnamn");
                return;
            }

            Console.WriteLine("Skriv en ny PIN kod");
            int newPin = GetInputInt();

            Console.WriteLine("Ge användaren adminrättigheter? y/n");
            bool isAdmin = IsInputCorrect("y");

            usernames = AddToArray(usernames, newUser);
            pins = AddToArray(pins, newPin);
            admin = AddToArray(admin, isAdmin);
            string[][] tempUserLoans = new string[userLoans.Length + 1][];
            DateTime[][] tempDates = new DateTime[returnDates.Length + 1][];

            for (int i = 0; i < userLoans.Length; i++)
            {
                tempUserLoans[i] = userLoans[i];
                tempDates[i] = returnDates[i];
            }
            tempUserLoans[tempUserLoans.Length - 1] = new string[maxLoans];
            tempDates[tempDates.Length - 1] = new DateTime[maxLoans];

            userLoans = tempUserLoans;
            returnDates = tempDates;

            Console.WriteLine($"Ny användare skapad.");
            SaveData("..\\..\\..\\data.txt");
        }


        static void RemoveUser()
        {
            string previousUser = usernames[userIndex];
            Console.WriteLine("Vilken användare vill du ta bort?");
            for (int i = 0; i < usernames.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {usernames[i]}");
            }
            int input = GetInputInt() - 1;
            Console.WriteLine($"Tar bort {usernames[input]} från användarlistan.");

            int arraySize = usernames.Length - 1;
            string[] tempUsers = new string[arraySize];
            int[] tempPins = new int[arraySize];
            bool[] tempAdmin = new bool[arraySize];
            string[][] tempUserLoans = new string[arraySize][];
            DateTime[][] tempDates = new DateTime[arraySize][];

            int count = 0;
            for (int i = 0; i < usernames.Length; i++)
            {
                if (i != input)
                {
                    tempUsers[count] = usernames[i];
                    tempPins[count] = pins[i];
                    tempAdmin[count] = admin[i];
                    tempUserLoans[count] = userLoans[i];
                    tempDates[count] = returnDates[i];
                    count++;
                }
            }

            usernames = tempUsers;
            pins = tempPins;
            admin = tempAdmin;
            userLoans = tempUserLoans;
            returnDates = tempDates;

            if (input == userIndex)
            {
                userIndex = LogIn();
            }

            for (int i = 0; i < usernames.Length; i++)
            {
                if (usernames[i] == previousUser)
                {
                    userIndex = i;
                }
            }

            SaveData("..\\..\\..\\data.txt");
        }


        //extends the length of array by copying it and transferring its values
        static int[] AddToArray(int[] array, int newValue = 0)
        {
            int[] tempArray = new int[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                tempArray[i] = array[i];
            }
            tempArray[tempArray.Length - 1] = newValue;
            return tempArray;
        }


        //works the same as method above but accepts a string array instead
        static string[] AddToArray(string[] array, string newValue = "")
        {
            string[] tempArray = new string[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                tempArray[i] = array[i];
            }
            tempArray[tempArray.Length - 1] = newValue;
            return tempArray;
        }


        //works the same as methods above but accepts a bool array instead
        static bool[] AddToArray(bool[] array, bool newValue = false)
        {
            bool[] tempArray = new bool[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                tempArray[i] = array[i];
            }
            tempArray[tempArray.Length - 1] = newValue;
            return tempArray;
        }


        //reusable method for input
        static int GetInputInt()
        {
            int input = 0;
            while (!int.TryParse(Console.ReadLine(), out input))
            {
                Console.WriteLine("Felaktig inmatning, försök igen.");
            }
            return input;
        }


        //compares user input to a predetermined answer and returns a bool
        static bool IsInputCorrect(string correctAnswer)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input) || input != correctAnswer)
            {
                return false;
            }
            return true;
        }


        static void SaveData(string savePath)
        {
            if (File.Exists(savePath))
            {
                File.WriteAllLines(savePath, books);
            }
            else
            {
                File.Create(savePath);
            }
        }


        static void LoadData(string loadPath)
        {
            if (File.Exists(loadPath))
            {
                string data = File.ReadAllText(loadPath);
                Console.WriteLine(data);
            }
        }
    }
}
