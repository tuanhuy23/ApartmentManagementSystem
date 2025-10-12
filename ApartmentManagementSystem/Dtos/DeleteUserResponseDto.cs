namespace ApartmentManagementSystem.Dtos
{
    public class DeleteUserResponseDto
    {
        public IEnumerable<string> UserNamesDeleteSuccess { get; set; }
         public IEnumerable<string> UserNamesDeleteError { get; set; }
    }
}