using Financial.Api.Infrastructure.Extensions;
using Financial.Api.Models;
using Financial.Domain.Entities;
using Financial.Domain.Enums;
using Financial.Domain.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Financial.Api.Controllers
{
    [RoutePrefix("api/transaction-categories")]
    public class TransactionCategoriesController : ApiController
    {
        private readonly TransactionCategoriesService _service;

        public TransactionCategoriesController(TransactionCategoriesService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpDelete, Route("{id}")]
        public async Task<IHttpActionResult> DeleteTransactionCategoryAsync(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }

        [HttpGet, Route("")]
        public IHttpActionResult GetTransactionCategories([FromUri]TransactionType type = TransactionType.Unknown)
        {
            var categories = _service.GetMany(type);
            return Ok(categories);
        }

        /// <summary>
        /// Get specific transaction category
        /// </summary>
        /// <param name="id">Transaction category id</param>
        /// <returns>Transaction category</returns>
        [HttpGet, Route("{id}")]
        public async Task<IHttpActionResult> GetAsync(int id)
        {
            var category = await _service.GetAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        /// <summary>
        /// Create a new transaction category
        /// </summary>
        /// <param name="inputModel">Transaction category</param>
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> CreateAsync([FromBody]TransactionCategoryInputModel inputModel)
        {
            var category = AutoMapper.Mapper.Map<TransactionCategory>(inputModel);
            await _service.AddAsync(category);
            
            var url = Url.LinkController("transaction-categories", category.Id);
            return Created(url, category);
        }
    }
}