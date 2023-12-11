// ... (Previous using statements remain unchanged)

using System.Data.SqlClient;
using System.Data;
using System.Windows.Controls;
using System.Windows;
using System;


namespace CRUD
{
    public partial class DepartmentsWindow : Page
    {
        SqlConnection con = null;

        public DepartmentsWindow()
        {
            this.setConnection();
            InitializeComponent();
            updateDepartmentDataGrid();
        }
        private void setConnection()
        {
            // Set up the database connection when the page is loaded
            // Change the connection string for SQL Server
            string connectionString = "Data Source=(localdb)\\W_U_R;Initial Catalog=WUR;Integrated Security=True";
            con = new SqlConnection(connectionString);
            try
            {
                con.Open();
            }
            catch (Exception exp)
            {
                // Display an error message if there's an issue connecting to the database
                MessageBox.Show("Error connecting to the database: " + exp.Message);
            }
        }
        
        private void resetDepartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            // Your implementation for resetDepartmentBtn_Click
            // Reset UI controls
            department_id_txtbx.Text = "";
            department_name_txtbx.Text = "";

            // Enable the add button and disable update and delete buttons
            addDepartmentBtn.IsEnabled = true;
            updateDepartmentBtn.IsEnabled = false;
            deleteDepartmentBtn.IsEnabled = false;
        }


        private void updateDepartmentDataGrid()
        {
            // Fetch department data from the database and update the data grid
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT DEPARTMENT_ID, DEPARTMENT_NAME FROM DEPARTMENTS";
            cmd.CommandType = CommandType.Text;

            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            departmentDataGrid.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void addDepartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            // Handle the add department button click event
            String sql = "INSERT INTO DEPARTMENTS(DEPARTMENT_ID, DEPARTMENT_NAME) " +
                         "VALUES(@DEPARTMENT_ID, @DEPARTMENT_NAME)";
            this.AUDDepartment(sql, 0);
            resetDepartmentControls();
        }

        private void updateDepartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            // Handle the update department button click event
            String sql = "UPDATE DEPARTMENTS SET DEPARTMENT_NAME = @DEPARTMENT_NAME " +
                         "WHERE DEPARTMENT_ID = @DEPARTMENT_ID";
            this.AUDDepartment(sql, 1);
            resetDepartmentControls();
        }

        private void deleteDepartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            // Handle the delete department button click event
            String sql = "DELETE FROM DEPARTMENTS WHERE DEPARTMENT_ID = @DEPARTMENT_ID";
            this.AUDDepartment(sql, 2);
            resetDepartmentControls();
        }

        private void departmentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle the department data grid selection changed event
            DataGrid dg = sender as DataGrid;

            if (dg.SelectedItem != null)
            {
                DataRowView dr = dg.SelectedItem as DataRowView;

                if (dr != null)
                {
                    department_id_txtbx.Text = dr["DEPARTMENT_ID"].ToString();
                    department_name_txtbx.Text = dr["DEPARTMENT_NAME"].ToString();

                    updateDepartmentBtn.IsEnabled = true;
                    deleteDepartmentBtn.IsEnabled = true;
                }
            }
            else
            {
                resetDepartmentControls();
            }
        }

        private void resetDepartmentControls()
        {
            // Reset department controls and button states
            department_id_txtbx.Text = "";
            department_name_txtbx.Text = "";

            updateDepartmentBtn.IsEnabled = false;
            deleteDepartmentBtn.IsEnabled = false;
        }

        private void AUDDepartment(String sql_stmt, int state)
        {
            // Handle Add, Update, and Delete operations for departments
            String msg = "";
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;

            try
            {
                switch (state)
                {
                    case 0:
                        msg = "Department Added Successfully!";
                        cmd.Parameters.Add("@DEPARTMENT_ID", SqlDbType.Int).Value = string.IsNullOrEmpty(department_id_txtbx.Text) ? DBNull.Value : (object)Int32.Parse(department_id_txtbx.Text);
                        cmd.Parameters.Add("@DEPARTMENT_NAME", SqlDbType.VarChar, 255).Value = department_name_txtbx.Text;
                        break;

                    case 1:
                        msg = "Department Updated Successfully!";
                        cmd.Parameters.Add("@DEPARTMENT_ID", SqlDbType.Int).Value = string.IsNullOrEmpty(department_id_txtbx.Text) ? DBNull.Value : (object)Int32.Parse(department_id_txtbx.Text);
                        cmd.Parameters.Add("@DEPARTMENT_NAME", SqlDbType.VarChar, 255).Value = department_name_txtbx.Text;
                        break;

                    case 2:
                        msg = "Department Deleted Successfully!";
                        cmd.Parameters.Add("@DEPARTMENT_ID", SqlDbType.Int).Value = string.IsNullOrEmpty(department_id_txtbx.Text) ? DBNull.Value : (object)Int32.Parse(department_id_txtbx.Text);
                        break;

                       
                }

                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                    updateDepartmentDataGrid();
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values.");
            }
            catch (Exception expe)
            {
                MessageBox.Show("Error: " + expe.Message);
            }
        }

    }
}
