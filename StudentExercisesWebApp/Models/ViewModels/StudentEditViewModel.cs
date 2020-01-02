using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesWebApp.Models.ViewModels
{
    public class StudentEditViewModel
    {
        public Student Student { get; set; }

        [Display(Name = "Exercises")]
        public List<SelectListItem> AllExercises { get; set; } = new List<SelectListItem>();

        // We'll use this list of integers later, when the data comes back from the form.
        public List<int> SelectedExercises { get; set; }

        public List<SelectListItem> Cohorts { get; set; }

        protected string _connectionString;

        protected SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public StudentEditViewModel() { }

        public StudentEditViewModel(int studentId, string connectionString)
        {
            _connectionString = connectionString;

            // Get the student's information and assign it to the Student property
            Student = GetOneStudent(studentId);

            // Get the exercises that are currently assigned to this student
            Student.AssignedExercises = GetAssignedExercisesByStudent(studentId);

            // Get ALL the exercises and convert them into a list of select list items
            AllExercises = GetAllExercises().Select(singleExercise => new SelectListItem()
            {
                Text = singleExercise.Name,
                Value = singleExercise.Id.ToString(),
                Selected = Student.AssignedExercises.Any(assignedExercise => assignedExercise.Id == singleExercise.Id)
            })
            .ToList();

            // Get ALL the cohorts and assign them to a list of select list items
            Cohorts = GetAllCohorts()
               .Select(cohort => new SelectListItem()
               {
                   Text = cohort.Name,
                   Value = cohort.Id.ToString(),
                   Selected = Student.CohortId == cohort.Id
               })
               .ToList();


        }

        private Student GetOneStudent(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, FirstName, LastName, SlackHandle, CohortId
                        FROM Student
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student student = null;

                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),

                        };
                    }
                    reader.Close();

                    return student;
                }
            }

        }

        private List<Exercise> GetAssignedExercisesByStudent(int studentId)
        {
            List<Exercise> assignedExercises = new List<Exercise>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT e.Id, e.Name, e.Language FROM Exercise e JOIN StudentExercise se ON e.Id=se.ExerciseId WHERE se.StudentId=@id";

                    cmd.Parameters.Add(new SqlParameter("@id", studentId));
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        assignedExercises.Add(new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language")),
                        });
                    }
                    reader.Close();
                }
            }
            return assignedExercises;
        }

        private List<Exercise> GetAllExercises()
        {
            List<Exercise> allExercises = new List<Exercise>();

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Language FROM Exercise";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        allExercises.Add(new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language")),
                        });
                    }
                    reader.Close();
                }
            }
            return allExercises;
        }

        private List<Cohort> GetAllCohorts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cohort";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Cohort> cohorts = new List<Cohort>();
                    while (reader.Read())
                    {
                        cohorts.Add(new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }

                    reader.Close();

                    return cohorts;
                }
            }
        }
    }
}
