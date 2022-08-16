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
 * Date: 9/8/2022
 * Purpose: AT1 - Project Wiki Prototype */
/* Case Study – Data Structures Wiki Catalogue
 * As a senior programmer for CITE Managed Services develop a wiki app prototype 
 *  for junior programmers cataloguing Data Structures with the fields Name, 
 *  Category, Structure & Definition using a Window Forms Application.
 * A rudimentary interface design is provided.
 * Need to use GitHub version control for project. */
/* Basic Structure:
 *  - ONLY allowed 1 class: FormWIki.cs
 *  - Simple global 2D String array to store records: myRecordsArray[11, 3].
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
        public FormWiki()
        {
            InitializeComponent();
        }
    }
}
