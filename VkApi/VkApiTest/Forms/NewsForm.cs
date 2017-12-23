using demo.framework;
using demo.framework.Elements;
using demo.framework.forms;
using OpenQA.Selenium;

namespace VkApiTest.Forms
{
    public class NewsForm : BaseForm
    {
        private Button btnMyPage = new Button(By.XPath("//li[@id='l_pr']//span[contains(text(),'Моя Страница')]"),"My page button VK");

        public NewsForm()
            : base(By.Id("main_feed"), "Vk.com News Page")
        {
        }

        public void NavigateToMyPage()
        {
            btnMyPage.Click();
            Browser.WaitForPageToLoad();
        }
    }
}
