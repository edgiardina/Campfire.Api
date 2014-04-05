using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.Storage.Streams;
using Campfire.Api.Models;
using Newtonsoft.Json;

namespace Campfire.Api
{
    public sealed class CampfireApi
    {
        public bool IsInitialized { get; private set; }

        private HttpWebRequest httpWebRequest;
        private string campfireUrl = "https://{0}.campfirenow.com/";
        private readonly string CAMPFIRE_CLIENT_ID;
        private readonly string CAMPFIRE_REDIRECT_URL;
        private readonly string CAMPFIRE_SUCCESS_URL;

        public User LoggedInUser { get; private set; }

        private string _accessToken { get; set; }

        public CampfireApi(string campfireClientId, string campfireRedirectUrl, string campfireSuccessUrl)
        {
            CAMPFIRE_CLIENT_ID = campfireClientId;
            CAMPFIRE_REDIRECT_URL = campfireRedirectUrl;
            CAMPFIRE_SUCCESS_URL = campfireSuccessUrl;

            httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(campfireUrl, "default"));
            IsInitialized = false;
        }

        public async void ExitRoom(int chatRoomId)
        {
            string path = string.Format("room/{0}/leave.json", chatRoomId);
            await GetResponseAsString(path, "POST");
        }

        public string GetEmojiUrl(string emojiName)
        {
            return string.Format(campfireUrl + "images/emoji/{0}.png", WebUtility.UrlEncode(emojiName.Replace(":", string.Empty)));
        }

        public void SignOut()
        {
            IsInitialized = false;
            
            _accessToken = null;
            LoggedInUser = null;
        }

        public async Task<Authorization> Initialize(string accessToken = null)
        {
            if (!String.IsNullOrEmpty(accessToken))
            {
                _accessToken = accessToken;
            }
            else
            {
               accessToken  = await CheckAndGetAccessToken();
            }

            var auth = await GetUserAuthorizationData();
            auth.AccessToken = accessToken;
            IsInitialized = true;

            return auth;
        }

        private async Task<string> GetAuthorizeCode()
        {
            WebAuthenticationResult war;

            var url =
                string.Format("https://launchpad.37signals.com/authorization/new?type=web_server&client_id={0}&redirect_uri={1}",
                CAMPFIRE_CLIENT_ID,
                WebUtility.UrlEncode(CAMPFIRE_REDIRECT_URL));

            var startUri = new Uri(url);
            var callbackUri = new Uri(CAMPFIRE_SUCCESS_URL);
            var t = startUri.Query;


            war = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None,
                startUri, callbackUri);


