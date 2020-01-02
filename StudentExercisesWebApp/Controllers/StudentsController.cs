using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesWebApp.Models;
using StudentExercisesWebApp.Models.ViewModels;





namespace StudentExercisesWebApp.Controllers
{
    public class StudentsController : Controller
    {

        private readonly IConfiguration _config;

        public StudentsController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: Students
        public ActionResult Index()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                     SELECT s.Id,
                     s.FirstName,
                    s.LastName,
                    s.SlackHandle,
                    s.CohortId
                    FROM Student s";
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Student> students = new List<Student>();
                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                        students.Add(student);
                    }

                    reader.Close();

                    return View(students);
                }
            }
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
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

                    Student student = new Student ();

                    if (reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))

                        };
                    }
                    reader.Close();

                 
                    return View(student);
                }
            }
        }

        //GET: Students/Create
        public ActionResult Create()
        {
            // Create a new instance of a CreateStudentViewModel
            // If we want to get all the cohorts, we need to use the constructor that's expecting a connection string. 
            // When we create this instance, the constructor will run and get all the cohorts.
            CreateStudentViewModel studentViewModel = new CreateStudentViewModel(_config.GetConnectionString("DefaultConnection"));

            // Once we've created it, we can pass it to the view
            return View(studentViewModel);
        }

        // POST: Students/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateStudentViewModel model)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student
                ( FirstName, LastName, SlackHandle, CohortId )
                VALUES
                ( @firstName, @lastName, @slackHandle, @cohortId )";
                    cmd.Parameters.Add(new SqlParameter("@firstName", model.student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", model.student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@slackHandle", model.student.SlackHandle));
                    cmd.Parameters.Add(new SqlParameter("@cohortId", model.student.CohortId));
                    cmd.ExecuteNonQuery();

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        // GET: Students/Edit/5
        public ActionResult Edit(int id)
        {
            // Create a new instance of a StudentEditViewModel
            // Pass it the studentId and a connection string to the database
            StudentEditViewModel viewModel = new StudentEditViewModel(id, _config.GetConnectionString("DefaultConnection"));
            
            // The view model's constructor will work its magic
            // Pass the new instance of the view model to the view

            return View(viewModel);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, StudentEditViewModel studentViewModel)
        {
            try
            {
                // First, update the student's information, including their cohortId

                // Now you have to deal with their exercises. You have a list of integers on your view model called SelectedExercises. These are the primary keys of the exercises the user selected from the multi-select.

                // But wait! Before you start creating new entries in our StudentExercises join table, what about the student's PREVIOUSLY assigned exercises? Let's say that Jane was assigned Chicken Monkey and Boy Bands, but then we went in and edited her and now we've assigned Kennel. How can we make sure that she's no longer assigned to Chicken Monkey and Boy Bands?

                // The easiest way to do this is to DELETE all of the old entries in the join table with this student's Id and start fresh. This is a little nuclear, but it gets the job done in this case. 

                // Once you've deleted all the entries in the join table with this student's id (i.e. wiped out all their previously assigned exercises), you go can go about assigning new ones

                // Loop over that list of integers INSERT a new entry into the join table for each one. Each entry will have the current exercise Id in the loop and the given studetn's Id. 

                // Ta da!! Your student is edited.

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(studentViewModel);
            }
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, firstName, lastName, slackHandle, cohortId
                        FROM Student
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Student Student = null;

                    if (reader.Read())
                    {
                        Student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("firstName")),
                            LastName = reader.GetString(reader.GetOrdinal("lastName")),
                            SlackHandle = reader.GetString(reader.GetOrdinal("slackHandle")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("cohortId"))

                        };
                    }
                    reader.Close();

                    return View(Student);
                }
            }
        }

        // POST: Students/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {

                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM StudentExercise WHERE studentId = @id
                        DELETE FROM Student WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                    }
                }
             
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}