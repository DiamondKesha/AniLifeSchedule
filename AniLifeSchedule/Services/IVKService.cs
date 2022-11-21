using AniLifeSchedule.Models.VK.Group;
using AniLifeSchedule.Models.VK.SaveFile;
using AniLifeSchedule.Models.VK.UploadFile;
using AniLifeSchedule.Models.VK.WallPost;
using AniLifeSchedule.Models.Wrapper;

namespace AniLifeSchedule.Services
{
    public interface IVKService
    {
        /// <summary>
        /// Gets information about current authorized group by token
        /// </summary>
        /// <param name="token">Authorize token</param>
        /// <returns>List of GroupData object</returns>
        public Task<Result<List<GroupData>>> GetGroupInformation(string token);

        /// <summary>
        /// Create post in group
        /// </summary>
        /// <param name="token">Authorize token</param>
        /// <param name="text"></param>SaveFileOnServer
        /// <param name="attachments"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Task<Result<WallPost>> CreatePost(string token, string text, string attachments, DateTime? time = null);

        /// <summary>
        /// Gets server url for upload file to the server VK
        /// </summary>
        /// <param name="token">Authorize token</param>
        /// <returns>UploadResponse object</returns>
        public Task<Result<UploadFileUrl>> GetWallUploadServer(string token);

        /// <summary>
        /// Uplpad bytes of file to the server VK
        /// </summary>
        /// <param name="uploadUrl">Upload url. Can be gets from GetWallUploadServer() method. </param>
        /// <param name="fileBytes">File bytes</param>
        /// <param name="filename">Filename of file</param>
        /// <returns>UploadFileResponse object</returns>
        public Task<Result<UploadFileData>> UploadFileToServer(string uploadUrl, byte[] fileBytes, string filename);

        /// <summary>
        /// Saves file on server VK
        /// </summary>
        /// <param name="token">Authorize token</param>
        /// <param name="fileString">File string can be gets from UploadFileToServer() method. </param>
        /// <param name="title">Name of the file</param>
        /// <param name="tags">Tags</param>
        /// <returns>SaveFileResponse object</returns>
        public Task<Result<SaveFile>> SaveFileOnServer(string token, string fileString, string title, string tags);

        /// <summary>
        /// Delete document file from group
        /// </summary>
        /// <param name="token">Authorize token</param>
        /// <param name="docId">Document id</param>
        /// <returns>if 1 is success, if 0 is error</returns>
        public Task<Result<int>> DeleteDocs(string token, string docId);
    }
}
