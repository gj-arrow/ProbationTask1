using demo.framework;
using demo.framework.Elements;
using demo.framework.forms;
using OpenQA.Selenium;

namespace VkApiTest.Forms
{
    public class LoginForm : BaseForm
    {
        private TextBox txbUserEmail = new TextBox(By.Id("index_email"), "User email or phone");
        private TextBox txbPassword = new TextBox(By.Id("index_pass"), "User password");
        private Button btnSubmitLogin = new Button(By.Id("index_login_button"), "Button Submit Login");

        public LoginForm()
            : base(By.Id("index_login"), "Vk.com Login Page")
        {
        }

        public void Login(string userEmailOrPhone, string password)
        {
            txbUserEmail.SetText(userEmailOrPhone);
            txbPassword.SetText(password);
            btnSubmitLogin.Click();
            Browser.WaitForPageToLoad();
        }
    }
}