            switch (war.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    {
                        //TODO Instead look for  Token. 

                        // grab access_token and oauth_verifier
                        var response = war.ResponseData;

                        IDictionary<string, string> keyDictionary = new Dictionary<string, string>();
                        var qSplit = response.Split('?');
                        foreach (var kvp in qSplit[qSplit.Length - 1].Split('&'))
                        {
                            var kvpSplit = kvp.Split('=');
                            keyDictionary.Add(kvpSplit[0], kvpSplit[1]);
                        }

                        _accessToken = WebUtility.UrlDecode(keyDictionary["token"]);
                        return _accessToken;
                    }
                case WebAuthenticationStatus.UserCancel:
                    {
                        throw new SecurityAccessDeniedException("Authentication with Campfire not successful.");
                    }
                default:
                case WebAuthenticationStatus.ErrorHttp:
                    throw new SecurityAccessDeniedException("Authentication with Campfire not successful.");
                    break;
            }


        }

        private async Task<Authorization> GetUserAuthorizationData()
        {
            var url = "https://launchpad.37signals.com/authorization.json";
            string jsonString = "";

            using (var httpClient = new HttpClient())
            {
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Headers["Authorization"] = "Bearer " + _accessToken;

                var response = await httpWebRequest.GetResponseAsync();

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    jsonString = sr.ReadToEnd();
                }

                try
                {
                    var authorization = JsonConvert.DeserializeObject<Authorization>(jsonString);

                    //TODO: This could throw a null reference exception or the like. Capture and show dialog that user doesn't sign
                    //up for campfire
                    campfireUrl = authorization.Accounts.First(n => n.Product == "campfire").Href + "/";

                    //Store the user
                    LoggedInUser = await GetLoggedInUser();

                    return authorization;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private async Task<string> CheckAndGetAccessToken()
        {
            // If we don't have an access token, we will try to get one
            if (string.IsNullOrEmpty(_accessToken))
            {
                return await GetAuthorizeCode();
            }
            return _accessToken;
        }

        public async Task<List<Room>> GetRooms()
        {
            string text = await GetResponseAsString("rooms.json");

            var rooms = JsonConvert.DeserializeObject<RoomCollection>(text);
            return rooms.Rooms;
        }

        public async Task<Room> GetRoom(int roomId)
        {
            string text = await GetResponseAsString(string.Format("room/{0}.json", roomId));

            var room = JsonConvert.DeserializeObject<SingleRoom>(text);
            return room.Room;
        }

        public async Task<List<Message>> GetMessages(int chatRoomId, int sinceThisMessageId = 0)
        {
            Dictionary<int, User> userDict = new Dictionary<int, User>();

            string path = string.Format("room/{0}/recent.json", chatRoomId);

            if (sinceThisMessageId > 0)
                path += string.Format("?since_message_id={0}", sinceThisMessageId);

            string text = await GetResponseAsString(path);

            var messages = JsonConvert.DeserializeObject<MessageCollection>(text);

            //TODO: Cache user info, so we only poll the web service when we see an unknown user.
            var userList = messages.Messages.Select(n => n.UserId).Distinct();

            foreach (var i in userList.Where(i => i.HasValue))
            {
                var user = await GetUser(i.Value);
                userDict.Add(i.Value, user);
            }

            foreach (var j in messages.Messages)
            {
                if (j.UserId.HasValue)
                {
                    j.UserName = userDict[j.UserId.Value].Name;
                    j.AvatarUrl = userDict[j.UserId.Value].AvatarUrl;
                }

                if (j.Type == MessageType.UploadMessage)
                {
                    j.Upload = await GetUpload(j.Id, j.RoomId);
                }
            }

            return messages.Messages;
        }

        public async Task<User> GetUser(int userId)
        {
            string path = string.Format("users/{0}.json", userId);
            string text = await GetResponseAsString(path);

            var user = JsonConvert.DeserializeObject<SingleUser>(text);

            return user.User;
        }

        /// <summary>
        /// Get the current user who is authenticated to basecamp
        /// </summary>
        /// <remarks>This value is cached since it should not change within the application. Use LoggedInUser publically</remarks>
        /// <see cref="LoggedInUser" />
        /// <returns></returns>
        private async Task<User> GetLoggedInUser()
        {
            string path = "users/me.json";
            string text = await GetResponseAsString(path);

            var user = JsonConvert.DeserializeObject<SingleUser>(text);

            return user.User;
        }

        public async void EnterRoom(int chatRoomId)
        {
            string path = string.Format("room/{0}/join.json", chatRoomId);
            await GetResponseAsString(path, "POST");
        }

        public async void StarMessage(int messageId)
        {
            string path = string.Format("messages/{0}/star.json", messageId);
            await GetResponseAsString(path, "POST");
        }

        public async void UnStarMessage(int messageId)
        {
            string path = string.Format("messages/{0}/star.json", messageId);
            await GetResponseAsString(path, "DELETE");
        }

        public async void LockRoom(int roomId)
        {
            string path = string.Format("room/{0}/lock.json", roomId);
            await GetResponseAsString(path, "POST");
        }

        public async void UnlockRoom(int roomId)
        {
            string path = string.Format("room/{0}/unlock.json", roomId);
            await GetResponseAsString(path, "POST");
        }

        public async Task<Upload> UploadFileToRoom(int roomId, StorageFile file)
        {
            //POST /room/#{id}/uploads.xml
            string path = string.Format("room/{0}/uploads.json", roomId);
            string uploaduri = campfireUrl + path;
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);
            httpClient.DefaultRequestHeaders.Add("Content-Transfer-Encoding", "binary");

            IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);
            MultipartFormDataContent form = new MultipartFormDataContent("---------------------------XXX");

            var content = readStream.AsStream();
            var streamContent = new StreamContent(content);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"upload\"",
                FileName = "\"" + file.Name + "\""
            };
            form.Add(streamContent);

            HttpResponseMessage response = await httpClient.PostAsync(uploaduri, form);
            string responseText = await response.Content.ReadAsStringAsync();

            var upload = JsonConvert.DeserializeObject<Upload>(responseText);

            return upload;
        }

        public async Task<Message> PostMessage(int chatRoomId, string message, bool isSound = false)
        {
            //room/1/speak.json
            string path = string.Format("room/{0}/speak.json", chatRoomId);

            //TODO: this is shitty. Fix it.
            dynamic messageObj = new ExpandoObject();
            messageObj.message = new ExpandoObject();
            messageObj.message.body = message;
            
            if(isSound)
                messageObj.message.type = MessageType.SoundMessage.ToString();
            
            var serializedMessage = JsonConvert.SerializeObject(messageObj);

            var response = await GetResponseAsString(path, "POST", serializedMessage);

            //This is also kinda shitty
            var returnMessage = JsonConvert.DeserializeObject<SingleMessage>(response).Message;
            var user = GetUser(returnMessage.UserId);
            returnMessage.UserName = user.Name;
            return returnMessage;
        }
        
        public async Task<Upload> GetUpload(int uploadId, int roomId)
        {
            string path = string.Format("room/{0}/messages/{1}/upload.json", roomId, uploadId);
            string text = await GetResponseAsString(path);

            var upload = JsonConvert.DeserializeObject<SingleUpload>(text);

            return upload.Upload;
        }

        public async Task<byte[]> GetFile(string url)
        {
            return await GetResponseAsByteArray(url);
        }

        private async Task<string> GetResponseAsString(string path, string method = "GET", string body = null)
        {
            string text;
            var response = await GetResponse(path, method, body);

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            return text;
        }

        private async Task<byte[]> GetResponseAsByteArray(string path, string method = "GET", string body = null)
        {
            var response = await GetResponse(path, method, body);

            using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
            {
                Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                return lnByte;
            }
        }

        private async Task<WebResponse> GetResponse(string path, string method = "GET", string body = null)
        {
            byte[] data = null;
            if (body != null)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                data = encoding.GetBytes(body);
            }

            return await GetResponse(path, method, body: data);
        }

        private async Task<WebResponse> GetResponse(string path, string method = "GET", string contentType = "application/json", byte[] body = null)
        {
            httpWebRequest = (HttpWebRequest)WebRequest.Create(path.Contains(campfireUrl) ? path : campfireUrl + path);

            httpWebRequest.Method = method;
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers["Authorization"] = "Bearer " + _accessToken;
            
            if (body != null)
            {
                httpWebRequest.ContentType = contentType;
                Stream newStream = await httpWebRequest.GetRequestStreamAsync();
                newStream.Write(body, 0, body.Length);
            }

            try
            {
                var responseA = await httpWebRequest.GetResponseAsync();
                return responseA;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
