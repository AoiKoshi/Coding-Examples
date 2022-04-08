#include <iostream>
#include <ctime>

bool CheckGuess(int GuessSum, int GuessProduct, int CodeSum, int CodeProduct)
{
    if(GuessSum == CodeSum && GuessProduct == CodeProduct)
    {
        std::cout << "Correct! You may now proceed to the next security level.\n";
        std::cin.get();
        return true;
    }
    else{
        std::cout << "Error! Wrong codes entered. Shutting down...\n";
        std::cin.get();
        return false;
    }
}

bool PlayGameAtDifficulty(int difficulty)
{
    std::cout << "Security level: " << difficulty << std::endl;

    int CodeA = rand()%(10*difficulty) + 1;
    int CodeB = rand()%(10*difficulty) + 1;
    int CodeC = rand()%(10*difficulty) + 1;
    int CodeSum = CodeA + CodeB + CodeC;
    int CodeProduct = CodeA * CodeB * CodeC;

    //Uncomment for testing/cheating purposes
    //std::cout << CodeA << CodeB << CodeC << std::endl;
    std::cout << "There are 3 numbers in the code.\n";
    std::cout << "The codes add up to: " << CodeSum << std::endl;
    std::cout << "The codes multiply to: " << CodeProduct << std::endl;

    int GuessA, GuessB, GuessC;
    std::cin >> GuessA >> GuessB >> GuessC;
    int GuessSum = GuessA + GuessB + GuessC;
    int GuessProduct = GuessA * GuessB * GuessC;

    return CheckGuess(GuessSum, GuessProduct, CodeSum, CodeProduct);
}

void IntroSequence()
{
    std::cout << "Hey there super hacker! You are *the* SilverFox, right?\n";
    std::cout << "Well, we're in dire need of your help.\n";
    std::cout << "You'll need to enter the correct codes to continue...\n";
}

void EndSequence(bool ShutDown)
{
    if(ShutDown)
    {
       std::cout << "Oh dear, it seems you've failed. They've been alerted, get out of there!\n";
       std::cin.get();
    }
    else{
       std::cout << "Wowza, mission accomplished!\n";
       std::cout << "Now retrieve the data and get out of there!\n";
       std::cin.get();
    }
}

int main()
{
    srand(time(NULL));
    IntroSequence();

    int Difficulty = 1;
    const int MaxDifficulty = 3;
    bool ShutDown = false;
    
    while(Difficulty <= MaxDifficulty)
    {
        if(PlayGameAtDifficulty(Difficulty))
        {
            ++Difficulty;
        }
        else{
            ShutDown = true;
            break;
        }
        std::cin.clear(); //Clears the failbit
        std::cin.ignore(); //Discards the buffer
    }

    EndSequence(ShutDown);
    return 0;
}