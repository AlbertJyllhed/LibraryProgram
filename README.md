# LibraryProgram
This project was developed for a school assignment and the goal was to simulate a library where users can log in and loan books.
Admins can add or remove users and add new books into the library.

# The user is given a menu of options that consist of:
1. ShowBooks (display a list of all books in the library and how many are available)
2. SearchForBook (displays a specific book in the library based on user input)
3. BorrowBook (adds selected book from the library into the logged in user's private loan list)
4. ReturnBook (removes selected book from the logged in user's private loan list and makes it available to borrow again)
5. CheckBorrowedBooks (displays a list of all books in the logged in user's private loan list)

# Admin options:
6. AddBook (adds a new book into the library based on user input)
7. AddUser (adds a new user into the system)
8. RemoveUser (removes selected user from the system)

# Exit options:
10. LogIn (used to log in if user inputs the correct username and pin)
0. Exit (exits the program)

# Post mortem
I was quite limited in my options when developing this project which is why some implementations are not ideal. I could not create custom classes or use methods we hadn't gone through in class yet so I did the best I could with what I was allowed to work with.

This program heavily utilizes arrays which makes the code kind of tricky to handle. It does make it easy to group together data as it can just be accessed with an index, which is one benefit of this approach.
If I were to make a program like this again in the future I would make use of custom classes to create the same functionality without so many lines of code.

The program makes use of txt files to save/load data. If the file is modified outside of the program it will crash on load because the program can only parse very specific strings of data.

A better solution than txt files would have been Json files which can more easily be converted into data.
