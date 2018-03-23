using System;
using System.Text;
using System.IO;
using GoLA2.Models.Database;

namespace GoLA2.Models.Logic
{
    /// <summary>
    /// Provides a model of a valid template that can be used
    /// in the game of life as well as functionality to save
    /// to the template to file and create a new template from
    /// the string array read from a file.
    /// </summary>
    public class Template : ITemplate
    {
        /// <summary> Name used for saving to file. </summary>
        public string Name { get; set; }
        /// <summary> Height of the Cells grid </summary>
        public int Height { get; set; }
        /// <summary> Width of the Cells grid </summary>
        public int Width { get; set; }
        /// <summary> A grid of Grid.Alive and Grid.Dead cells </summary>
        public Cell[][] Cells { get; set; }
        // character references for importing/exporting cells
        private static char Alive = 'O';
        private static char Dead = 'X';
        private static char alive = 'o';
        private static char dead = 'x';

        /// <summary>
        /// Helper method for the constructors to avoid repeating the easy part
        /// of setting the name, height and width.
        /// </summary>
        /// <param name="name">name of template</param>
        /// <param name="height">height of template</param>
        /// <param name="width">width of template</param>
        private void SetMainParams(string name, int height, int width)
        {
            // set properties of class
            this.Name = name;
            // CHECK width and height are more than 0
            if (height <= 0 || width <= 0)
            {
                throw new FormatException("Both height and width must be > 0");
            }
            // ASSIGN valididated values to width and height
            else
            {
                this.Height = height;
                this.Width = width;
            }
        }

        /// <summary>
        /// Create a template based on pre-initialised values.
        /// </summary>
        /// <param name="name">Name of the template (used for exporting and importing).</param>
        /// <param name="height">Height of the cells grid (must match cells).</param>
        /// <param name="width">Width of the cells grid (must match cells).</param>
        /// <param name="cells">A grid of Cell.Alive and Cell.Dead values (must be height*width in size).</param>
        public Template(string name, int height, int width, Cell[][] cells)
        {
            SetMainParams(name, height, width);

            // CHECK the array height and width match the stated Height and Width
            if (cells.Length != Height || cells[0].Length != Width)
            {
                throw new FormatException("The array doesn't match the size stated, odd");
            }
            // CHECK the cells are all initialised
            else if (isNotValidCells(cells, Width))
            {
                throw new FormatException("The cells must be alive or dead");
            }
            // ASSIGN validated cells array
            else
            {
                this.Cells = cells;
            }
        }

        /// <summary>
        /// Creates a template based on a string of cells with rows delineated
        /// by new line characters.
        /// </summary>
        /// <param name="name">Name of template</param>
        /// <param name="height">Height of template</param>
        /// <param name="width">Width of template</param>
        /// <param name="cells">Grid of cells in string form with new lines to seperate rows</param>
        public Template(string name, int height, int width, string cells)
        {
            // Set the easy params
            SetMainParams(name, height, width);
            // Set the cells array
            SetCells(cells);
        }

        /// <summary>
        /// Helper method for constructors that takes a string of cells with 
        /// new line delineated rows. Assumes the template's width and height
        /// are already set and uses those to check for validity in the cells
        /// string
        /// </summary>
        /// <param name="cells">Grid of cells in string form with new lines to seperate rows</param>
        private void SetCells(string cells)
        {
            // Split the string at new lines
            String[] file = cells.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);

