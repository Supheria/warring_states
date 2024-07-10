namespace WarringStates.Net;

public enum OperateCode : byte
{
     None,
     Message,
     UpdateUserList,
     UploadRequest,
     UploadContinue,
     UploadFinish,
     DownloadRequest,
     DownloadContinue,
     DownloadFinish,
}
