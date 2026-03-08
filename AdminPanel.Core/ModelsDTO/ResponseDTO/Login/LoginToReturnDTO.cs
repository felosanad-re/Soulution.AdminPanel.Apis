namespace AdminPanel.Core.ModelsDTO.ResponseDTO.Login
{
    public class LoginToReturnDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
