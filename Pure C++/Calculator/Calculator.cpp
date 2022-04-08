#include <iostream>
using namespace std;

char operation;
float num1, num2;
bool shouldContinue = true;

//Declaring functions
void performOperation();
void askEnd();

int main() {
    while (shouldContinue)
    {
        cout << "Enter first number: ";
        cin >> num1;

        cout << "Enter operator: +, -, *, /: ";
        cin >> operation;

        cout << "Enter second numbers: ";
        cin >> num2;

        performOperation();

        askEnd();
    }

    return 0;
}

void askEnd()
{
    char confirmation;
    cout << "Would you like to continue using the calculator? y/n" << endl;
    cin >> confirmation;
    switch (confirmation)
    {
        case 'y':
            cout << "Confirmed. The calculator will reset." << endl;
            break;
        case 'n':
            cout << "Thank you for using this simple calculator application." << endl;
            cin.get();
            shouldContinue = false;
            break;
        default:
            cout << "Your input was not recognised. Will restart operations." << endl;
            break;
    }
}

//Defining functions
void performOperation()
{
    switch(operation) {

        case '+':
        cout << num1 << " + " << num2 << " = " << num1 + num2 << endl;
        break;

        case '-':
        cout << num1 << " - " << num2 << " = " << num1 - num2 << endl;
        break;

        case '*':
        cout << num1 << " * " << num2 << " = " << num1 * num2 << endl;
        break;

        case '/':
        cout << num1 << " / " << num2 << " = " << num1 / num2 << endl;
        break;

        default:
        // Incorrect or unsupported operators show error
        cout << "Error! operator is not correct\n";
        break;
    }
}