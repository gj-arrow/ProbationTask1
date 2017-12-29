using System.ComponentModel;

public enum ApiParametersEnum
{
    [Description("owner_id=")]
    OWNER_ID,
    [Description("message=")]
    MESSAGE,
    [Description("attachments=")]
    ATTACHMENTS,
    [Description("post_id=")]
    POST_ID,
    [Description("group_id=")]
    GROUP_ID,
    [Description("user_id=")]
    USER_ID,
    [Description("photo=")]
    PHOTO,
    [Description("server=")]
    SERVER,
    [Description("hash=")]
    HASH,
    [Description("type=")]
    TYPE,
    [Description("item_id=")]
    ITEM_ID,
    [Description("access_token=")]
    ACCESS_TOKEN,
    [Description("photo_id=")]
    PHOTO_ID
}