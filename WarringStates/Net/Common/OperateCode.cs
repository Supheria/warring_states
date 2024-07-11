namespace WarringStates.Net.Common;

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
