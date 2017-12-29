using demo.framework;
using LibPuzzle;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkApi.Enumerables;
using VkApi.Forms;
using VkApi.Utils;

namespace VkApi.VkApiTest
{
    [TestClass]
    public class VkApiTest : BaseTest
    {
        private readonly string _token = RunConfigurator.GetValue("token");
        private readonly string _username = RunConfigurator.GetValue("username");
        private readonly string _password = RunConfigurator.GetValue("password");
        private readonly string _userId = RunConfigurator.GetValue("userId");
        private readonly string _photoOriginalName = RunConfigurator.GetValue("photoOriginalName");
        private readonly string _pathToFolderResources = RunConfigurator.GetValue("pathToFolderResources");
        private const int LenghtMessage = 15;

        [TestMethod]
        public void VkApiTesting()
        {
            Log.Step("Navigate to vk.com");
            var loginF = new LoginForm();

            Log.Step("Login");
            loginF.Login(_username, _password);

            Log.Step("Navigate to 'My Page'");
            var newsF = new NewsForm();
            newsF.ClickMyPageBtn();

            Log.Step("Using the API request, create an post with randomly generated text on the wall and get the record id from the response");
            var vkApiUtils = new VkApiUtils();
            var messagePost = RandomUtils.RandomString(LenghtMessage);
            var idPost = vkApiUtils.CreateWallPost(_userId, MethodEnum.WALL_POST.GetStringMapping(), _token, messagePost);

            var mainF = new MainForm();
            Log.Step("Not updating the page, check was an post on the wall with the right text from the right user");
            Assert.IsTrue(mainF.IsWallPostMessage( messagePost), "This post doesn't exist on the wall");
            Assert.IsTrue(mainF.IsWallPostUser(_userId, messagePost), "This user not sent this post to the wall");

            Log.Step("Edit the post through the API request - change the text and add (load) any picture.");
            var messageEditPost = RandomUtils.RandomString(LenghtMessage);
            var photoInfo = vkApiUtils.AddPhotoWithMessage(_userId, idPost, 
                MethodEnum.PHOTOS_SAVE_WALL_PHOTO.GetStringMapping(), _token, _pathToFolderResources + _photoOriginalName, messageEditPost);
 
            Log.Step(@"Without updating the page, make sure that the text of the
            message has changed and the uploaded image has been added(make sure that the pictures are the same)");
            Assert.IsTrue(mainF.IsWallPostMessage(messageEditPost), "This post didn't edited on the wall");
            var downloadedFileName = mainF.DownloadFile(_pathToFolderResources, photoInfo["photoId"]);
            var similarityImages = mainF.ComparePhoto(_pathToFolderResources, _photoOriginalName, downloadedFileName);
            Assert.IsTrue(similarityImages < IPuzzle.LowSimilarityThreshold, "Images don't match");

            Log.Step("Using the API request, add a comment to the post with random text");
            var messageComment = RandomUtils.RandomString(LenghtMessage);
            vkApiUtils.ExecuteMessageActionWall(_userId, idPost, MethodEnum.WALL_CREATE_COMMENT.GetStringMapping(),_token, messageComment);

            Log.Step("Not updating the page, it's necessary that a comment from the right user is added to the correct post");
            Assert.IsTrue(mainF.IsCommentWallPost(messageEditPost), "This post not present on the wall");
            Assert.IsTrue(mainF.IsCommentWallPostUser(_userId, messageComment), "This user not sent this post to the wall");
            Assert.IsTrue(mainF.IsCommentWallPostMesage(messageComment), "This comment not present on the wall");

            Log.Step("Through UI, add 'Like' for the record.");
            mainF.AddLikeWallPost(messageEditPost);

            Log.Step("Through the request to the API, make sure that the 'Like' was sent from the right user");
            Assert.IsTrue(vkApiUtils.IsUserLikePost(_userId, idPost, 
                MethodEnum.LIKES_ISLIKED.GetStringMapping(), "post", _token), "No like from this user");

            Log.Step("Via the API request, delete the created record");
            vkApiUtils.DeleteWallPost(_userId, idPost, MethodEnum.WALL_DELETE.GetStringMapping(), _token);

            Log.Step("Not updating the page, it's necessary to make sure that the entry is deleted");
            Assert.IsTrue(mainF.IsDeletedWallPost(), "Post wasn't deleted from the wall");
            vkApiUtils.DeletePhotoFromSite(MethodEnum.PHOTOS_DELETE.GetStringMapping(), _userId, photoInfo["photoPid"], _token);
            mainF.DeletePhotoDownloadedFromVk(_pathToFolderResources, downloadedFileName);
        }
    }
}
