using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace CRUD
{
    public partial class MainWindow : Page
    {
        // Connection string for the database
        private string connectionString = "Data Source=(localdb)\\W_U_R;Initial Catalog=WUR;Integrated Security=True";
        private SqlConnection con = null;

        // Constructor for the MainWindow class
        public MainWindow()
        {
            InitializeComponent();
            SetConnection();
        }

        // Method to update the data grid with employee information
        private void updateDataGrid()
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT EMPLOYEE_ID, LAST_NAME, JOB_ID, HIRE_DATE, EMAIL FROM EMPLOYEES ORDER BY HIRE_DATE DESC";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            myDataGrid.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        // Method to set up the database connection
        private void SetConnection()
        {
            con = new SqlConnection(connectionString);
            try
            {
                con.Open();
            }
            catch (Exception exp)
            {
                MessageBox.Show("Error connecting to the database: " + exp.Message);
            }
        }
        // Event handler for the page being loaded
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            updateDataGrid();
            LoadDepartmentsComboBox();
        }
        // Method to load departments into the combo box
        private void LoadDepartmentsComboBox()
        {
            DataAccess dataAccess = new DataAccess(connectionString);

            List<Department> departments = dataAccess.GetDepartmentsFromDatabase();

            departmentComboBox.ItemsSource = departments;
            departmentComboBox.DisplayMemberPath = "DepartmentName";
            departmentComboBox.SelectedValuePath = "DepartmentID";
        }

        // Class representing a department
        public class Department
        {
            public int DepartmentID { get; set; }
            public string DepartmentName { get; set; }
            // Add more properties if needed
        }
        // Class for data access operations
        public class DataAccess
        {
            private string connectionString;

            public DataAccess(string connectionString)
            {
                this.connectionString = connectionString;
            }

            public List<Department> GetDepartmentsFromDatabase()
            {
                List<Department> departments = new List<Department>();

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand cmd = new SqlCommand("SELECT DEPARTMENT_ID, DEPARTMENT_NAME FROM DEPARTMENTS", connection))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Department department = new Department
                                {
                                    DepartmentID = reader.GetInt32(0),
                                    DepartmentName = reader.GetString(1),
                                    // Add more properties if needed
                                };

                                departments.Add(department);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    MessageBox.Show("Error retrieving departments: " + ex.Message);
                }

                return departments;
            }
        }

        // Event handler for the page being unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            con.Close();
        }
     
        private void AUDEmployee(string sql_stmt, int state)
        {
            // Handle Add, Update, and Delete operations for departments
            string msg = "";
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sql_stmt;
                cmd.CommandType = CommandType.Text;

                switch (state)
                {
                    case 0:
                        msg = "Row Inserted Successfully!";
                        cmd.Parameters.Add("@EMPLOYEE_ID", SqlDbType.Int).Value = int.Parse(employee_id_txtbx.Text);
                        cmd.Parameters.Add("@LAST_NAME", SqlDbType.VarChar, 25).Value = last_name_txtbx.Text;
                        cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar, 25).Value = email_txtbx.Text;
                        cmd.Parameters.Add("@HIRE_DATE", SqlDbType.Date).Value = hire_date_picker.SelectedDate;
                        cmd.Parameters.Add("@JOB_ID", SqlDbType.VarChar, 10).Value = job_id_txtbx.Text;
                        break;

                    case 1:
                        msg = "Row Updated Successfully!";
                        cmd.Parameters.Add("@LAST_NAME", SqlDbType.VarChar, 25).Value = last_name_txtbx.Text;
                        cmd.Parameters.Add("@EMAIL", SqlDbType.VarChar, 25).Value = email_txtbx.Text;
                        cmd.Parameters.Add("@HIRE_DATE", SqlDbType.Date).Value = hire_date_picker.SelectedDate;
                        cmd.Parameters.Add("@JOB_ID", SqlDbType.VarChar, 10).Value = job_id_txtbx.Text;
                        cmd.Parameters.Add("@EMPLOYEE_ID", SqlDbType.Int).Value = int.Parse(employee_id_txtbx.Text);
                        break;

                    case 2:
                        msg = "Row Deleted Successfully!";
                        cmd.Parameters.Add("@EMPLOYEE_ID", SqlDbType.Int).Value = int.Parse(employee_id_txtbx.Text);
                        break;
                }

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        MessageBox.Show(msg);
                        updateDataGrid();
                    }
                }
                catch (Exception expe)
                {
                    MessageBox.Show("Error: " + expe.Message);
                }
            }
        }

        private void Reset_btn_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        private void resetAll()
        {
            // Reset department states
            employee_id_txtbx.Text = "";
            email_txtbx.Text = "";
            last_name_txtbx.Text = "";
            job_id_txtbx.Text = "";
            hire_date_picker.SelectedDate = null;

            add_btn.IsEnabled = true;
            update_btn.IsEnabled = false;
            delete_btn.IsEnabled = false;
        }
        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            // Handle the delete department button click event
            String sql = "DELETE FROM EMPLOYEES WHERE EMPLOYEE_ID = @EMPLOYEE_ID";
            this.AUDEmployee(sql, 2);
            resetAll();
        }

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Implementation for DataGrid selection changed
            DataGrid dg = sender as DataGrid;

            if (dg.SelectedItem != null)
            {
                DataRowView dr = dg.SelectedItem as DataRowView;

                if (dr != null)
                {
                    employee_id_txtbx.Text = dr["EMPLOYEE_ID"].ToString();
                    last_name_txtbx.Text = dr["LAST_NAME"].ToString();
                    job_id_txtbx.Text = dr["JOB_ID"].ToString();
                    email_txtbx.Text = dr["EMAIL"].ToString();
                    hire_date_picker.SelectedDate = DateTime.Parse(dr["HIRE_DATE"].ToString());

                    add_btn.IsEnabled = false;
                    update_btn.IsEnabled = true;
                    delete_btn.IsEnabled = true;
                }
            }
            else
            {
                resetAll();
            }
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            // Implementation for add button click
            String sql = "INSERT INTO EMPLOYEES(EMPLOYEE_ID, LAST_NAME, EMAIL, HIRE_DATE, JOB_ID) " +
                 "VALUES(@EMPLOYEE_ID, @LAST_NAME, @EMAIL, @HIRE_DATE, @JOB_ID)";
            this.AUDEmployee(sql, 0);
            resetAll();
        }

        private void update_btn_Click(object sender, RoutedEventArgs e)
        {
            // Handle the update department button click event
            // Implementation for update button click
            String sql = "UPDATE EMPLOYEES SET LAST_NAME = @LAST_NAME, " +
                 "EMAIL = @EMAIL, HIRE_DATE = @HIRE_DATE, JOB_ID = @JOB_ID " +
                 "WHERE EMPLOYEE_ID = @EMPLOYEE_ID";
            this.AUDEmployee(sql, 1);
            resetAll();
        }

        private void reset_btn_Click(object sender, RoutedEventArgs e)
        {
            // Implementation for reset button click
            resetAll();
        }
    }
}