            // Create the cells array
            Cells = new Cell[Height][];
            // Loop through each row of the array and set the cells
            for (int i = 0; i < Cells.Length; i++)
            {
                // Create the Cell row array
                Cells[i] = new Cell[Width];
                // check the row to be entered matches the width expected
                if (file[i].Length == Width)
                {
                    // Loop through and set each cell in the row
                    for (int j = 0; j < file[i].Length; j++)
                    {
                        // check if char is dead and set Cell.Dead
                        if (isDeadCell(file[i][j]))
                        {
                            Cells[i][j] = Cell.Dead;
                        }
                        // Check if char is Alive and set Cell.Alive
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
                // Invalid row length send an error
                else
                {
                    throw new FormatException("The file is corrupt");
                }
            }
        }

        /// <summary>
        /// Creates a Template based on the EF UserTemplate class. 
        /// Does not assume valid template, checks are still made to
        /// ensure validity exception may occur if invalid template.
        /// </summary>
        /// <param name="game">EF UserTemplate class</param>
        public Template(UserTemplate game)
        {
            // Set the main pproperties (checks if height and width >0)
            SetMainParams(game.Name, game.Height, game.Width);
            // Set the cells based on the USerTemplate (does validation checks)
            SetCells(game.Cells);
        }

        /// <summary>
        /// Checks to see if parametres provided would be valid template
        /// without needing to create your own template object, if they are
        /// a Cell array is returned populated with Cell Enums based on
        /// the cell string provided.
        /// </summary>
        /// <param name="Height">int greater than 0</param>
        /// <param name="Width">int greater than 0</param>
        /// <param name="file">String array that contains x and o characters to represent the cells, each index represents a row.</param>
        /// <returns></returns>
        public static Cell[][] ValidTemplate(int Height, int Width, string[] file)
        {
            // Check grid size is at least one cell
            if (Height < 1 || Width < 1)
                throw new FormatException("Grid too small needs to be at least 1x1");

            // Check the string array is as tall as the Height
            if (file.Length == Height) // cursory check to make sure file is correct length
            {
                // Create the cell array array
                Cell[][] Cells = new Cell[Height][];
                // Loop through the rows and add the cells
                for (int i = 0; i < Height; i++)
                {
                    // create the cell row
                    Cells[i] = new Cell[Width];
                    // check the string length is the right width
                    if (file[i].Length == Width)
                    {
                        // Loop through the cells and add them to the array
                        for (int j = 0; j < Width; j++)
                        {
                            // is dead cell set dead cell
                            if (isDeadCell(file[i][j]))
                            {
                                Cells[i][j] = Cell.Dead;
                            }
                            // is alive cell set alive cell
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
                    // Row length is wrong have an error
                    else
                    {
                        throw new FormatException("The file is corrupt");
                    }
                }
                // Everything WORKED, return the created array
                return Cells;
            }
            // Cell array height is wrong have an error
            else
            {
                throw new FormatException("The file is corrupt");
            }
        }

        /// <summary>
        /// Outputs the template data to a file using the format
        /// provided by ToString().
        /// </summary>
        /// <param name="TemplateFolder">The folder name to export to.</param>
        /// <param name="TemplateExtension">The file extension to use.</param>
        /// <seealso cref="ToString()"/>
        public void OutputTemplateFile(string TemplateFolder, string TemplateExtension)
        {
            File.WriteAllText($"{TemplateFolder}{Name}{TemplateExtension}", ToString());
        }


        /// <summary>
        /// Outputs the key template data as a formated string that is 
        /// used for exporting to a file.
        /// </summary>
        /// <returns>
        /// Formated string in the form:
        /// From line 0 to line "height" ->[string of length "width" consisting of X and Os]\n.
        /// </returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (Cell[] carray in Cells)
            {
                foreach (Cell c in carray)
                {
                    if (c == Cell.Alive)
                        s.Append(Alive);
                    else
                        s.Append(Dead);
                }
                s.AppendLine();
            }
            return s.ToString();
        }

        /// <summary>
        /// Check the cells passed in are valid cells, that is they
        /// are all initialised to either Alive or Dead no nulls.
        /// </summary>
        /// <returns>
        /// RETURNS True if INVALID
        /// </returns>
        private static bool isNotValidCells(Cell[][] cells, int width)
        {
            bool isNotValid = false;
            foreach (Cell[] ca in cells)
            {
                if (ca.Length != width)
                    isNotValid = true;
                foreach (Cell c in ca)
                {
                    if (!(c.Equals(Cell.Alive) || c.Equals(Cell.Dead)))
                        isNotValid = true;
                }
            }

            return isNotValid;
        }

        /// <summary>
        /// Check if a cell is alive
        /// </summary>
        /// <param name="cell">The char of the cell we're looking at</param>
        /// <returns> true if alive</returns>
        private static bool isAliveCell(char cell)
        {
            if (cell == Alive || cell == alive)
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
            if (cell == Dead || cell == dead)
                return true;
            else
                return false;
        }
    }
}
