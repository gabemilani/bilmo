using Financial.Api.Infrastructure.Extensions;
using Financial.Api.Models;
using Financial.Api.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Financial.Api.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly UsersAppService _service;

        public UsersController(UsersAppService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> CreateUserAsync([FromBody]UserInputModel inputModel)
        {
            var user = await _service.AddAsync(inputModel);
            var url = Url.LinkController("users", user.Id);
            return Created(url, user);
        }

        [HttpPost, Route("{id}/wallets")]
        public async Task<IHttpActionResult> CreateUserWalletAsync(int id, [FromBody]WalletInputModel inputModel)
        {
            var wallet = await _service.AddUserWalletAsync(id, inputModel);
            var url = Url.LinkController("wallets", wallet.Id);
            return Created(url, wallet);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> DeleteAsync(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }

        [HttpGet, Route("{id}")]
        public async Task<IHttpActionResult> GetUserAsync(int id)
        {
            var user = await _service.GetAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet, Route("")]
        public IHttpActionResult GetUsers()
        {
            var users = _service.GetAll();
            return Ok(users);
        }

        [HttpGet, Route("{id}/wallets")]
        public async Task<IHttpActionResult> GetUserWalletsAsync(int id)
        {
            var wallets = await _service.GetWalletsAsync(id);
            return Ok(wallets);
        }

        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> ReplaceUserAsync(int id, [FromBody]UserInputModel inputModel)
        {            
            var user = await _service.ReplaceAsync(id, inputModel);
            return Ok(user);
        }
    }
}