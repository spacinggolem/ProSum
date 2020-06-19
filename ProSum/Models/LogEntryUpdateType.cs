namespace ProSum.Models
{
    public enum LogEntryUpdateType
    {
        CREATED_PROJECT,
        CREATED_USER,
        CREATED_CLIENT,

        UPDATED_PROJECT,
        UPDATED_CLIENT,
        UPDATED_USER,
        UPDATED_USER_PERMISSIONS,

        DELETED_PROJECT,
        DELETED_USER,
        DELETED_CLIENT,

        STATUS_UPDATE,
        ADD_EMPLOYEE,
    }
}
