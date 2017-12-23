using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace VkApiTest.Utils
{
    public class VkApiUtils
    {
        private string wallMessage = "", idWallPost = "", commentWallMessage = "";
        private string photoPid = "", PhotoId = "";
        private Dictionary<string, string> response = new Dictionary<string, string>();
        private const char StartLetter = 'a', TrimChar = ']';
        private const int StartIndexLetter = 4, EndIndexLetter = 20, Success = 1, MatchGroupRegular = 1;
        private const string Response = "response";
        private const string RegularFindResponse = @":\[?(\{?.+)\}\]?";
        private const string TemplateRequestString = "https://api.vk.com/method/{0}?{1}&access_token={2}";

        private Dictionary<string, string> MethodApi(string methodName, string parameters, string token)
        {
            Dictionary<string, string> responseDictionary = new Dictionary<string, string>();
            var requestString = string.Format(TemplateRequestString, methodName, parameters, token);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestString);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var readResponse = reader.ReadToEnd();
                var responseString = GetMatchString(RegularFindResponse, readResponse).Trim(TrimChar);
                try
                {
                    responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
                }
                catch (JsonSerializationException)
                {
                    responseDictionary.Add(Response, responseString);
                }
                catch (JsonReaderException ex)
                {
                    throw new Exception(ex.Message + "\nResponse:" + responseString);
                }
                return responseDictionary;
            }
        }

        public string WallPost(string userId, string method, string token)
        {
            wallMessage = GenerateMesage();
            response = MethodApi(method, string.Format("owner_id={0}&message={1}", userId, wallMessage), token);
            idWallPost = response["post_id"];
            return wallMessage;
        }

        public string WallPostPhoto(string method, string userId, string token, string photoId)
        {
            wallMessage = GenerateMesage();
            response = MethodApi(method, string.Format("owner_id={0}&attachments={1}&post_id={2}&message={3}", 
                userId, photoId, idWallPost, wallMessage), token);
            return wallMessage;
        }

        public void WallEdit(string userId, string method, string token)
        {
            wallMessage = GenerateMesage();
            response = MethodApi(method, string.Format("owner_id={0}&message={1}&post_id={2}", userId, wallMessage, idWallPost), token);
        }

        public string CreateCommentWallPost(string userId, string method, string token)
        {
            commentWallMessage = GenerateMesage();
            response = MethodApi(method, string.Format("owner_id={0}&message={1}&post_id={2}", userId, commentWallMessage, idWallPost), token);
            return commentWallMessage;
        }

        public void DeleteWallPost(string userId, string method, string token)
        {
            response = MethodApi(method, string.Format("&owner_id={0}&post_id={1}", userId, idWallPost), token);
        }

        public string AddedPhotoWallPost(string userId, string method, string token)
        {
            response = MethodApi(method, string.Format("&group_id={0}", userId), token);
            return response["upload_url"];
        }

        public Dictionary<string, string> SaveWallPhoto(string userId, string method, string token, string server, string photo, string hash)
        {
            response = MethodApi(method, string.Format("&user_id={0}&group_id={0}&photo={1}&server={2}&hash={3}", userId, photo, server, hash), token);
            return response;
        }

        public string PostPhoto(string urlServer, string userId, string method, string token, string pathToPhoto)
        {
            string pathToPhotoFull = Environment.CurrentDirectory + pathToPhoto;
            var responseUploadFiles = UploadFilesToRemoteUrl(urlServer, pathToPhotoFull);
            var responseSavePhoto = SaveWallPhoto(userId, method, token, responseUploadFiles["server"],
                responseUploadFiles["photo"], responseUploadFiles["hash"]);
            var newWallPostMessage = WallPostPhoto("wall.edit", userId, token, responseSavePhoto["id"]);
            photoPid = responseSavePhoto["pid"];
            PhotoId = responseSavePhoto["id"];
            return newWallPostMessage;
        }

        public void DeletePhoto(string method, string userId, string token)
        {
            response = MethodApi(method, string.Format("user_id={0}&photo_id={1}", userId, photoPid), token);
        }

        public static Dictionary<string, string> UploadFilesToRemoteUrl(string url, string file, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            Stream memStream = new MemoryStream();
            var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            var endBoundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            if (formFields != null)
            {
                foreach (string key in formFields.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, formFields[key]);
                    byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }
            string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            var header = string.Format(headerTemplate, "photo", file);
            var headerbytes = Encoding.UTF8.GetBytes(header);
            memStream.Write(headerbytes, 0, headerbytes.Length);
            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[1024];
                var bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
            }
            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;
            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }
            using (var response = request.GetResponse())
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
                return result;
            }
        }

        public void DeletePhotoFromVk(string method, string userId, string token)
        {
            response = MethodApi(method, string.Format("user_id={0}&photo_id={1}", userId, photoPid), token);
        }


        public bool AssertLikeWallPost(string userId, string method, string type, string token)
        {
            response = MethodApi(method, string.Format("user_id{0}&owner_id={0}&type={1}&item_id={2}", userId, type, idWallPost), token);
            return Int32.Parse(response[Response]) == Success;
        }


        private static string GetMatchString(string patternStr, string text)
        {
            var result = "";
            foreach (Match match in Regex.Matches(text, patternStr, RegexOptions.IgnoreCase))
            {
                result = match.Groups[MatchGroupRegular].Value;
            }
            return result;
        }

        private string GenerateMesage()
        {
            Random random = new Random();
            int countLetter = 0;
            int randomValue = 0;
            char lettter = ' ';
            string message = "";
            countLetter = random.Next(StartIndexLetter, EndIndexLetter);
            for (int j = 0; j < countLetter; j++)
            {
                randomValue = random.Next(StartIndexLetter, EndIndexLetter);
                lettter = (char)(StartLetter + randomValue);
                message += lettter;
            }
            return message;
        }
    }
}
