using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/* Name: Corin Little
 * ID: P453208
 * Date: 9/8/2022 - 30/8/2022
 * Purpose: AT1 - Project Wiki Prototype */
/* Case Study – Data Structures Wiki Catalogue
 * As a senior programmer for CITE Managed Services develop a wiki app prototype 
 *  for junior programmers cataloguing Data Structures with the fields Name, 
 *  Category, Structure & Definition using a Window Forms Application.
 * A rudimentary interface design is provided.
 * Need to use GitHub version control for project. */
/* Basic Structure:
 *  - ONLY allowed 1 class: FormWIki.cs
 *  - Simple global 2D String array to store records: myRecordsArray[12, 4].
 *  - Records' 4 String fields: recName, recCategory, recStructure & recDefinition.
 *  - Can add/edit/delete records
 *  - Search records by Name.
 *  - Binary file definitions.dat to stored the records by create/save/load from it.
 *  - Full error trapping & feedback messages.
 *  - Selecting a record causes its data to be display in the 4 fields
 *  - Sort array by name using separate bubble sort & swap methods
 *  - Clear all 4 field boxes when name field is double clicked
 * Form Design:
 *  - Listbox displaying selectable records sorted by name.
 *  - 4 textboxes for the 4 fields.
 *  - A textbox for searching.
 *  - Buttons to add, edit, delete, save & load.
 *  - Status strip to display error messages.
 */
namespace AT1_WikiPrototype
{
    public partial class FormWiki : Form
    {
        // Initialise the 2D array
        string[,] myRecordsArray = new string[12, 4];
        public FormWiki()
        {
            InitializeComponent();
        }
        // Index of first null in array
        private int nullIndex = 0;
        // Get screen height so window doesn't grow too tall
        private int maxWindowHeight = Screen.PrimaryScreen.Bounds.Height/10*9;

        // ______________________NOT FINISHED_______________________
        // btn to add a record to myRecordsArray & display it
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddRecord();
        }

        // ______________________NOT FINISHED_______________________
        // Add record details to myRecordsArray if valid
        private void AddRecord()
        {
            // hasData = false if invalid field (stat-msg but still add record)
            // hasName: if false DONT add record
            bool hasName = true, hasData = true;
            int duplicateFound;
            string missingField = "", statMsg = "";

            if (String.IsNullOrEmpty(tbName.Text))
            {
                hasName = false;
                hasData = false;
                missingField = "Name, ";
            }
            if (String.IsNullOrEmpty(tbCategory.Text))
            {
                hasData = false;
                missingField += "Category, ";
            }
            if (String.IsNullOrEmpty(tbStructure.Text))
            {
                hasData = false;
                missingField += "Structure, ";
            }
            if (String.IsNullOrEmpty(tbDefinition.Text))
            {
                hasData = false;
                missingField += "Definition, ";
            }

            // __________________HERE_______________________
            duplicateFound = SearchRecords(tbName.Text, -1);
            if (duplicateFound != -1)
            {

            }

            if (hasData == false)
            {
                statMsg += "The following field(s) are empty: " 
                    + missingField.Remove(missingField.Length-2)
                    + "\nRemember to fill them in later\n";
            }
            // Display message in status strip & true to word wrap
            StatusMsg(statMsg, true);
        }

        // ______________________NOT FINISHED_______________________
        // Binary search of array to match searchTxt, -1 = not found
        private int SearchRecords(string searchTxt, int ignoreIndex)
        {
            int recIndex = -1;
            int startIndex = -1;
            int finalIndex = nullIndex;
            bool flag = false;
            int foundIndex = -1;

            while (!flag && !((finalIndex - startIndex) <= 1))
            {
                int newIndex = (finalIndex + startIndex) / 2;
                // Compare: == if myRecordsArray[newIndex, 0] same position in 
                //  sort order as tbName.Text
                if (String.Compare(myRecordsArray[newIndex, 0], tbName.Text) == 0)
                {
                    foundIndex = newIndex;
                    flag = true;
                    break;
                }
                else
                {
                    // Compare: > if myRecordsArray[newIndex, 0] precedes 
                    //  tbName.Text's position in sort order 
                    if (String.Compare(myRecordsArray[newIndex, 0], tbName.Text) > 0)
                    { finalIndex = newIndex; }
                    // Compare: < if tbName.Text precedes 
                    //  myRecordsArray[newIndex, 0]'s position in sort order 
                    else
                    { startIndex = newIndex; }
                }
            }
            if (flag == true)
            {
                // Populate fields & list
                tbName.Text = myRecordsArray[foundIndex, 0];
                tbCategory.Text = myRecordsArray[foundIndex, 1];
                ListViewItem listView1 = new ListViewItem(myRecordsArray[foundIndex, 0]);
                listView1.SubItems.Add(myRecordsArray[foundIndex, 1]);
                listView1.SubItems.Add(myRecordsArray[foundIndex, 2]);
                listView1.SubItems.Add(myRecordsArray[foundIndex, 3]);
                listViewRecords.Items.Add(listView1);
            }

            // Return -1 for no match found
            return recIndex;
        }

        // Displays the status message after formating msg & strip
        private void StatusMsg(string statMsg, bool wrapTxt)
        {
            int originalHeight = statusStrip1.Height;
            // Strip text line NOT visible if >95 char for default size
            int maxLength = (int)(Convert.ToDouble(statStripLabel.Width)/4.8) - 3;
            
            // Manual word wrap for the status strip
            if (wrapTxt == true)
            {
                string[] newLines = statMsg.Split('\n');
                string msgParts = "";
                foreach (var line in newLines)
                {
                    string[] words = line.Split(' ');
                    var parts = new Dictionary<int, string>();
                    string part = string.Empty;
                    int partCounter = 0;
                    foreach (var word in words)
                    {
                        if (part.Length + word.Length < maxLength)
                            part += string.IsNullOrEmpty(part) ? word : " " + word;
                        else
                        {
                            parts.Add(partCounter, part);
                            part = word;
                            partCounter++;
                        }
                    }
                    parts.Add(partCounter, part);
                    foreach (var item in parts)
                        msgParts += item.Value + "\n";
                }
                statMsg = msgParts;
            }
            statStripLabel.Text = statMsg.Trim('\n');

            int newStripHeight = statusStrip1.Height - originalHeight;
            // Increase height of window so status strip isn't covering stuff
            if (this.Height + newStripHeight < maxWindowHeight)
            {
                this.Height += statusStrip1.Height - originalHeight;
            }
            else
            {
                this.Height = maxWindowHeight;
            }
        }
    }
}
