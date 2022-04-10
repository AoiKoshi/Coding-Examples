#include <iostream>
#include <fstream>
#include <string>
#include <list>
#include <vector>
using namespace std;

//Declarations
ifstream readTextFile();
void createMatrix(ifstream& mazeFile);
void displayMatrix();
int BFS();
string recordDirection(int direction);

//Define types
typedef pair<int,int> xy;

//N > W > E > S
int directionY[] = {-1, 0, 0, 1};
int directionX[] = {0, -1, 1, 0};

//Keys
const char cStart = 'A';
const char cEnd = 'B';
const char cWall = 'x';
const char cSpace = '.';

//Structs
struct mazeStruct {
    int rows, columns;
    xy posStart, posEnd;
    vector<vector<int>> matrix;
    string pathway;
} maze;

struct pointStruct {
    xy pos;
};

struct queueNodeStruct {
    pointStruct point;
    int distance; //distance from the source
    string pathway; //route from start to this point so far
};


//Check cell is in bounds
bool isInBounds(int row, int column)
{
    return (row >= 0) && (row < maze.rows) && (column >= 0) && (column < maze.columns);
}

//Check cell is a space that can be moved into
bool isValidSpace(int row, int column)
{
    return maze.matrix.at(row).at(column);
}

int main()
{
    ifstream mazeFile = readTextFile();

    createMatrix(mazeFile);
    displayMatrix();

    int distance = BFS();
    if (distance != -1)
    {
        cout << "Shortest path is: " << maze.pathway << endl;
        cout << "The distance is " << distance << " moves." << endl;
    }
    else{
        cout << "There is no path." << endl;
    }

    cin.get();

    mazeFile.close();
    maze.pathway = "";

    return 0;
}

ifstream readTextFile()
{
    string nameOfMazeFile;
    cout << "Enter the name of the maze file you wish to open:" << endl;
    cin >> nameOfMazeFile;

    //Reads a text file
    ifstream mazeFile("Mazes/" + nameOfMazeFile + ".txt");
    return mazeFile;
}

//Produces a MxN matrix from the text file
void createMatrix(ifstream& mazeFile)
{
    int rows = 0;
    int columns = 0;

    string str;
    while (getline(mazeFile, str))
    {
        //Maze will expand to fit widest line of characters
        columns = str.length();

        //Processing each character type to make a MxN matrix while displaying the original
        vector<int> tempRow;
        for (int i = 0; i < str.length(); i++)
        {
            switch (str[i])
            {
                case cWall:
                    tempRow.push_back(0);
                    cout << "x";
                    break;
                case cSpace:
                    tempRow.push_back(1);
                    cout << ".";
                    break;
                case cStart:
                    maze.posStart = xy(i, rows);
                    tempRow.push_back(1);
                    cout << "A";
                    break;
                case cEnd:
                    maze.posEnd = xy(i, rows);
                    tempRow.push_back(1);
                    cout << "B";
                    break;
                default:
                    tempRow.push_back(0);
                    cout << "x";
                    break;
            }
        }

        rows++; //Add a row for each line of text

        cout << endl;
        
        maze.matrix.push_back(tempRow);
    }

    maze.rows = rows;
    maze.columns = columns;
}

//Output the MxN matrix to the terminal for visual representation
void displayMatrix()
{
    cout << "\nConverted into a MxN matrix:\n" << endl;
    cin.get();

    for (int i = 0; i < maze.rows; i++)
    {
        string line;
        for (int j = 0; j < maze.columns; j++)
        {
            line += to_string(maze.matrix.at(i).at(j));
        }
        cout << line << endl;
    }
}

//Breadth-First-Search algorithm > Expected time complexity is O(MN)
int BFS()
{
    vector<vector<bool>> visited;
    visited.resize(maze.rows);
    for(int i=0; i < maze.rows; i++)
    {
        visited[i].resize(maze.columns);
    }

    //Set all spaces to unvisited
    for (int i = 0; i < visited.capacity(); i++)
    {
        for (int j = 0; j < visited.capacity(); j++)
        {
            visited.at(i).at(j) = false;
        }
    }

    //Marking start pos as visited
    visited[maze.posStart.second][maze.posStart.first] = true;

    list<queueNodeStruct> q;
    queueNodeStruct s = {
        maze.posStart, 
        0, 
        ""
    };
    q.push_back(s);

    string directions;

    while (!q.empty())
    {
        queueNodeStruct current = q.front();
        pointStruct point = current.point;

        if (point.pos == maze.posEnd)
        {
            string pathway = current.pathway;
            pathway.erase(pathway.begin() + current.distance, pathway.end());
            maze.pathway = pathway;
            return current.distance;
        }

        q.pop_front();

        for (int i = 0; i < 4; i++)
        {
            int row = point.pos.second + directionY[i];
            int col = point.pos.first + directionX[i];

            if (isInBounds(row, col) && isValidSpace(row, col))
            {
                if (!visited.at(row).at(col))
                {
                    visited[row][col] = true;
                    queueNodeStruct adjacentCell = {
                        xy(col, row),
                        current.distance + 1,
                        //Store current route
                        current.pathway + recordDirection(i)
                    };
                    q.push_back(adjacentCell);
                }
            }
        }
    }

    //If destination cannot be reached, return -1
    return -1;
}

string recordDirection(int direction)
{
    switch (direction)
    {
        case 0:
            return "N";
            break;
        case 1:
            return "W";
            break;
        case 2:
            return "E";
            break;
        case 3:
            return "S";
            break;
    }

    return "";
}