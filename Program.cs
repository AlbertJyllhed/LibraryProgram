using System;

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
        public static int[][] userBookLoans = new int[usernames.Length][];
        //used to keep track of which user is currently logged in
        public static int currentUserIndex = -1;


        static void Main(string[] args)
        {
            //initialize userBookLoans array
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
                    case 7:
                        AddBook();
                        break;
                    case 8:
                        AddUser();
                        break;
                    case 9:
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
            Console.WriteLine("2. Sök efter bok");
            Console.WriteLine("3. Låna bok");
            Console.WriteLine("4. Lämna tillbaka bok");
            Console.WriteLine("5. Mina lån");

            if (admin[currentUserIndex])
            {
                Console.WriteLine("7. Lägg till bok");
                Console.WriteLine("8. Lägg till användare");
            }

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("9. Logga ut");
            Console.WriteLine("10. Avsluta");

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
            if (!HasBorrowedBooks())
            {
                Console.WriteLine("Du har inga lånade böcker.");
                return;
            }

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
                loanedBooks[input]--;
                userBookLoans[currentUserIndex][input]--;
                Console.WriteLine($"{usernames[currentUserIndex]} lämnar tillbaka en kopia av {books[input]}");
            }
        }


        static void CheckBorrowedBooks()
        {
            if (!HasBorrowedBooks())
            {
                Console.WriteLine("Du har inga lånade böcker.");
                return;
            }

            Console.WriteLine($"{usernames[currentUserIndex]}s lån:");
            for (int i = 0; i < userBookLoans[currentUserIndex].Length; i++)
            {
                if (userBookLoans[currentUserIndex][i] > 0)
                {
                    Console.WriteLine($"{books[i]}: {userBookLoans[currentUserIndex][i]} lånade");
                }
            }
        }


        //checks if the current user has any borrowed books
        static bool HasBorrowedBooks()
        {
            for (int i = 0; i < userBookLoans[currentUserIndex].Length; i++)
            {
                if (userBookLoans[currentUserIndex][i] > 0)
                {
                    return true;
                }
            }

            return false;
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


        static void AddBook()
        {
            if (!IsAdmin())
            {
                return;
            }

            Console.WriteLine("Skriv en boktitel du vill lägga till i biblioteket.");
            string? newBook = Console.ReadLine();
            if (newBook == null)
            {
                Console.WriteLine("Ogiltig boktitel");
                return;
            }

            books = AddToStringArray(books, newBook);
            int randomBookAmount = new Random().Next(5, 20);
            bookAmounts = AddToIntArray(bookAmounts, randomBookAmount);
            loanedBooks = AddToIntArray(loanedBooks);
            for (int i = 0; i < userBookLoans.Length; i++)
            {
                userBookLoans[i] = AddToIntArray(userBookLoans[i]);
            }
        }


        static void AddUser()
        {
            if (!IsAdmin())
            {
                return;
            }
        }


        //extends the length of array by copying it and transferring its values
        static int[] AddToIntArray(int[] array, int newValue = 0)
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
        static string[] AddToStringArray(string[] array, string newValue = "")
        {
            string[] tempArray = new string[array.Length + 1];
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
                Console.WriteLine($"Felaktig inmatning, försök igen.");
            }
            return input;
        }


        //checks if the user is an admin and writes a message if they are not
        static bool IsAdmin()
        {
            if (admin[currentUserIndex])
            {
                Console.WriteLine("Du har inte tillgång till den här funktionen");
                return true;
            }

            return false;
        }
    }
}
