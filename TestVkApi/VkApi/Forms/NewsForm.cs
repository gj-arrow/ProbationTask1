using demo.framework.Elements;
using demo.framework.forms;
using OpenQA.Selenium;

namespace VkApi.Forms
{
    public class NewsForm : BaseForm
    {
        private readonly Button _btnMyPage = new Button(By.XPath("//li[@id='l_pr']//span[contains(text(),'Моя Страница')]"),"My page button");

        public NewsForm()
            : base(By.Id("main_feed"), "News Page")
        {
        }

        public void ClickMyPageBtn()
        {
            _btnMyPage.ClickAndWait();
        }
    }
}
