using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All(); // Load list data
        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();
        public CreateTeamForm()
        {
            InitializeComponent();
            //CreateSampleData();
            WireUpLists();
        }

        //Create sample data to add to list to test if they are functional
        private void CreateSampleData()
        {
            availableTeamMembers.Add(new PersonModel { FirstName = "Armando", LastName = "Aranda" });
            availableTeamMembers.Add(new PersonModel { FirstName = "Max", LastName = "Aranda" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "Leon", LastName = "Aranda" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "Mabel", LastName = "Aranda" });
        }

        // This will wire up our drop down and list box to a list
        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null; // This will force a refresh on the list

            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName"; // This string represents one property in the PersonModel

            teamMembersListBox.DataSource = null;// This will force a refresh on the list

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";
        }
        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel p = new PersonModel();

                p.FirstName = firstNameValue.Text;
                p.LastName = lastNameValue.Text;
                p.EmailAddress = emailValue.Text;
                p.CellphoneNumber = cellphoneValue.Text;

                GlobalConfig.Connection.CreatePerson(p);

                selectedTeamMembers.Add(p);

                WireUpLists();

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellphoneValue.Text = "";
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields.");
            }
        }

        private bool ValidateForm()
        {
            if (firstNameValue.Text.Length == 0)
            {
                return false;
            }
            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }
            if (emailValue.Text.Length == 0)
            {
                return false;
            }
            if (cellphoneValue.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        // This will take a person from the Select Team Member drop down and put them into the List Box
        private void addMemberButton_Click(object sender, EventArgs e)
        {
            // Add in who is being selected
            PersonModel p = (PersonModel)selectTeamMemberDropDown.SelectedItem;

            if (p != null)
            {
                // Removes the person from the Team Members list
                availableTeamMembers.Remove(p);
                // Adds person to selected Team Members list
                selectedTeamMembers.Add(p);

                WireUpLists(); 
            }

        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;


            if (p != null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);

                WireUpLists();  
            }
        }
    }
}
