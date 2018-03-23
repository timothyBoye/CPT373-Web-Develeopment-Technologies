//using Newtonsoft.Json;
using GoLA2.Models.Database;
using System;
using System.Text;

namespace GoLA2.Models.Logic
{
    /// <summary>
    /// The model for a game of life.
    /// </summary>
    public class GameOfLife : IGameOfLife
    {
        public string Name { get; set; }
        /// <summary> The height of the game board. </summary>
        public int Height { get; set; }
        /// <summary> The width of the game board. </summary>
        public int Width { get; set; }
        /// <summary> The grid of Cells that are all either Alive or Dead. </summary>
        public Cell[][] Cells { get; set; }
        /// <summary>EF UserGameID for connecting this game to the EF class UserGame</summary>
        public int UserGameID { get; set; }
        /// <summary>EF UserID for connecting the owner of this game to the EF class UserGame</summary>
        public int UserID { get; set; }
        /// <summary>EF User for connecting the owner of this game to the EF class UserGame</summary>
        public User User { get; set; }
        /// <summary> The name of the template that was used to start the game. </summary>
        public string TemplateName { get; private set; }
        /// <summary> The character used to print an alive cell. </summary>
        private const char Alive = '█';
        /// <summary> The character used to print a dead cell. </summary>
        private const char Dead = ' ';

        // character references for importing/exporting cells
        private static char AliveString = 'O';
        private static char DeadString = 'X';
        private static char aliveString = 'o';
        private static char deadString = 'x';


        /// <summary>
        /// Takes and width and height and creates the grid based on
        /// those. The width and height both must be more than zero
        /// obviously.
        /// </summary>
        /// <param name="height">The height of the game board.</param>
        /// <param name="width">The width of the game board.</param>
        /// <exception cref="FormatException">
        /// Throws FormatException if the size of the game board is < one by one.
        /// </exception>
        public GameOfLife(int height, int width, string name)
        {
            this.Name = name;
            // CHECK if the grid is less than 1x1 and throw if it is
            if (width <= 0 || height <= 0)
            {
                throw new FormatException("The board needs to be at least 1x1");
            }
            // CREATE the grid
            else
            {
                this.Height = height;
                this.Width = width;
                Cells = new Cell[Height][];
                for (int i = 0; i < Cells.Length; i++)
                {
                    Cells[i] = new Cell[Width];
                    for (int j = 0; j < Cells[i].Length; j++)
                    {
                        Cells[i][j] = Cell.Dead;
                    }
                }
            }
        }

        /// <summary>
        /// Creates a game based on the EF class UserGame although
        /// we don't assume that the class is a valid game and we 
        /// still check it.
        /// </summary>
        /// <param name="game">EF UserGame object to be made into a game</param>
        /// <param name="ID">The id of the game</param>
        /// <param name="userID">the id of the user</param>
        /// <param name="user">the user themselves</param>
        public GameOfLife(UserGame game, int ID, int userID, User user)
        {
            // Set the base params assume valid
            this.Name = game.Name;
            this.Height = game.Height;
            this.Width = game.Width;
            this.UserGameID = ID;
            this.UserID = userID;
            this.User = user;

            // the grid needs to be at least 1x1)
            if (Height > 0 && Width > 0)
            {
                // split the UserGame cell string to get the rows
                String[] file = game.Cells.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
                // Create the cell array array
                Cells = new Cell[Height][];
                // Loop through the rows and enter the cells
                for (int i = 0; i < Cells.Length; i++)
                {
                    // create this rows array
                    Cells[i] = new Cell[Width];
                    // check the UserGame's row is a valid length
                    if (file[i].Length == Width)
                    {
                        // loop through the cells, check valid and enter into array
                        for (int j = 0; j < file[i].Length; j++)
                        {
                            // dead cell is dead
                            if (isDeadCell(file[i][j]))
                            {
                                Cells[i][j] = Cell.Dead;
                            }
                            // alive cell is alive
                            else if (isAliveCell(file[i][j]))
                            {
                                Cells[i][j] = Cell.Alive;
                            }
                            // Cell isn't alive or dead? Schrodinger can have an error
                            else
                            {
                                throw new FormatException("The file is corrupt");
                            }

                        }
                    }
                    // invalid row length
                    else
                    {
                        throw new FormatException("The file is corrupt");
                    }
                }
            }
            // invalid height
            else
            {
                throw new FormatException("The file is corrupt");
            }
        }

