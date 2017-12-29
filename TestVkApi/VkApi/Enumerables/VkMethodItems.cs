using System.ComponentModel;

public enum MethodEnum
{
    [Description("wall.post")]
    WALL_POST,
    [Description("wall.edit")]
    WALL_EDIT,
    [Description("photos.getWallUploadServer")]
    PHOTOS_GET_WALL_UPLOAD_SERVER,
    [Description("photos.saveWallPhoto")]
    PHOTOS_SAVE_WALL_PHOTO,
    [Description("wall.createComment")]
    WALL_CREATE_COMMENT,
    [Description("likes.isLiked")]
    LIKES_ISLIKED,
    [Description("wall.delete")]
    WALL_DELETE,
    [Description("photos.delete")]
    PHOTOS_DELETE
}
