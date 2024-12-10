using myApi.Model.Video;

namespace myApi.Repository
{
    public interface IVideoRepository
    {
        Task<VideoOutput> UploadVideo(int id, string sourceName, IFormFile video);
        Task<VideoOutput> SaveEncodedVideo(int id, string sourceName, IFormFile video);

        List<VideoOutput> GetVideo(string name, int user, int duration, int page, int perPage);

        VideoOutput UpdateVideo(int id, string name, int user);

        bool DeleteVideo(int id);

        int CountPagerOfVideo(string name, int user);

    }
}
