using demo.framework.Elements;
using demo.framework.forms;
using OpenQA.Selenium;

namespace VkApi.Forms
{
    public class LoginForm : BaseForm
    {
        private readonly TextBox _txbUserEmail = new TextBox(By.Id("index_email"), "User email or phone");
        private readonly TextBox _txbPassword = new TextBox(By.Id("index_pass"), "User password");
        private readonly Button _btnSubmitLogin = new Button(By.Id("index_login_button"), "Button Submit Login");

        public LoginForm()
            : base(By.Id("index_login"), "Login Page")
        {
        }

        public void Login(string userEmailOrPhone, string password)
        {
            _txbUserEmail.SetText(userEmailOrPhone);
            _txbPassword.SetText(password);
            _btnSubmitLogin.ClickAndWait();
        }
    }
}

