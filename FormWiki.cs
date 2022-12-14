using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/* Name: Corin Little
 * ID: P453208
 * Date: 9/8/2022 - 20/9/2022
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
 *  - Records' 4 String fields for Name, Category, Structure & Definition.
 *  - Can add/edit/delete records
 *  - Binary search records by Name (NO built-in)
 *  - definitions.dat to stored the records by create/save/load from it. (Choose location)
 *  - Selecting a record causes its data to be display in the 4 fields
 *  - Sort array by name using separate bubble sort & swap methods (NO built-in)
 *  - Clear all 4 field boxes when name field is double clicked
 *  - Full error trapping & feedback messages.
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

        // btn to add a valid record to myRecordsArray & display it
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (AddRecord())
            {
                ClearFields();
                tbName.Focus();
            }
            DisplayRecords();
        }

        // btn to edit the selected record if new field values are valid & display it
        private void btnEdit_Click(object sender, EventArgs e)
        {
            // If no record selected output message that a record must be selected
            if (listViewRecords.SelectedIndices.Count > 0)
            {
                if (EditRecord(listViewRecords.SelectedIndices[0]))
                {
                    ClearFields();
                    tbName.Focus();
                    DisplayRecords();
                }
            }
            else
            {
                StatusMsg("Error: Record was NOT editted \nReason: No record " 
                    + "was selected to edit", true);
            }
        }

        // btn to delete the selected record if confirmed & update listview
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // If no record selected output message that a record must be selected
            if (listViewRecords.SelectedIndices.Count > 0)
            {
                int selectedIndex = listViewRecords.SelectedIndices[0];
                DialogResult result = MessageBox.Show(("Are sure you want to delete "
                    + "the record called \"" + myRecordsArray[selectedIndex, 0] 
                    + "\" at index " + selectedIndex + "\n\nClick 'Yes' to delete the"
                    + " record\nClick 'No' to cancel deletion"),
                    "Confirm Deletion", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    DeleteRecord(selectedIndex);
                    ClearFields();
                    tbSearch.Focus();
                    tbSearch.SelectAll();
                    DisplayRecords();
                }
                else
                {
                    StatusMsg("Record deletion was cancelled", true);
                }
            }
            else
            {
                StatusMsg("Error: Record was NOT deleted \nReason: No record "
                    + "was selected to delete", true);
            }

        }

        // btn to save the records array & select location (definitions.dat)
        private void btnSave_Click(object sender, EventArgs e)
        {
            // If no records in array ask want to save
            if (nullIndex == 0)
            {
                DialogResult result = MessageBox.Show(("Currently no records are loaded."
                    + "\nAre you sure you want to save."),
                    "Begin Save", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
            }
            
            saveFileDialogWiki.FileName = "definitions";
            if (saveFileDialogWiki.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(saveFileDialogWiki.FileName).ToLower() != ".dat")
                {
                    saveFileDialogWiki.FileName += ".dat";
                }
                FileWriter(saveFileDialogWiki.FileName);
                saveFileDialogWiki.InitialDirectory = Path.GetDirectoryName(
                    saveFileDialogWiki.FileName);
            }
            DisplayRecords();
        }

        // btn to load the records to the array by selecting a file (definitions.dat)
        private void btnLoad_Click(object sender, EventArgs e)
        {
            // If no records in array ask want to save
            if (nullIndex > 0)
            {
                DialogResult result = MessageBox.Show(("Loading a records file will " 
                    + "overwrite current records."
                    + "\nAre you sure you want to load records from file."),
                    "Warning: Overwrite current records", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
            }

            openFileDialogWiki.FileName = "definitions";
            if (openFileDialogWiki.ShowDialog() == DialogResult.OK)
            {
                FileReader(openFileDialogWiki.FileName);
                openFileDialogWiki.InitialDirectory = Path.GetDirectoryName(
                    openFileDialogWiki.FileName);
            }
            DisplayRecords();
        }

        // Searches for a record's name that matches tbSearch & if found selects
        //   record in listview displaying its details
        // Focuses & clear tbSearch after search
        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                listViewRecords.SelectedIndices.Clear();
                ClearFields();
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

        // If tbName is double clicked clear all 4 fields & focuses tbName
        private void tbName_DoubleClick(object sender, EventArgs e)
        {
            ClearFields();
            tbName.Focus();
            tbName.SelectAll();
            StatusMsg("All fields have been cleared", true);
        }

        // Detects if a record is selected/unselected in the listview 
        //   & calls SelectRecords() to display its details
        private void listRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectRecord();
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

        // Add new record to myRecordsArray if valid
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

                // Add record to myRecordsArray[] if valid
                if (hasName == true && duplicateFound == -1)
                {
                    myRecordsArray[nullIndex, 0] = tbName.Text;
                    myRecordsArray[nullIndex, 1] = tbCategory.Text;
                    myRecordsArray[nullIndex, 2] = tbStructure.Text;
                    myRecordsArray[nullIndex, 3] = tbDefinition.Text;

                    wasAdded = true;
                    statMsg += "Record called \"" + myRecordsArray[nullIndex, 0] 
                        + "\" was added (" + (nullIndex+1) + "/" + maxRecords + " records)";
                    nullIndex++;
                    BubbleSort();
                    if (hasData == false)
                    {
                        statMsg += "\nThe following field(s) are empty: "
                            + missingField.Remove(missingField.Length - 2)
                            + "\nRemember to fill them in later\n";
                    }
                }
            }
            // Display message in status strip & true to word wrap
            StatusMsg(statMsg, true);
            return wasAdded;
        }

        // Edit record & update array if valid at sent index
        private bool EditRecord(int editIndex)
        {
            bool wasEditted = false;
            string statMsg = "", nameChange = "";
            // hasName: if false DONT edit record
            bool hasName = true;
            // if duplicateFound != -1 then a duplicate was found
            int duplicateFound = -1;

            if (String.Compare(myRecordsArray[editIndex, 0], tbName.Text,
                    StringComparison.OrdinalIgnoreCase) != 0)
            {
                duplicateFound = SearchRecords(tbName.Text);
                nameChange = "\nname was changed from \"" + myRecordsArray[editIndex, 0] 
                    + "\" to \"" + tbName.Text + "\"";
            }
            if (String.IsNullOrEmpty(tbName.Text))
            {
                hasName = false;
                tbName.Focus();
                tbName.SelectAll();
                statMsg += "ERROR Invalid Input: Record was NOT editted"
                    + "\nReason: Name field CANNOT be empty";
            }
            else if (duplicateFound != -1)
            {
                tbName.Focus();
                tbName.SelectAll();
                statMsg += "ERROR Invalid Input: Record was NOT editted"
                    + "\nReason: Duplicate names are NOT ALLOWED "
                    + "\nA record with the name: \"" + tbName.Text
                    + "\" already exists at index " + duplicateFound;
            }

            // Edit record & update myRecordsArray[] if true
            if (hasName == true && duplicateFound == -1)
            {
                myRecordsArray[editIndex, 0] = tbName.Text;
                myRecordsArray[editIndex, 1] = tbCategory.Text;
                myRecordsArray[editIndex, 2] = tbStructure.Text;
                myRecordsArray[editIndex, 3] = tbDefinition.Text;

                wasEditted = true;
                statMsg += "Record called \"" + myRecordsArray[editIndex, 0]
                    + "\" was editted" + nameChange;
                BubbleSort();
            }
            // Display message in status strip & true to word wrap
            StatusMsg(statMsg, true);
            return wasEditted;
        }

        // Delete record after confirmation at sent index
        private void DeleteRecord(int delIndex)
        {
            string statMsg = "Deleted record called \"" + myRecordsArray[delIndex, 0]
                    + "\" that was located at index " + delIndex + " ("
                    + (nullIndex-1) + "/" + maxRecords + " records)";
            for (int i = delIndex; i < nullIndex - 1; i++)
            {
                Swapper(i, (i + 1));
            }
            nullIndex--;
            myRecordsArray[nullIndex, 0] = null;
            myRecordsArray[nullIndex, 1] = null;
            myRecordsArray[nullIndex, 2] = null;
            myRecordsArray[nullIndex, 3] = null;
            StatusMsg(statMsg, true);
        }

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
                // Loop ends if flag NOT changed to true before end of loop
                flag = false;
                for (int i = 0; i < nullIndex - 1; i++)
                {
                    if (String.IsNullOrEmpty(myRecordsArray[(i + 1), 0]))
                    {
                        break;
                    }
                    /* Sorts a to z, should trigger until nothing left to swap
                     * CompareTo output: "abc".CompareTo("bcd")=-1, bcd.abc=1, 
                     *   abc.abc=0, ABC.abc=1, acc.abc=1 
                     * Records with identical names are placed together (CANT happen i think) */
                    else if (myRecordsArray[i, 0].CompareTo(myRecordsArray[(i+1), 0]) > 0)
                    {
                        Swapper(i, (i + 1));
                        // Set 'true' to keep looping
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
                statMsg = msgParts.Trim('\n');
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
        // Bolds only the selected item
        private void SelectRecord()
        {
            // Unbolds all items first so only selected item is bold
            Font notSelectedFont = new Font(listViewRecords.Font, FontStyle.Regular);
            for (int i = 0; i < nullIndex; i++)
            {
                listViewRecords.Items[i].Font = notSelectedFont;
            }
            // If it detects an item is selected make its name bold
            if (listViewRecords.SelectedIndices.Count != 0)
            {
                int selectedIndex = listViewRecords.SelectedIndices[0];
                listViewRecords.Items[selectedIndex].Font =
                    new Font(listViewRecords.Font, FontStyle.Bold);
                tbName.Text = myRecordsArray[selectedIndex, 0];
                tbCategory.Text = myRecordsArray[selectedIndex, 1];
                tbStructure.Text = myRecordsArray[selectedIndex, 2];
                tbDefinition.Text = myRecordsArray[selectedIndex, 3];
            }
        }

        // Clears all 4 fields (NOT tbSearch or listview)
        private void ClearFields()
        {
            tbName.Clear();
            tbCategory.Clear();
            tbStructure.Clear();
            tbDefinition.Clear();
        }
        
        // Records data is written to selected file (definitions.dat)
        private void FileWriter(string filePath)
        {
            BinaryWriter bw;

            // Create the file
            try
            {
                bw = new BinaryWriter(new FileStream(filePath, FileMode.Create));
            }
            catch (Exception fe)
            {
                MessageBox.Show(" ERROR: Cannot write data to file."
                    + "\nFilepath: " + filePath + "\n\n" + fe.Message);
                return;
            }

            // Writing to file
            try
            {
                for (int i = 0; i < nullIndex; i++)
                {
                    bw.Write(myRecordsArray[i, 0]);
                    bw.Write(myRecordsArray[i, 1]);
                    bw.Write(myRecordsArray[i, 2]);
                    bw.Write(myRecordsArray[i, 3]);
                }
                StatusMsg(nullIndex + " record/s were successfully saved to:\n" + filePath, true);
            }
            catch (Exception fe)
            {
                MessageBox.Show(" ERROR: Cannot write data to file."
                    + "\nFilepath: " + filePath + "\n\n" + fe.Message);
            }

            bw.Close();
        }

        // Reading from selected file (definitions.dat) 
        private void FileReader(string filePath)
        {
            BinaryReader br;
            try
            {
                br = new BinaryReader(new FileStream(filePath,
                    FileMode.Open));
            }
            catch (Exception fe)
            {
                MessageBox.Show(" ERROR: Cannot open file to read data from."
                    + "\nFilepath: " + filePath + "\n\n" + fe.Message);
                return;
            }

            // Clear current array by initialising again
            myRecordsArray = new string[maxRecords, 4];
            // Read data
            try
            {
                for (int i = 0; i < maxRecords; i++)
                {
                    // Exit loop if reached end of file
                    if (br.BaseStream.Position == br.BaseStream.Length)
                    { break; }
                    myRecordsArray[i, 0] = br.ReadString();
                    myRecordsArray[i, 1] = br.ReadString();
                    myRecordsArray[i, 2] = br.ReadString();
                    myRecordsArray[i, 3] = br.ReadString();
                    nullIndex = i;
                }
                nullIndex++;
                StatusMsg("Loaded " + nullIndex + " record/s from:\n" + filePath, true);
            }
            catch (EndOfStreamException eof) // Catches the EOF
            {
                MessageBox.Show("EOF reached, no more data.");
                nullIndex++;
            }
            catch (Exception fe)
            {
                MessageBox.Show(" ERROR: Cannot read data from file."
                + "\nFilepath: " + filePath + "\n\n" + fe.Message);
            }
            br.Close();
        }

        // On load sets InitialDirectory & status strip to display some tips
        private void FormWiki_Load(object sender, EventArgs e)
        {
            string initialPath = Path.Combine(Application.StartupPath, @"");
            saveFileDialogWiki.InitialDirectory = initialPath;
            openFileDialogWiki.InitialDirectory = initialPath;
            StatusMsg("Tips: " +
                "1. Press 'Load from File' to load saved records.\n         " +
                "2. Press the 'Enter' key in the 'Search' box to search input.\n" +
                "         3. Records with the same name cannot be added.\n" +
                "         4. Clicking on a record will select it & show its details " +
                "in the fields.\n         5. Double click the 'Name' field to clear " +
                "all 4 fields.", false);
        }

        // On close asks to select location to save the records array (definitions.dat)
        private void FormWiki_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If any records in array ask to save
            if (nullIndex != 0)
            {
                saveFileDialogWiki.FileName = "definitions";
                if (saveFileDialogWiki.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(saveFileDialogWiki.FileName).ToLower() != ".dat")
                    {
                        saveFileDialogWiki.FileName += ".dat";
                    }
                    FileWriter(saveFileDialogWiki.FileName);
                    saveFileDialogWiki.InitialDirectory = Path.GetDirectoryName(
                        saveFileDialogWiki.FileName);
                }
            }
        }
    }
}