        /// <summary>
        /// Check if a cell is alive
        /// </summary>
        /// <param name="cell">The char of the cell we're looking at</param>
        /// <returns> true if alive</returns>
        private static bool isAliveCell(char cell)
        {
            if (cell == AliveString || cell == aliveString)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Check if a cell is dead
        /// </summary>
        /// <param name="cell">The char of the cell we're looking at</param>
        /// <returns> true if dead</returns>
        private static bool isDeadCell(char cell)
        {
            if (cell == DeadString || cell == deadString)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Takes a template and a grid position and places the template
        /// at the grid position provided if it fits within the size of the
        /// game.
        /// </summary>
        /// <param name="template">The template object to import into the game.</param>
        /// <param name="templateX">The x coordinate to place the template at.</param>
        /// <param name="templateY">The u coordinate to place the template at.</param>
        /// <exception cref="FormatException">
        /// Throws FormatException if the template won't fit on the game
        /// board at the location specified.
        /// </exception>
        public void InsertTemplate(ITemplate template, int templateX, int templateY)
        {
            // THROW an exception if the template wont fit where it's meant to
            if (template.Height > this.Height - templateY
                || template.Width > this.Width - templateX)
            {
                throw new FormatException("The template doesn't fit");
            }
            // TEMPLATE FITS so insert it ito the game grid
            else
            {
                TemplateName = template.Name;
                for (int i = 0; i < template.Cells.Length; i++)
                {
                    for (int j = 0; j < template.Cells[i].Length; j++)
                    {
                        Cells[templateY + i][templateX + j] =
                            template.Cells[i][j];
                    }
                }
            }
        }


        /// <summary>
        /// Advances the game state one step.
        /// </summary>
        public void TakeTurn()
        {
            // MAKE temporary grid
            Cell[][] newCells = CloneCells();
            // TRANSITION all cells
            for (int i = 0; i < Cells.Length; i++)
            {
                for (int j = 0; j < Cells[i].Length; j++)
                {
                    newCells[i][j] = transition(i,j);
                }
            }
            // SAVE new state
            Cells = newCells;
        }

        /// <summary>
        /// Determines the new state of a cell from the Cell array indicated by the input coordinates
        /// and returns the cell's new state. DOES NOT alter the Cell array, this must be done by the 
        /// calling function.
        /// </summary>
        /// <param name="x">X Coordinate of cell</param>
        /// <param name="y">Y Coordinate of cell</param>
        /// <returns>New Cell state based on game rules</returns>
        private Cell transition(int x, int y)
        {
            // COUNTER set at 0
            int liveNeighbours = 0;
            // CHECK IF EDGE CASE and assign start and end of grid to check
            int xStart = x - 1 >= 0 ? x - 1 : x;
            int yStart = y - 1 >= 0 ? y - 1 : y;
            int xEnd = x + 1 < Height ? x + 1 : x;
            int yEnd = y + 1 < Width ? y + 1 : y;

            // CYCLE through the grid around the coordinate provided
            for (int i = xStart; i <= xEnd; i++)
            {
                for (int j = yStart; j <= yEnd; j++)
                {
                    // ADD 1 to count if cell alive and ISN'T the coord provided
                    if( !((i == x) && (j == y)) 
                        && (Cells[i][j] == Cell.Alive))
                    {
                        liveNeighbours++;
                    }
                }
            }
            // CHECK Cell is alive or dead then use the game rules to alter its state based on the count
            // WAS Alive
            if (Cells[x][y] == Cell.Alive)
            {
                // OVER and UNDER population kills
                if (liveNeighbours < 2 || liveNeighbours > 3)
                {
                    return Cell.Dead;
                }
                // No change
                else
                {
                    return Cell.Alive;
                }
            }
            // WAS dead
            else
            {
                // POPULATION Growth
                if (liveNeighbours == 3)
                {
                    return Cell.Alive;
                }
                // No change
                else
                {
                    return Cell.Dead;
                }
            }
        }

        /// <summary>
        /// Creates a formatted string primarily for
        /// outputting the game grid to the console.
        /// </summary>
        /// <param name="alivechar">the char to be used to represent alive cells</param>
        /// <param name="deadchar">the char to be used to represent dead cells</param>
        /// <returns>A formatted string using deadchar for dead cells and alivechar for
        /// Alive cells.</returns>
        public string ToString(char alivechar, char deadchar)
        {
            // CREATE a string builder to create the output string
            StringBuilder s = new StringBuilder();
            // LOOP through all the cells and print them to the stringbuilder
            foreach (Cell[] carray in Cells)
            {
                foreach (Cell c in carray)
                {
                    if (c == Cell.Alive)
                        s.Append(alivechar);
                    else
                        s.Append(deadchar);
                }
                s.AppendLine();
            }

            // RETURN the grid string
            return s.ToString();
        }

        /// <summary>
        /// Clones the Cells arrays size NOT the cells!
        /// </summary>
        /// <returns> Identical in size copy of the Cells array</returns>
        private Cell[][] CloneCells()
        {
            Cell[][] newCells = new Cell[Height][];
            for (int i = 0; i < Cells.Length; i++)
            {
                newCells[i] = new Cell[Width];
            }
            return newCells;
        }

        // Don't need JSON in this implementation
        ///// <summary>
        ///// Saves the game state to file using JSON and overrides any saved games already present at [gameFile].
        ///// </summary>
        ///// <param name="gameFile">File name and location string</param>
        //public void SaveGame(String gameFile)
        //{
        //    // CREATE JSON string representation of object
        //    string outputString = JsonConvert.SerializeObject(this);
        //    // SAVE JSON string to file
        //    File.WriteAllText(gameFile, outputString);
        //}

        /// <summary>
        /// Outputs a EF UserGame object based on this game.
        /// </summary>
        /// <returns>EF UserGame object based on this game</returns>
        public UserGame SaveGame()
        {
            // create a game and set its properties
            UserGame game = new UserGame()
            {
                Name = this.Name,
                Height = this.Height,
                Width = this.Width,
                //Creates a string for the EF class in its format
                Cells = this.ToString(AliveString, DeadString),
                User = this.User,
                UserGameID = this.UserGameID,
                UserID = this.UserID
            };
            // New UserGame created return it
            return game;
        }
    }
}
