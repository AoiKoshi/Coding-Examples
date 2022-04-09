#include <iostream>
#include <string>
using namespace std;

string str;
bool shouldContinue = true;

//Declaring functions
void askEnd();
void directOperation(const int operation);
void reverse(const string& str);
void checkCharFrequency(const string& str);
void removeCharacter(string str);

int main()
{
    while (shouldContinue)
    {
        cout << "Enter a string: " << endl;
        getline(cin, str);

        int operation;
        const char *options =   "String operations\n"
                                "1 = Reversal\n"
                                "2 = Check frequency of a character\n"
                                "3 = Remove character\n";
        cout << options << endl;
        cout << "Select an operation by its number: " << endl;
        cin >> operation;
            
        directOperation(operation);

        askEnd();
    }

    return 0;
}

//Definitions
void askEnd()
{
    char confirmation;
    cout << "Would you like to continue using the string manipulator? y/n" << endl;
    cin >> confirmation;
    switch (confirmation)
    {
        case 'y':
            cout << "Confirmed. The manipulator will reset." << endl;
            break;
        case 'n':
            cout << "Thank you for using this string manipulator application." << endl;
            cin.get();
            shouldContinue = false;
            break;
        default:
            cout << "Your input was not recognised. Will restart manipulator." << endl;
            break;
    }
}

void directOperation(const int operation)
{
    switch (operation)
    {
        case 1:
            reverse(str);
            cin.get();
            break;
        case 2:
            checkCharFrequency(str);
            cin.get();
            break;
        case 3:
            removeCharacter(str);
            cin.get();
            break;
    }
}

void reverse(const string& str)
{
    // store the size of the string
    size_t numOfChars = str.size();

    //Output single character, no need for reversal
    if(numOfChars == 1) {
        cout << str << endl;
    }
    else {
        cout << str[numOfChars - 1];
        reverse(str.substr(0, numOfChars - 1)); //Recursion!
    }
}

void checkCharFrequency(const string& str)
{
    char checkCharacter;
    cout << "Enter character: \n";
    cin >> checkCharacter;

    //Count every time there's a character match
    int count = 0;
    for (int i = 0; i < str.size(); i++)
    {
        if (str[i] == checkCharacter)
        {
            ++count;
        }
    }

    cout << "Number of " << checkCharacter << " = " << count << endl;
}

void removeCharacter(string str)
{
    char checkCharacter;
    cout << "Enter character to remove: \n";
    cin >> checkCharacter;

    string temp;

    for (int i = 0; i < str.length(); ++i)
    {
        //Add all but the blacklisted character to temp
        if (str[i] != checkCharacter)
        {
            temp += (str[i]);
        }
    }
    
    cout << temp << endl;
}