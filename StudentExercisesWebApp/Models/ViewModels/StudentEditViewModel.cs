using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesWebApp.Models.ViewModels
{
    public class StudentEditViewModel
    {
        // --------------------- EDITING THE STUDENT ------------------------------//
        // ---- What's the first thing the user needs to see ---- //
        // All the info about a given student in pre-filled form fields

        // ---- How can we get that information?--- //
        // SELECT everything from Student where Id matches the id in the route

        // ----Where should we store this info?----//
        // We already have a custom Student type-- let's just add a property on this view model of type student and store the information from the DB there.


        // ------------------------ EDITING THEIR EXERCISES -------------------------//

        // ---- What does the user needs to see? ----//
        // A multi-select of all the exercises. The ones they're currently working on should be pre-selected. 

        // ---- How can we get this information? ---//
        // First we can SELECT everything from the exercise table to get a list of ALL the available exercises. We also need to know which ones the student is currently working on. We can get that by joining in the student exercise table and looking for entries that match the given student's Id.

        // ---- Where should we store this information? ----//
        // We need to store two lists of exercises -- one for ALL of the exercises and one for the ones the student is currently working on. The list of all exercises needs to be a list of select list items. For the ones they're currently working on, we can use the AssignedExercises property on the Student model.


        // ------------------- EDITING THEIR COHORT -------------------------//

        // ----- What does the user need to see? -----//
        // A select list of all the cohorts, with the student's current cohort pre-selected. 
        // ----- How do we get this information? -----//
        // First step- SELECT everything from the Cohort table
        // Second step - figure out which cohort is assinged to this student (we may already have this information from the student's CohortId property

        // ---- Where should we store this information? ----// 
        // We'll need a list of select list items of all the cohorts. We can store the student's current cohort Id on this view models Student property. 


        public Student Student { get; set; }

        [Display(Name = "Exercises")]
        public List<SelectListItem> AllExercises { get; set; } = new List<SelectListItem>();

        // We'll use this list of integers later, when the data comes back from the form.
        public List<int> SelectedExercises { get; set; }
        public List<SelectListItem> Cohorts { get; set; }

        public StudentEditViewModel() { }

        public StudentEditViewModel(int studentId, string connectionString)
        {

        // Get the student's information from the db and assign it to the Student property

        // Get the exercises that are currently assigned to this student

        // Get ALL the exercises and convert them into a list of select list items

        // Get ALL the cohorts and assign them to a list of select list items

        // HINT: You can decide which options are pre-selected by adding a SELECTED property to the Select List Item instance -- https://docs.microsoft.com/en-us/dotnet/api/system.web.mvc.selectlistitem.selected?view=aspnet-mvc-5.2


        }

    }
}
