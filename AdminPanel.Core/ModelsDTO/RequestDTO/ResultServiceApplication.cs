using AdminPanel.Core.Entities;

namespace AdminPanel.Core.ModelsDto.RequestDTO
{
    public class ResultServiceApplication<T>
    {
        public bool Succeed { get; set; }
        public string? Message { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public T? Data { get; set; }

        // Static Method For Succeeded
        public static ResultServiceApplication<T> Success(T data, string message)
        {
            return new ResultServiceApplication<T>
            {
                Succeed = true,
                Message = message,
                Data = data
            };
        }

        // Static Method For Failed

        public static ResultServiceApplication<T> Fail(string message)
        {
            return new ResultServiceApplication<T>
            {
                Succeed = false,
                Message = message,
            };
        }
    }
}
