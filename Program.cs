/*Project Description:
The Student Management System is a dynamic and comprehensive C# Windows (Or Web) Form
Application designed to streamline the management of student-related information within an
educational institution. This project encompasses modules that cover key aspects of student data,
faculty details, attendance tracking, marks management, and financial records. As you progress through
the modules, you'll gain practical experience in C# programming, database interactions using SQL Server,
and the development of user-friendly interfaces.*/

/* Module 2: Forms
 
1.Login Form:
• Create a login form that authenticates users against the faculty table.
• Implement secure password hashing for user authentication.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Data;

namespace StudentManagementSystem
{
    public partial class LoginForm : Form
    {
        private const string connectionString = "Your_Connection_String";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = HashPassword(txtPassword.Text);

            if (AuthenticateUser(username, password))
            {
                MessageBox.Show("Login successful!");
                // Open the main form or perform actions after successful login
                // Example: MainApplicationForm mainForm = new MainApplicationForm();
                // mainForm.Show();
                // this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password!");
            }
        }

        private bool AuthenticateUser(string username, string password)
        {
            bool isAuthenticated = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Faculty WHERE Username = @Username AND PasswordHash = @PasswordHash";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", password);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        isAuthenticated = true;
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return isAuthenticated;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}



/*2.Registration Form:
• Design a registration form for new faculty members.
• Allow new faculty members to register with a unique username and password.*/


namespace StudentManagementSystem
{
    public partial class RegistrationForm : Form
    {
        private const string connectionString = "Your_Connection_String"; // Replace with your actual connection string

        public RegistrationForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            string username = txtUsername.Text;
            string password = HashPassword(txtPassword.Text);

            if (IsUsernameAvailable(username))
            {
                if (RegisterFaculty(name, username, password))
                {
                    MessageBox.Show("Registration successful!");
                    // You can perform actions after successful registration, like redirecting to the login page
                    this.Close(); // Close the registration form after successful registration
                }
                else
                {
                    MessageBox.Show("Failed to register faculty. Please try again.");
                }
            }
            else
            {
                MessageBox.Show("Username already exists. Please choose a different username.");
            }
        }

        private bool IsUsernameAvailable(string username)
        {
            bool isAvailable = true;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Username FROM Faculty WHERE Username = @Username";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        isAvailable = false;
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return isAvailable;
        }

        private bool RegisterFaculty(string name, string username, string password)
        {
            bool isRegistered = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Faculty (Name, Username, PasswordHash) VALUES (@Name, @Username, @PasswordHash)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", password);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        isRegistered = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return isRegistered;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}


/*3.Student Form:
• Design a form for managing student information (CRUD operations).
• Implement functionalities to add, view, edit, and delete student records*/

namespace StudentManagementSystem
{
    public partial class StudentForm : Form
    {
        private const string connectionString = "Your_Connection_String"; // Replace with your actual connection string
        private SqlDataAdapter adapter;
        private DataTable dataTable;
        private SqlCommandBuilder commandBuilder;
        private SqlConnection connection;

        public StudentForm()
        {
            InitializeComponent();
            LoadStudents();
        }

        private void LoadStudents()
        {
            connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT * FROM Students", connection);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridViewStudents.DataSource = dataTable;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();
            newRow["Name"] = txtName.Text;
            newRow["RollNumber"] = txtRollNumber.Text;

            dataTable.Rows.Add(newRow);
            SaveChanges();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewStudents.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];
                row["Name"] = txtName.Text;
                row["RollNumber"] = txtRollNumber.Text;

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a student to edit.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewStudents.SelectedRows[0].Index;
                dataTable.Rows[selectedIndex].Delete();

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a student to delete.");
            }
        }

        private void SaveChanges()
        {
            try
            {
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message);
            }
        }

        private void dataGridViewStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStudents.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewStudents.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];

                txtName.Text = row["Name"].ToString();
                txtRollNumber.Text = row["RollNumber"].ToString();
            }
        }
    }
}


/*4.Marks Form:
• Create a form for managing student marks.
• Implement functionalities to add, view, edit, and delete student marks.*/

namespace StudentManagementSystem
{
    public partial class MarksForm : Form
    {
        private const string connectionString = "Your_Connection_String"; // Replace with your actual connection string
        private SqlDataAdapter adapter;
        private DataTable dataTable;
        private SqlCommandBuilder commandBuilder;
        private SqlConnection connection;

        public MarksForm()
        {
            InitializeComponent();
            LoadMarks();
        }

        private void LoadMarks()
        {
            connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT * FROM Marks", connection);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridViewMarks.DataSource = dataTable;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();
            newRow["StudentID"] = txtStudentID.Text;
            newRow["SubjectID"] = txtSubjectID.Text;
            newRow["MarksObtained"] = txtMarksObtained.Text;

            dataTable.Rows.Add(newRow);
            SaveChanges();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewMarks.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewMarks.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];
                row["StudentID"] = txtStudentID.Text;
                row["SubjectID"] = txtSubjectID.Text;
                row["MarksObtained"] = txtMarksObtained.Text;

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a mark record to edit.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewMarks.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewMarks.SelectedRows[0].Index;
                dataTable.Rows[selectedIndex].Delete();

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a mark record to delete.");
            }
        }

        private void SaveChanges()
        {
            try
            {
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message);
            }
        }

        private void dataGridViewMarks_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewMarks.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewMarks.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];

                txtStudentID.Text = row["StudentID"].ToString();
                txtSubjectID.Text = row["SubjectID"].ToString();
                txtMarksObtained.Text = row["MarksObtained"].ToString();
            }
        }
    }
}

