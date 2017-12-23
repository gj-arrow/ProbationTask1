using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using demo.framework;
using demo.framework.Elements;
using demo.framework.forms;
using OpenQA.Selenium;
using LibPuzzle;

namespace VkApiTest.Forms
{
    public class MainForm : BaseForm
    {
        private TextBox txbWallPost;
        private TextBox txbUrlPhoto;
        private TextBox txbWallPostUserId;
        private TextBox txbWallPostCommentUserId;
        private TextBox txbCommentWall;
        private TextBox txbWallPostComment;
        private TextBox txbPageBlock = new TextBox(By.XPath("//div[@id='profile_wall']/div[contains(@class,('page_block'))]"), "Page Block Wall" );
        private Button btnLikeWalPost;
        private string WallMessage = "", idWallPost = "", CommentWallMessage ="", namePhotoDownloaded = "";
        private string wallPostLocator = "", photoPid = "", PhotoId = "";
        private char startLetter = 'a';
        private const string TemplateWallPostLocator = "//div[contains(@class,'wall_text')]//div[contains(text(),'{0}')]";
        private const string RegularFindUrlPhoto = "{0}.+url\\((.+)\\)";
        private const string SeparatorPathToFolder = "\\";

        public MainForm()
            : base(By.XPath("//div[@id='narrow_column']/div[contains(@class,'page_photo')]"), "Vk.com Main Page")
        {
        }

        public void AddLikeWallPost()
        {
            btnLikeWalPost = new Button(By.XPath(wallPostLocator + "/../../..//a[contains(@class,'post_like')]"), "Button 'Like' on the wall post");
            if (btnLikeWalPost.IsDisplayed())
            {
                btnLikeWalPost.Click();
            }
        }

        public bool AssertWallPostUser(string userId)
        {
            txbWallPostUserId = new TextBox(By.XPath(string.Format(wallPostLocator
                + "/../../../../..//div[contains(@class, 'post_header')]/a[contains(@href,'{0}')]", userId)), "User id sent post to the wall");
            txbPageBlock.ScrollToElement();
            return txbWallPost.IsPresent();
        }

        public bool AssertWallPostMessage(string userId, string wallPostMessage)
        {
            wallPostLocator = string.Format(TemplateWallPostLocator, wallPostMessage);
            BaseElement.WaitForElementPresent(By.XPath(wallPostLocator), "Post present on the wall");
            txbWallPost = new TextBox(By.XPath(wallPostLocator), "Post on the wall");
            return txbWallPost.IsPresent();
        }

        public bool AssertDeleteWallPost(string userId)
        {
            return !txbWallPostUserId.IsPresent();
        }

        public bool AssertCommentWallPost(string userId, string wallPostMessage)
        {
            txbWallPostComment = new TextBox(By.XPath(string.Format(TemplateWallPostLocator,
                wallPostMessage)), "Post on the wall");
            return txbWallPostComment.IsPresent();
        }

        public bool AssertCommentWallPostUser(string userId, string commentWallMessage)
        {
            var commentWallPostLocator = string.Format("//div[contains(@class, 'reply_content')]//div[contains(text(),'{0}')]",
                commentWallMessage);
            BaseElement.WaitForElementPresent(By.XPath(commentWallPostLocator), "Post on the wall");
            txbWallPostCommentUserId = new TextBox(By.XPath(string.Format(commentWallPostLocator
                  + "/../../../div[contains(@class, 'reply_author')]/a[contains(@href,'sancho_96')]", userId)), "User id sent post to the wall");
            return txbWallPostCommentUserId.IsPresent();
        }

        public bool AssertCommentWallPostMesage(string userId, string commentWallMessage)
        {
            var commentWallPostLocator = string.Format("//div[contains(@class, 'reply_content')]//div[contains(text(),'{0}')]",
                commentWallMessage);
            BaseElement.WaitForElementPresent(By.XPath(commentWallPostLocator), "Post on the wall");
            txbCommentWall = new TextBox(By.XPath(commentWallPostLocator), "Comment on the wall post");
            return txbCommentWall.IsPresent();
        }

        public double AssertPhotoCompare(string pathToFolderDownload, string originalPhoto, string namePhotoDownloadedFromVk)
        {
            var context = new PuzzleContext();
            var imageOriginal = context.FromPath(Environment.CurrentDirectory + pathToFolderDownload + originalPhoto);
            var imageDownloadedToVk = context.FromPath(Environment.CurrentDirectory + pathToFolderDownload + SeparatorPathToFolder + namePhotoDownloadedFromVk);
            var similarity = imageOriginal.GetDistanceFrom(imageDownloadedToVk);
            return similarity;
        }

        public string TakeUrlPhoto(string photoId)
        {
            string urlPhoto = "";
            var pageSource = Browser.GetDriver().PageSource;
            foreach (Match match in Regex.Matches(pageSource, string.Format(RegularFindUrlPhoto, photoId), RegexOptions.IgnoreCase))
            {
                urlPhoto = match.Groups[1].Value;
            }

            return urlPhoto;
        }

        public string DownloadFile(string pathToFolderDownload, string photoName)
        {
            string fullPatgToFile = Environment.CurrentDirectory + pathToFolderDownload;
            string url = TakeUrlPhoto(PhotoId);
            namePhotoDownloaded = GetFilename(url);
            using (var client = new WebClient())
            {
                client.DownloadFile(url, fullPatgToFile + SeparatorPathToFolder + namePhotoDownloaded);
            }
            return namePhotoDownloaded;
        }

        private string GetFilename(string hreflink)
        {
            Uri uri = new Uri(hreflink);
            string filename = Path.GetFileName(uri.LocalPath);
            return filename;
        }

        public void DeletePhotoDownloadedFromVk(string pathToFolderDownload, string photoName)
        {
            string fullPatgToFile = Environment.CurrentDirectory + pathToFolderDownload;
            File.Delete(fullPatgToFile + SeparatorPathToFolder + namePhotoDownloaded);
        }
    }
}