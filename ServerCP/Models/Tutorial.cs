namespace CryptoPuzzles.Server.Models
{
    public class Tutorial
    {
        public int Id { get; set; }
        public int MethodId { get; set; }
        public string TheoryTitle { get; set; } = string.Empty;
        public string TheoryContent { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual EncryptionMethod Method { get; set; } = null!;
    }
}