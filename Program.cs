namespace LibraryProgram
{
    internal class Program
    {
        public static string[] books = ["Pride and Prejudice", "The Great Gatsby", "Moby-Dick", "Les Miserables", "Frankenstein"];
        public static int[] bookAmounts = [6, 8, 5, 10, 13]; //keeps track of how many copies of each book there are in the library
        public static int[] loanedBooks = new int[books.Length]; //keeps track of all the books the library has loaned out

        public static string[] usernames = ["Anna", "Bob", "Cecilia", "David", "Eva"];
        public static int[] pins = [1234, 2345, 3456, 4567, 5678]; //login pins for each user
        public static bool[] admin = [true, false, true, false, false]; //keeps track of whether the user is an admin or not
        public static string[][] userLoans = new string[usernames.Length][]; //keeps track of the books that users have loaned
        public static DateTime[][] returnDates = new DateTime[usernames.Length][]; //keeps track of when every book needs to be returned
        public static int maxLoans = 10; //number used to set the size of every user's loan list
        public static int userIndex = -1; //keeps track of which user is currently logged in


        static void Main(string[] args)
        {
            Console.Title = "LibraryProgram";
            LoadLibraryData();
            LoadUserData();
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

                SaveLibraryData();
                SaveUserData();
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

            //if the current user is an admin they get special options
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


        //writes out every book and how many are available for rent in the console
        static void ShowBooks()
        {
            for (int i = 0; i < books.Length; i++)
            {
                int availableBooks = bookAmounts[i] - loanedBooks[i];
                Console.WriteLine($"{i + 1}: {books[i]}, Tillgängliga exemplar: {availableBooks}");
            }
        }


        //searches the library for a specific book and writes its name in the console
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


        //adds selected book into the user's loans list
        static void BorrowBook()
        {
            //if loan list is full the user cannot borrow anything
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
                        //creates return date for the book seven days in the future
                        userLoans[userIndex][i] = books[input];
                        returnDates[userIndex][i] = DateTime.Now.AddDays(7);
                        returnDate = returnDates[userIndex][i].ToLongDateString();
                        break;
                    }
                }
                Console.WriteLine($"{usernames[userIndex]} lånar en kopia av {books[input]}, åter {returnDate}");
            }
            else
            {
                Console.WriteLine($"Det finns inga fler exemplar av {books[input]} att låna.");
            }
        }


        //checks if the current user's loan list is full and returns a bool based on the result
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


        //removes selected book from the user loans so it can be borrowed again
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
        }


        //sorts the userLoans array so that the indexes are presented correctly
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


        //checks if the user has any borrowed books and writes them out in the console
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
            //the user gets three tries to log in before the program exits
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


        //adds a new book into the books array based on user input
        static void AddBook()
        {
            //first checks if the current user can access the AddBook method
            if (!admin[userIndex])
            {
                Console.WriteLine("Du har inte tillgång till den här funktionen");
                return;
            }

            //lets the user choose a title for the new book
            Console.WriteLine("Skriv en boktitel du vill lägga till i biblioteket.");
            string? newBook = Console.ReadLine();
            if (string.IsNullOrEmpty(newBook))
            {
                Console.WriteLine("Ogiltig boktitel");
                return;
            }

            //lets the user choose how many copies of the new book should be added into the library
            Console.WriteLine($"Hur många kopior av {newBook} vill du lägga till?");
            int bookAmount = GetInputInt();

            //update the relevant arrays
            books = AddToArray(books, newBook);
            bookAmounts = AddToArray(bookAmounts, bookAmount);
            loanedBooks = AddToArray(loanedBooks);

            Console.WriteLine($"{bookAmount} kopior av {newBook} lades till i biblioteket.");
        }


        static void AddUser()
        {
            //first checks if the current user can access the AddUser method
            if (!admin[userIndex])
            {
                Console.WriteLine("Du har inte tillgång till den här funktionen");
                return;
            }

            /*lets the user choose a username and pin
            and if the new user is an admin or not*/
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

            //adds a space to all the relevant arrays
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
        }


        //removes a user from and clears all the relevant arrays related to that user
        static void RemoveUser()
        {
            string previousUser = usernames[userIndex];
            //first the user gets to select a user to remove
            Console.WriteLine("Vilken användare vill du ta bort?");
            for (int i = 0; i < usernames.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {usernames[i]}");
            }
            int input = GetInputInt() - 1;
            Console.WriteLine($"Tar bort {usernames[input]} från användarlistan.");

            //then creates temporary arrays with one less space
            int arraySize = usernames.Length - 1;
            string[] tempUsers = new string[arraySize];
            int[] tempPins = new int[arraySize];
            bool[] tempAdmin = new bool[arraySize];
            string[][] tempUserLoans = new string[arraySize][];
            DateTime[][] tempDates = new DateTime[arraySize][];

            //saves every user except for the one chosen for removal
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

            //lastly save all the temp arrays into the ones the program uses
            usernames = tempUsers;
            pins = tempPins;
            admin = tempAdmin;
            userLoans = tempUserLoans;
            returnDates = tempDates;

            //if the current user is the one being removed, go back to the login screen
            if (input == userIndex)
            {
                userIndex = LogIn();
            }

            //ensures that the same user is logged in after the indexes change
            for (int i = 0; i < usernames.Length; i++)
            {
                if (usernames[i] == previousUser)
                {
                    userIndex = i;
                }
            }
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


        //reusable method for getting an int from user input
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


        //saves all book specific data to a text file
        static void SaveLibraryData()
        {
            string savePath = "..\\..\\..\\LibraryData.txt";
            string libraryData = "";
            for (int i = 0; i < books.Length; i++)
            {
                /*continuously copies data from arrays to libraryData string
                and goes to the next line after saving the current user's data*/
                libraryData += $"{books[i]}, {bookAmounts[i]}, {loanedBooks[i]}\n";
            }
            File.WriteAllText(savePath, libraryData);
        }


        //saves all user specific data to a text file
        static void SaveUserData()
        {
            string savePath = "..\\..\\..\\UserData.txt";
            string userData = "";
            for (int i = 0; i < usernames.Length; i++)
            {
                /*continuously copies data from arrays to userData string
                and goes to the next line after saving the current user's data*/
                userData += $"{usernames[i]},{pins[i]},{admin[i]},";
                for (int j = 0; j < maxLoans; j++)
                {
                    userData += $"{userLoans[i][j]}_";
                }
                userData += ",";
                for (int k = 0; k < maxLoans; k++)
                {
                    userData += $"{returnDates[i][k]}_";
                }
                userData += "\n";
            }
            File.WriteAllText(savePath, userData);
        }


        //loads books from save file and assigns them to premade arrays
        static void LoadLibraryData()
        {
            string loadPath = "..\\..\\..\\LibraryData.txt";
            if (File.Exists(loadPath))
            {
                //reads all lines in the save file and saves them to a string
                string[] libraryData = File.ReadAllLines(loadPath);
                if (libraryData.Length == 0)
                {
                    return;
                }

                //sets new lengths of relevant arrays based on data
                books = new string[libraryData.Length];
                bookAmounts = new int[libraryData.Length];
                loanedBooks = new int[libraryData.Length];

                for (int i = 0; i < libraryData.Length; i++)
                {
                    /*splits up each line in libraryData and
                    assigns the data to different arrays*/
                    string[] splitData = libraryData[i].Split(',');
                    books[i] = splitData[0];
                    bookAmounts[i] = int.Parse(splitData[1]);
                    loanedBooks[i] = int.Parse(splitData[2]);
                }
            }
        }


        //loads user data from save file and assigns it to premade arrays
        static void LoadUserData()
        {
            string loadPath = "..\\..\\..\\UserData.txt";
            if (File.Exists(loadPath))
            {
                //reads all lines in the save file and saves them to a string
                string[] userData = File.ReadAllLines(loadPath);
                if (userData.Length == 0)
                {
                    return;
                }

                //sets new lengths of relevant arrays based on data
                usernames = new string[userData.Length];
                pins = new int[userData.Length];
                admin = new bool[userData.Length];
                userLoans = new string[userData.Length][];
                returnDates = new DateTime[userData.Length][];

                for (int i = 0; i < userData.Length; i++)
                {
                    /*splits up each line in userData and
                    assigns the data to different arrays*/
                    string[] splitData = userData[i].Split(',');
                    usernames[i] = splitData[0];
                    pins[i] = int.Parse(splitData[1]);
                    admin[i] = bool.Parse(splitData[2]);

                    string[] splitLoans = splitData[3].Split('_');
                    userLoans[i] = new string[splitLoans.Length];
                    userLoans[i] = splitLoans;

                    string[] splitDates = splitData[4].Split('_');
                    returnDates[i] = new DateTime[splitDates.Length];
                    for (int j = 0; j < splitDates.Length; j++)
                    {
                        if (!string.IsNullOrEmpty(splitDates[j]))
                        {
                            returnDates[i][j] = DateTime.Parse(splitDates[j]);
                        }
                    }
                }
            }
            else
            {
                string[][] tempUserLoans = new string[usernames.Length][];
                DateTime[][] tempDates = new DateTime[usernames.Length][];
                for (int i = 0; i < usernames.Length; i++)
                {
                    tempUserLoans[i] = new string[maxLoans];
                    tempDates[i] = new DateTime[maxLoans];
                }
                userLoans = tempUserLoans;
                returnDates = tempDates;
            }
        }
    }
}
