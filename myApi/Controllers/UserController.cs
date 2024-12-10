using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myApi.Model.Token;
using myApi.Model.User;
using myApi.Repository;

namespace myApi.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {

        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost, Route("user")]
        public IActionResult CreateUser([FromForm] UserInput newUser)
        {
            if (ModelState.IsValid)
            {
                return this.Ok(new
                {
                    message = "Ok",
                    data = _userRepository.CreateUser(newUser)
                });
            }
            else
            {
                return this.BadRequest(new
                {
                    message = "Bad Request",
                    code = "400",
                    date = ModelState
                });
            }
        }

        [HttpPost, Route("auth")]
        public IActionResult Authentification([FromForm] string login, [FromForm] string password)
        {
            try
            {
                TokenOutput token = _userRepository.Authentification(login, password);

                return this.StatusCode(201, new
                {
                    message = "ok",
                    data = token
                });


            }
            catch (Exception e)
            {
                return this.NotFound(new
                {
                    message = "L'utilisateur n'existe pas",
                });
            }
        }

        [HttpDelete, Route("/user/{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                bool result = _userRepository.DeleterUser(id);

                return this.StatusCode(204);

            }
            catch (Exception e)
            {
                return this.NotFound(e.Message);
            }
        }

        [HttpPut, Route("/user/{id}")]
        [Authorize]
        public IActionResult UpdateUser([FromForm] UserInput updateUser, int id)
        {
            try
            {
                UserOutput result = _userRepository.UpdateUser(updateUser.Username, updateUser.Pseudo, updateUser.Email, updateUser.Password, id);

                return this.Ok(new
                {
                    message = "Ok",
                    data = result
                });
            }
            catch (Exception e)
            {
                return this.NotFound(e.Message);
            }
        }

        [HttpGet, Route("/users")]
        public IActionResult GetAllUser([FromForm] string pseudo, [FromForm] int page, [FromForm] int perPage)
        {
            try
            {
                if (page != 0)
                {
                    List<UserOutput> userList = _userRepository.GetAllUser(pseudo, page, perPage);

                   
                        if (page > Math.Round(decimal.Divide(_userRepository.CountPagerOfUser(pseudo), perPage)))
                        {
                            return this.BadRequest();
                        }
                        else
                        {
                            return this.Ok(new
                            {
                                message = "Ok",
                                data = userList,
                                pager = new
                                {
                                    current = page,
                                    total = Math.Round(decimal.Divide(_userRepository.CountPagerOfUser(pseudo), perPage))
                                }
                            });
                        }
                    
                   
                }
                else
                {
                    return this.BadRequest();
                }

            }
            catch (Exception e)
            {
                return this.NotFound(e.Message);
            }
        }

        [HttpGet, Route("/user/{id}")]
        [Authorize]
        public IActionResult GetUser(string id)
        {
            try
            {
                UserOutput user = _userRepository.GetUser(id);

                return this.Ok(new
                {
                    message = "OK",
                    data = user,
                });
            } catch (Exception e)
            {
                return this.NotFound(e.Message);
            }
        }
    }
}
