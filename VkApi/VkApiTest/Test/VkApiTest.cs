using System.Diagnostics;
using demo.framework;
using LibPuzzle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkApiTest.Forms;
using VkApiTest.Utils;

namespace VkApiTest.Test
{
    [TestClass]
    public class VkApiTest : BaseTest
    {
        private readonly string token = RunConfigurator.GetValue("token");
        private readonly string username = RunConfigurator.GetValue("username");
        private readonly string password = RunConfigurator.GetValue("password");
        private readonly string userId = RunConfigurator.GetValue("userId");
        private readonly string photoOriginalName = RunConfigurator.GetValue("photoOriginalName");
        private readonly string pathToFolderResources = RunConfigurator.GetValue("pathToFolderResources");

        [TestInitialize]
        public void SetUp()
        {
            Browser.GetInstance();
            Browser.GetDriver().Navigate().GoToUrl(Configuration.GetBaseUrl());
        }

        [TestCleanup]
        public void TearDown()
        {
            var processes = Process.GetProcessesByName(Configuration.GetBrowser());
            foreach (var process in processes)
            {
                process.Kill();
            }
            Browser.GetDriver().Quit();
        }

        [TestMethod]
        public void VkApiTesting()
        {
            Log.Step("Navigate to vk.com");
            LoginForm loginF = new LoginForm();

            Log.Step("Login in vk");
            loginF.Login(username, password);

            Log.Step("Click to button 'My Page'");
            NewsForm newsF = new NewsForm();
            newsF.NavigateToMyPage();

            Log.Step("Create post on the wall");
            VkApiUtils vkApiUtils = new VkApiUtils();
            var wallPostMessage = vkApiUtils.WallPost(userId, "wall.post", token);

            MainForm mainF = new MainForm();
            Log.Step("Check message and user, whose sent post on the wall");
            Assert.IsTrue(mainF.AssertWallPostMessage(userId, wallPostMessage), "This post doesn't exist on the wall");
            Assert.IsTrue(mainF.AssertWallPostUser(userId), "This user not sent this post to the wall");

            Log.Step("Edit message and add photo to the post");
            var urlServer = vkApiUtils.AddedPhotoWallPost(userId, "photos.getWallUploadServer", token);
            var newWallPostMessage = vkApiUtils.PostPhoto(urlServer, userId, "photos.saveWallPhoto", token, pathToFolderResources + photoOriginalName);
 
            Log.Step("Check edited message post and user on the wall and photo");
            Assert.IsTrue(mainF.AssertWallPostMessage(userId, newWallPostMessage), "This post didn't edited on the wall");
            var downloadedFileName = mainF.DownloadFile(pathToFolderResources, photoOriginalName);
            var similarityImages = mainF.AssertPhotoCompare(pathToFolderResources, photoOriginalName, downloadedFileName);
            Assert.IsTrue(similarityImages < IPuzzle.LowSimilarityThreshold, "Images don't match");

            Log.Step("Create comment on this post on the wall");
            var commentWallMessage = vkApiUtils.CreateCommentWallPost(userId, "wall.createComment", token);

            Log.Step("Check comment,post and user");
            Assert.IsTrue(mainF.AssertCommentWallPost(userId, newWallPostMessage), "This post not present on the wall");
            Assert.IsTrue(mainF.AssertCommentWallPostUser(userId, commentWallMessage), "This user not sent this post to the wall");
            Assert.IsTrue(mainF.AssertCommentWallPostMesage(userId, commentWallMessage), "This comment not present on the wall");

            Log.Step("Add 'Like' post on the wall");
            mainF.AddLikeWallPost();

            Log.Step("Check 'Like' post on the wall");
            Assert.IsTrue(vkApiUtils.AssertLikeWallPost(userId, "likes.isLiked", "post", token), "No like from this user");

            Log.Step("Delete post on the wall");
            vkApiUtils.DeleteWallPost(userId, "wall.delete", token);

            Log.Step("Check that deleted post disappear from the wall");
            Assert.IsTrue(mainF.AssertDeleteWallPost(userId), "Post wasn't deleted from the wall");
            vkApiUtils.DeletePhotoFromVk("photos.delete", userId, token);
            mainF.DeletePhotoDownloadedFromVk(pathToFolderResources, downloadedFileName);
        }
    }
}
