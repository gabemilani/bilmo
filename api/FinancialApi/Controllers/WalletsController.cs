using Financial.Api.Models;
using Financial.Api.Services;
using Financial.Domain.Enums;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Financial.Api.Controllers
{
    [RoutePrefix("api/wallets")]
    public class WalletsController : ApiController
    {
        private readonly WalletsAppService _service;

        public WalletsController(WalletsAppService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet, Route("{id}/transactions")]
        public IHttpActionResult GetTransactions(int id, TransactionType? type = null, int? categoryId = null, DateTime? fromDate = null, DateTime? toDate = null, int? limit = null)
        {
            var transactions = _service.GetTransactions(id, type, categoryId, fromDate, toDate, limit);
            return Ok(transactions);
        }

        [HttpGet, Route("{id}")]
        public async Task<IHttpActionResult> GetWalletAsync(int id)
        {
            var wallet = await _service.GetAsync(id);
            if (wallet == null)
                return NotFound();

            return Ok(wallet);
        }

        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> DeleteWalletAsync(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }

        [HttpPost, Route("{id}/transactions")]
        public async Task<IHttpActionResult> PostTransactionAsync(int id, [FromBody]TransactionInputModel inputModel)
        {
            await _service.AddTransactionAsync(id, inputModel);
            return Ok();
        }                
    }
}