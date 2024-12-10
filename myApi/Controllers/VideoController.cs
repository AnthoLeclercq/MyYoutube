using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myApi.Model.Video;
using myApi.Repository;

namespace myApi.Controllers
{
    [ApiController]
    public class VideoController : ControllerBase
    {

        private IVideoRepository _videoRepository;
        public VideoController(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        [HttpPost, Route("/user/{id}/video")]
        // [Authorize]
        public async Task<IActionResult> UploadVideo(int id, [FromForm] string name, [FromForm] IFormFile source)
        {
            try
            {
                VideoOutput vid = await _videoRepository.UploadVideo(id, name, source);

                return this.StatusCode(201, new
                {
                    message = "OK",
                    data = vid
                });

            } catch(Exception e)
            {
                return this.BadRequest(new
                {
                    message = e.Message,
                });
            }
        }
        [HttpPost, Route("/user/{id}/video/encoded")]
        // [Authorize]
        public async Task<IActionResult> SaveEncodedVideo( int id, [FromForm] string name, [FromForm] IFormFile source)
        {
            Console.WriteLine("Encoded video sent back to API");
            try
            {
                VideoOutput vid = await _videoRepository.SaveEncodedVideo(id, name, source);

                return this.StatusCode(201, new
                {
                    message = "OK",
                    data = vid
                });

            } catch(Exception e)
            {
                return this.BadRequest(new
                {
                    message = e.Message,
                });
            }
        }


        [HttpGet, Route("/videos")]
        public IActionResult GetVideo([FromForm] string name, [FromForm] int user, [FromForm] int duration, [FromForm] int page, [FromForm] int perPage)
        {
            try

            {
                if (page != 0)
                {

                    // if (page > Math.Round(decimal.Divide(_videoRepository.CountPagerOfVideo(name, user), perPage)))
                    // {

                    //     return this.BadRequest();
                    // }

                    List<VideoOutput> vidList = _videoRepository.GetVideo(name, user, duration, page, perPage);

                    return this.Ok(new
                    {
                        message = "OK",
                        data = vidList,
                        pager = new
                        {
                            current = page,
                            total = vidList.Count
                        }
                    });
                } else
                {
                    return this.BadRequest();
                }
            } catch (Exception e)
            {
                return this.NotFound(e.Message);
            }
        }

        [HttpPut, Route("/video/{id}")]
        [Authorize]
        public IActionResult UpdateVideo(int id, [FromForm] string name, [FromForm] int user)
        {
            try
            {

                VideoOutput vid = _videoRepository.UpdateVideo(id, name, user);

                return this.Ok(new
                {
                    message = "OK",
                    data = vid
                });
            }
            catch (Exception e)
            {
                return this.NotFound(e.Message);
            }
        }

        // [HttpPatch, Route("/video/{id}")] // this route was missing form myApi but what it should do is not very clear
        // [Authorize]
        // public IActionResult UpdateVideo(int id, [FromForm] string name, [FromForm] int user)
        // {
        //     try
        //     {

        //         VideoOutput vid = _videoRepository.UpdateVideo(id, name, user);

        //         return this.Ok(new
        //         {
        //             message = "OK",
        //             data = vid
        //         });
        //     }
        //     catch (Exception e)
        //     {
        //         return this.NotFound(e.Message);
        //     }
        // }

        [HttpDelete, Route("/video/{id}")]
        [Authorize]
        public IActionResult DeleteVideo(int id)
        {
            try
            {
                if (_videoRepository.DeleteVideo(id))
                {
                    return this.NoContent();
                }
                else
                {
                    return this.NotFound(new
                    {
                        message = "Not Found"
                    });
                }
            }
            catch (Exception e)
            {
                return this.BadRequest(new
                {
                    message = "Bad Request",
                    data = e.Message

                });
            }
        }
    }
}
