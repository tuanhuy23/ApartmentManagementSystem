namespace ApartmentManagementSystem.Dtos
{
    public class DeleteUserResponseDto
    {
        public List<string> UserIdsDeleteSuccess { get; set; }
        public List<string> UserIdsDeleteError { get; set; }
    }
}