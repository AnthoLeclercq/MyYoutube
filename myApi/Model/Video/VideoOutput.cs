using myApi.Model.User;

namespace myApi.Model.Video
{
    public class VideoOutput
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public DateTime? Created_at { get; set; }
        public int Views { get; set; }
        public bool Enabled { get; set; }
        public UserOutput User { get; set; }

        // Faut mettre le Format mais je comprends pas

    }
}
