using System.Security.Claims;
using FinTrack.Api.DTOs;
using FinTrack.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET api/transactions?year=2026&month=10
        [HttpGet]
        public async Task<IActionResult> GetByMonth(int year, int month)
        {
            if (month < 1 || month > 12)
                return BadRequest("Mes no valido");

            var transactions = await _transactionService.GetByMonth(GetUserId(), year, month);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _transactionService.GetById(GetUserId(), id);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransactionDto dto)
        {
            var transaction = await _transactionService.Create(GetUserId(), dto);
            if (transaction == null)
                return BadRequest("La categoria no existe");

            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateTransactionDto dto)
        {
            var transaction = await _transactionService.Update(GetUserId(), id, dto);
            if (transaction == null)
                return NotFound();

            return Ok(transaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _transactionService.Delete(GetUserId(), id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        // resumen para el dashboard
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(int year, int month)
        {
            if (month < 1 || month > 12)
                return BadRequest("Mes no valido");

            var summary = await _transactionService.GetSummary(GetUserId(), year, month);
            return Ok(summary);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }
    }
}
