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
 * Date: 9/8/2022 - 13/9/2022
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
 *  
 *  IMPORTANT FOR editRecord() DONT SEARCH IF NAME NOT CHANGED
 *  Add nullIndex++ to end of fileReader() as well as in the loop
 */
namespace AT1_WikiPrototype
{
    public partial class FormWiki : Form
    {
        // When using maxRecords for final index remember to -1 from it
        private static int maxRecords = 12;
        // Initialise the 2D array
        string[,] myRecordsArray = new string[maxRecords, 4];
        public FormWiki()
        {
            InitializeComponent();
        }
        // Index of first null in array
        private int nullIndex = 0;
        // Set in StatusMsg() so window is shorter than current screen height
        private int maxWindowHeight;
        private string appendErrMsg = "";

        // ______________________NOT FINISHED_______________________
        // btn to add a record to myRecordsArray & display it
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddRecord();
        }

        // Searches for a record's name that matches tbSearch & if found selects
        //   record in listview displaying its details
        // Focuses & clear tbSearch after search
        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(tbSearch.Text))
                {
                    int matchIndex = SearchRecords(tbSearch.Text);
                    if (matchIndex != -1)
                    {
                        // Selects a record in listview which is detected by
                        //   listRecords_SelectedIndexChanged() so the record is displayed
                        listViewRecords.Items[matchIndex].Selected = true;
                        StatusMsg("Match found for \"" + tbSearch.Text + "\" at index: " 
                            + matchIndex, true);
                    }
                    else
                    {
                        StatusMsg("No match found for \"" + tbSearch.Text + "\"", true);
                    }
                }
                else
                {
                    StatusMsg("ERROR Invalid Input: Value NOT searched"
                        + "\nReason: Search field was empty", true);
                }
                tbSearch.Clear();
                tbSearch.Focus();
                tbSearch.SelectAll();
            }
        }

        // Displays records in listViewRecords after sort
        private void DisplayRecords()
        {
            // Clear the list
            listViewRecords.Items.Clear();
            BubbleSort();
            for (int i = 0; i < nullIndex; i++)
            {
                // Error message if nullIndex is wrong & exit loop
                if (string.IsNullOrEmpty(myRecordsArray[i, 0]))
                {
                    appendErrMsg += "ERROR: nullIndex = " + nullIndex + " however index "
                        + i + " is null\n";
                    break;
                }
                ListViewItem listView1 = new ListViewItem(myRecordsArray[i, 0]);
                listView1.SubItems.Add(myRecordsArray[i, 1]);
                listViewRecords.Items.Add(listView1);
            }
        }

        // ______________________NOT FINISHED_______________________
        // Add record details to myRecordsArray if valid
        private bool AddRecord()
        {
            bool wasAdded = false;
            string statMsg = "ERROR CANNOT Add Record: Already at max capacity" 
                + "\nReason: Maxium of " + maxRecords + " records can be stored, " 
                + "delete a record to add a new one";
            // Check if space in array
            if (nullIndex <= maxRecords-1)
            {
                statMsg = "";
                // hasData = false if invalid field (stat-msg but still add record)
                // hasName: if false DONT add record
                bool hasName = true, hasData = true;
                int duplicateFound;
                string missingField = "";

                if (String.IsNullOrEmpty(tbName.Text))
                {
                    hasName = false;
                }
                if (String.IsNullOrEmpty(tbCategory.Text))
                {
                    hasData = false;
                    missingField = "Category, ";
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

                duplicateFound = SearchRecords(tbName.Text);
                if (hasName == false)
                {
                    tbName.Focus();
                    tbName.SelectAll();
                    statMsg += "ERROR Invalid Input: Record was NOT added"
                        + "\nReason: Name field CANNOT be empty";
                }
                else if (duplicateFound != -1)
                {
                    tbName.Focus();
                    tbName.SelectAll();
                    statMsg += "ERROR Invalid Input: Record was NOT added"
                        + "\nReason: Duplicate names are NOT ALLOWED "
                        + "\nA record with the name: \"" + tbName.Text
                        + "\" already exists at index " + duplicateFound;
                }

                // Add record to myRecordsArray[]
                if (hasName == true && duplicateFound == -1)
                {
                    myRecordsArray[nullIndex, 0] = tbName.Text;
                    myRecordsArray[nullIndex, 1] = tbCategory.Text;
                    myRecordsArray[nullIndex, 2] = tbStructure.Text;
                    myRecordsArray[nullIndex, 3] = tbDefinition.Text;

                    wasAdded = true;
                    nullIndex++;
                    statMsg += "Record called \"" 
                        + myRecordsArray[nullIndex, 0] + "\" was added";
                    BubbleSort();
                }
                if (hasData == false)
                {
                    statMsg += "\nThe following field(s) are empty: "
                        + missingField.Remove(missingField.Length - 2)
                        + "\nRemember to fill them in later\n";
                }
            }
            // Display message in status strip & true to word wrap
            StatusMsg(statMsg, true);
            return wasAdded;
        }

        // __________________NOT TESTED NEEDS DISPLAY____________________
        // Binary search of array to match searchTxt, return -1 if not found
        private int SearchRecords(string searchTxt)
        {
            int foundIndex = -1;
            int startIndex = -1;
            int finalIndex = nullIndex;
            bool flag = false;

            while (!flag && !((finalIndex - startIndex) <= 1))
            {
                int newIndex = (finalIndex + startIndex) / 2;
                // Compare: == if myRecordsArray[newIndex, 0] same position in 
                //   sort order as tbName.Text
                if (String.Compare(myRecordsArray[newIndex, 0], searchTxt, 
                    StringComparison.OrdinalIgnoreCase) == 0)
                {
                    foundIndex = newIndex;
                    flag = true;
                }
                else
                {
                    // Compare: > if myRecordsArray[newIndex, 0] precedes 
                    //  tbName.Text's position in sort order 
                    if (String.Compare(myRecordsArray[newIndex, 0], searchTxt, 
                        StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        finalIndex = newIndex;
                    }
                    // Compare: < if tbName.Text precedes 
                    //  myRecordsArray[newIndex, 0]'s position in sort order 
                    else
                    {
                        startIndex = newIndex;
                    }
                }
            }
            // Return -1 for no match found
            return foundIndex;
        }

        // Sorts myRecordsArray by name (a to z)
        // Calls Swapper() to swaps record position with the next index
        private void BubbleSort()
        {
            bool flag = false;
            int duplicateIndex = -1;
            do
            {
                flag = false;
                for (int i = 0; i < nullIndex - 1; i++)
                {
                    if (String.IsNullOrEmpty(myRecordsArray[(i + 1), 0]))
                    {
                        break;
                    }
                    // Sorts a to z
                    // CompareTo output: "abc".CompareTo("bcd")=-1, bcd.abc=1,  
                    //   abc.abc=0, ABC.abc=1, acc.abc=1
                    // Records with identical names are placed together (CANT happen i think)
                    else if (myRecordsArray[i, 0].CompareTo(myRecordsArray[(i+1), 0]) > 0)
                    {
                        Swapper(i, (i + 1));
                        // Loops until nothing left to swap
                        flag = true;
                    }
                    else if (String.Compare(myRecordsArray[i, 0], myRecordsArray[(i + 1), 0], 
                        StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        duplicateIndex = i;
                    }
                }
            } while (flag == true);
            if (duplicateIndex != -1)
            {
                appendErrMsg += "ERROR: The name \"" + myRecordsArray[duplicateIndex, 0]
                            + "\" is both index: " + duplicateIndex 
                            + " and " + (duplicateIndex + 1 + "\n");
            }
        }

        // Swaps array's index 'i' with 'i+1' to swap record's position with the next
        // Used by BubbleSort() to order records 
        private void Swapper(int a, int b)
        {
            string temp;
            // Swapping names
            temp = myRecordsArray[a, 0];
            myRecordsArray[a, 0] = myRecordsArray[b, 0];
            myRecordsArray[b, 0] = temp;
            // Swapping categories
            temp = myRecordsArray[a, 1];
            myRecordsArray[a, 1] = myRecordsArray[b, 1];
            myRecordsArray[b, 1] = temp;
            // Swapping structures
            temp = myRecordsArray[a, 2];
            myRecordsArray[a, 2] = myRecordsArray[b, 2];
            myRecordsArray[b, 2] = temp;
            // Swapping definition
            temp = myRecordsArray[a, 3];
            myRecordsArray[a, 3] = myRecordsArray[b, 3];
            myRecordsArray[b, 3] = temp;
        }

        // Displays the status message after formating msg & strip
        private void StatusMsg(string statMsg, bool wrapTxt)
        {
            int originalHeight = statusStrip1.Height;
            // Strip text line NOT visible if >95 char for default size
            int maxLength = (int)(Convert.ToDouble(statStripLabel.Width)/4.8) - 3;
            statMsg = (statMsg.Trim('\n') + "\n\n" + appendErrMsg.Trim('\n')).Trim('\n');
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
            statStripLabel.Text = statMsg;
            appendErrMsg = "";

            // maxWindowHeight set to 90% of current screen height
            maxWindowHeight = Screen.FromControl(this).Bounds.Height /10*9;
            int newStripHeight = statusStrip1.Height - originalHeight;
            // Increase height of window so status strip isn't covering stuff
            if (this.Height + newStripHeight < maxWindowHeight)
                this.Height += statusStrip1.Height - originalHeight;
            else
                this.Height = maxWindowHeight;
        }

        // Outputs details of the record selected in the listview to their textboxes
        private void SelectRecord()
        {
            int selectedIndex = listViewRecords.SelectedIndices[0];
            tbName.Text = myRecordsArray[selectedIndex, 0];
            tbCategory.Text = myRecordsArray[selectedIndex, 1];
            tbStructure.Text = myRecordsArray[selectedIndex, 2];
            tbDefinition.Text = myRecordsArray[selectedIndex, 3];
        }

        // Detects if a record is selected in the listview & calls SelectRecords()
        //   to display its details (if it detects selection is null does nothing)
        private void listRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do nothing if the SelectedIndexChanged() detected was nothing selected
            if (listViewRecords.SelectedIndices.Count > 0)
            {
                SelectRecord();
            }
        }

        // ______________________EMPTY METHOD_______________________
        // DECIDE IF I WANT TO LOAD DATA ON LAUNCH (CURRENTLY I DONT)
        private void FormWiki_Load(object sender, EventArgs e)
        {
            /*MessageBox.Show("On Load - Testing MessageBox: "
                + "\n1. What1 " + nullIndex
                + "\n2. What2 " + nullIndex + " EndOfMsg");*/
        }
    }
}
