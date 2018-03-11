using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmailAttach
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        string loginUser = "admin";
        string loginPwd = "admin";

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtUserName.Text == loginUser && txtUserPwd.Password == loginUser)
                {
                    NavigationService.Navigate(new Uri("EmailDetails.xaml", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    MessageBox.Show("Invalid credentials.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
