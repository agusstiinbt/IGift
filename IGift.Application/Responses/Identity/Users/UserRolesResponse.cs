namespace IGift.Application.Responses.Users
{
    /// <summary>
    /// Esta clase contiene una lista de UserRoleModel
    /// </summary>
    public class UserRolesResponse
    {
        public List<UserRoleModel> UserRoles { get; set; } = new();
    }
    /// <summary>
    /// Esta clase se usa para visualizar mejor un rol
    /// </summary>
    public class UserRoleModel
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        /// <summary>
        /// Es probable que un usuario pueda elegir el rol en el momento de estar ejecutándose la aplicación
        /// </summary>
        public bool Selected { get; set; }
    }
}
