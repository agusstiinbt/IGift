using IGift.Application.Responses.Users;

namespace IGift.Application.CQRS.Identity.Users
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
        /// <summary>
        /// Esto se debe obtener del token
        /// </summary>
        public string CurrentUserRole { get; set; }
    }
}
