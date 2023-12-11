using System.Windows;

namespace CRUD
{
    public partial class RootWindow : Window
    {
        public RootWindow()
        {
            InitializeComponent();

            // Set the initial content for the frames
            employeeFrame.NavigationService.Navigate(new MainWindow());
            departmentFrame.NavigationService.Navigate(new DepartmentsWindow());
        }

    }
}
