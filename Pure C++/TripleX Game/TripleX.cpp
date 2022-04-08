#include <iostream>
#include <ctime>

using namespace std;

bool checkGuess(int GuessSum, int GuessProduct, int CodeSum, int CodeProduct)
{
    if(GuessSum == CodeSum && GuessProduct == CodeProduct)
    {
        cout << "Correct! You may now proceed to the next security level.\n";
        cin.get();
        return true;
    }
    else{
        cout << "Error! Wrong codes entered. Shutting down...\n";
        cin.get();
        return false;
    }
}

bool playGameAtDifficulty(int difficulty)
{
    cout << "Security level: " << difficulty << endl;

    int CodeA = rand()%(10*difficulty) + 1;
    int CodeB = rand()%(10*difficulty) + 1;
    int CodeC = rand()%(10*difficulty) + 1;
    int CodeSum = CodeA + CodeB + CodeC;
    int CodeProduct = CodeA * CodeB * CodeC;

    //Uncomment for testing/cheating purposes
    //cout << CodeA << CodeB << CodeC << endl;
    cout << "There are 3 numbers in the code.\n";
    cout << "The codes add up to: " << CodeSum << endl;
    cout << "The codes multiply to: " << CodeProduct << endl;

    int GuessA, GuessB, GuessC;
    cin >> GuessA >> GuessB >> GuessC;

    int GuessSum = GuessA + GuessB + GuessC;
    int GuessProduct = GuessA * GuessB * GuessC;

    return checkGuess(GuessSum, GuessProduct, CodeSum, CodeProduct);
}

void introSequence()
{
    cout << "Hey there super hacker! You are *the* SilverFox, right?\n";
    cout << "Well, we're in dire need of your help.\n";
    cout << "You'll need to enter the correct codes to continue...\n";
}

void endSequence(bool ShutDown)
{
    if(ShutDown)
    {
       cout << "Oh dear, it seems you've failed. They've been alerted, get out of there!\n";
       cin.get();
    }
    else{
       cout << "Wowza, mission accomplished!\n";
       cout << "Now retrieve the data and get out of there!\n";
       cin.get();
    }
}

int main()
{
    srand(time(NULL));
    introSequence();

    //Setting the number of levels that can be played
    int Difficulty = 1;
    const int MaxDifficulty = 3;
    bool ShutDown = false;
    
    while(Difficulty <= MaxDifficulty)
    {
        if(playGameAtDifficulty(Difficulty))
        {
            ++Difficulty;
        }
        else{
            ShutDown = true;
            break;
        }
        cin.clear(); //Clears the failbit
        cin.ignore(); //Discards the buffer
    }

    endSequence(ShutDown);
    return 0;
}