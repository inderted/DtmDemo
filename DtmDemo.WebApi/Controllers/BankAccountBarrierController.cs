using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DtmDemo.WebApi.Data;
using DtmDemo.WebApi.Models;
using Dtmcli;
using MySqlConnector;

namespace DtmDemo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountBarrierController : ControllerBase
    {
        private readonly DtmDemoWebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDtmClient _dtmClient;
        private readonly IDtmTransFactory _transFactory;

        private readonly IBranchBarrierFactory _barrierFactory;
        private readonly ILogger<BankAccountsController> _logger;

        public BankAccountBarrierController(DtmDemoWebApiContext context, IConfiguration configuration, IDtmClient dtmClient, IDtmTransFactory transFactory, ILogger<BankAccountsController> logger, IBranchBarrierFactory barrierFactory)
        {
            this._context = context;
            this._configuration = configuration;
            this._dtmClient = dtmClient;
            this._transFactory = transFactory;
            this._logger = logger;
            _barrierFactory = barrierFactory;
        }

        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer(int fromUserId, int toUserId, decimal amount, CancellationToken cancellationToken = default)
        {
            var gid = await _dtmClient.GenGid(cancellationToken);
            var bizUrl = _configuration.GetValue<string>("BizBarrierUrl");
            var saga = _transFactory.NewSaga(gid)
                .Add(bizUrl + "/TransferOut", bizUrl + "/TransferOut_Compensate", new TransferRequest(fromUserId, amount))
                .Add(bizUrl + "/TransferIn", bizUrl + "/TransferIn_Compensate", new TransferRequest(toUserId, amount))
                ;

            await saga.Submit(cancellationToken);

            _logger.LogInformation("result gid is {0}", gid);
            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("TransferIn")]
        public async Task<IActionResult> TransferIn([FromBody] TransferRequest request)
        {
            var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);

            using (var conn = _context.Database.GetDbConnection())
            {
                var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                if (bankAccount == null)
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }

                await branchBarrier.Call(conn, async (tx) =>
                {
                    await _context.Database.UseTransactionAsync(tx);
                    bankAccount.Balance += request.Amount;
                    await _context.SaveChangesAsync();
                });
            }


            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("TransferIn_Compensate")]
        public async Task<IActionResult> TransferIn_Compensate([FromBody] TransferRequest request)
        {
            var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);

            using (var conn = _context.Database.GetDbConnection())
            {
                var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                if (bankAccount == null)
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }

                await branchBarrier.Call(conn, async (tx) =>
                {
                    await _context.Database.UseTransactionAsync(tx);
                    bankAccount.Balance -= request.Amount;
                    await _context.SaveChangesAsync();
                });
            }
            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("TransferOut")]
        public async Task<IActionResult> TransferOut([FromBody] TransferRequest request)
        {
            var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);

            using (var conn = _context.Database.GetDbConnection())
            {
                var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                if (bankAccount == null || bankAccount.Balance < request.Amount)
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }

                await branchBarrier.Call(conn, async (tx) =>
                {
                    await _context.Database.UseTransactionAsync(tx);
                    bankAccount.Balance -= request.Amount;
                    await _context.SaveChangesAsync();
                });
            }
            return Ok(new { dtm_result = "SUCCESS" });
        }

        [HttpPost("TransferOut_Compensate")]
        public async Task<IActionResult> TransferOut_Compensate([FromBody] TransferRequest request)
        {
            var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);

            using (var conn = _context.Database.GetDbConnection())
            {
                var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                if (bankAccount == null)
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }

                await branchBarrier.Call(conn, async (tx) =>
                {
                    await _context.Database.UseTransactionAsync(tx);
                    bankAccount.Balance += request.Amount;
                    await _context.SaveChangesAsync();
                });
            }
            return Ok(new { dtm_result = "SUCCESS" });
        }
    }
}