/*5. Subjects Form:
• Design a form for managing subjects.
• Implement functionalities to add, view, edit, and delete subject records*/

namespace StudentManagementSystem
{
    public partial class SubjectsForm : Form
    {
        private const string connectionString = "Your_Connection_String"; // Replace with your actual connection string
        private SqlDataAdapter adapter;
        private DataTable dataTable;
        private SqlCommandBuilder commandBuilder;
        private SqlConnection connection;

        public SubjectsForm()
        {
            InitializeComponent();
            LoadSubjects();
        }

        private void LoadSubjects()
        {
            connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT * FROM Subjects", connection);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridViewSubjects.DataSource = dataTable;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();
            newRow["SubjectName"] = txtSubjectName.Text;

            dataTable.Rows.Add(newRow);
            SaveChanges();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewSubjects.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewSubjects.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];
                row["SubjectName"] = txtSubjectName.Text;

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a subject record to edit.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewSubjects.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewSubjects.SelectedRows[0].Index;
                dataTable.Rows[selectedIndex].Delete();

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a subject record to delete.");
            }
        }

        private void SaveChanges()
        {
            try
            {
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message);
            }
        }

        private void dataGridViewSubjects_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewSubjects.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewSubjects.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];

                txtSubjectName.Text = row["SubjectName"].ToString();
            }
        }
    }
}

/*6. Fees Form:
• Develop a form for managing student fees.
• Implement functionalities to add, view, edit, and delete fee records.*/

namespace StudentManagementSystem
{
    public partial class FeesForm : Form
    {
        private const string connectionString = "Your_Connection_String"; 
        private SqlDataAdapter adapter;
        private DataTable dataTable;
        private SqlCommandBuilder commandBuilder;
        private SqlConnection connection;

        public FeesForm()
        {
            InitializeComponent();
            LoadFees();
        }

        private void LoadFees()
        {
            connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT * FROM Fees", connection);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridViewFees.DataSource = dataTable;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();
            newRow["StudentID"] = txtStudentID.Text;
            newRow["Amount"] = txtAmount.Text;

            dataTable.Rows.Add(newRow);
            SaveChanges();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewFees.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewFees.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];
                row["StudentID"] = txtStudentID.Text;
                row["Amount"] = txtAmount.Text;

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a fee record to edit.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewFees.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewFees.SelectedRows[0].Index;
                dataTable.Rows[selectedIndex].Delete();

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a fee record to delete.");
            }
        }

        private void SaveChanges()
        {
            try
            {
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message);
            }
        }

        private void dataGridViewFees_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewFees.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewFees.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];

                txtStudentID.Text = row["StudentID"].ToString();
                txtAmount.Text = row["Amount"].ToString();
            }
        }
    }
}


/*7. Faculty Form:
• Create a form for managing faculty information.
• Implement functionalities to add, view, edit, and delete faculty records.*/

namespace StudentManagementSystem
{
    public partial class FacultyForm : Form
    {
        private const string connectionString = "Your_Connection_String"; 
        private SqlDataAdapter adapter;
        private DataTable dataTable;
        private SqlCommandBuilder commandBuilder;
        private SqlConnection connection;

        public FacultyForm()
        {
            InitializeComponent();
            LoadFaculty();
        }

        private void LoadFaculty()
        {
            connection = new SqlConnection(connectionString);
            adapter = new SqlDataAdapter("SELECT * FROM Faculty", connection);
            dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridViewFaculty.DataSource = dataTable;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();
            newRow["Name"] = txtName.Text;
            newRow["Department"] = txtDepartment.Text;

            dataTable.Rows.Add(newRow);
            SaveChanges();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewFaculty.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewFaculty.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];
                row["Name"] = txtName.Text;
                row["Department"] = txtDepartment.Text;

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a faculty record to edit.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewFaculty.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewFaculty.SelectedRows[0].Index;
                dataTable.Rows[selectedIndex].Delete();

                SaveChanges();
            }
            else
            {
                MessageBox.Show("Please select a faculty record to delete.");
            }
        }

        private void SaveChanges()
        {
            try
            {
                commandBuilder = new SqlCommandBuilder(adapter);
                adapter.Update(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message);
            }
        }

        private void dataGridViewFaculty_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewFaculty.SelectedRows.Count > 0)
            {
                int selectedIndex = dataGridViewFaculty.SelectedRows[0].Index;
                DataRow row = dataTable.Rows[selectedIndex];

                txtName.Text = row["Name"].ToString();
                txtDepartment.Text = row["Department"].ToString();
            }
        }
    }
}
