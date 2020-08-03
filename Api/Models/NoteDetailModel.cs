namespace MyWineDb.Api.Models
{
    public class NoteDetailModel
    {
        public AzureTableKey NoteId { get; set; }
        public AzureTableKey EntityKey { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
    }
}
