using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using finTrack.Controllers.Data;
using finTrack.Dto.Transaction;
using finTrack.Interfaces;
using finTrack.Mappers;
using finTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace finTrack.Controllers
{
    [Route("[controller]")]
    public class TransactionController : Controller
    {
        private readonly IAcountRepository _acountRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly ITokenService _tokenService;

        public TransactionController(ApplicationDBContext context, IAcountRepository acountRepo, ITransactionRepository transactionRepo, ITokenService tokenService)
        {
            _acountRepo = acountRepo;
            _transactionRepo = transactionRepo;
            _tokenService = tokenService;
        }
        
        [HttpPost("confirmation")]
        public async Task<ActionResult> confirmationTransaction([FromBody]TransactionRequestDto request)
        {
            if (!HttpContext.Items.ContainsKey("UserID"))return StatusCode(400);
            var UserID = HttpContext.Items["UserID"]?.ToString();
            if (string.IsNullOrEmpty(UserID)) return StatusCode(400);

            var user = await _acountRepo.GetUserByGuidAsync(UserID);
            if(user == null)
            {
                return StatusCode(400, new {success = false});
            }

            AppUser? toUser = await _acountRepo.GetUserByUserNameAsync(request.ToUserName);
 

            if (!ModelState.IsValid)
                        return BadRequest(ModelState);
            
            if (request.Action == TransactionAction.Transfer.ToString())
            {
                if (toUser == null)
                {
                    return StatusCode(400, new { success = false, message = "Recipient not found." });
                }

                return Ok(new {
                success = true, 
                fromUser = $@"{user.FirstName} {user.LastName}", 
                toUser = $@"{toUser.FirstName} {toUser.LastName}", 
                amount = request.Amount});
            }
            else if (request.Action == TransactionAction.Deposit.ToString())
            {

                return Ok(new {
                success = true,
                amount = request.Amount});
            }
            else if (request.Action == TransactionAction.Withdraw.ToString())
            {
                return Ok(new {
                success = true,
                amount = request.Amount});
            }

            return Ok(new {
                success = true});
 
        }

        // Create a new transaction
        [HttpPost("create")]
        public async Task<ActionResult> CreateTransaction([FromBody]TransactionRequestDto request)
        {
            try
            {
                if (!HttpContext.Items.ContainsKey("UserID"))return StatusCode(400);
                var UserID = HttpContext.Items["UserID"]?.ToString();
                if (string.IsNullOrEmpty(UserID)) return StatusCode(400, new { success = false, message = "user not found."});

                var user = await _acountRepo.GetUserByGuidAsync(UserID);
                if(user == null)
                {
                    return StatusCode(400, new { success = false, message = "user not found."});
                }

                AppUser? toUser = new AppUser();

                request.UserID = user.Id;

                if (request.Action == TransactionAction.Transfer.ToString())
                {
                    toUser = await _acountRepo.GetUserByUserNameAsync(request.ToUserName);
                    request.ToUserID = toUser?.Id;
                    request.FromUserID = user.Id;
                }
                
                var transaction = request.ToTransactionFromTransactionRequestDto();

                if (!ModelState.IsValid)
                            return BadRequest(ModelState);
              
                if (transaction.Action == TransactionAction.Transfer)
                {

                    var fromUser = user; 

                    if (fromUser == null || toUser == null)
                    {
                        return StatusCode(400, new { success = false, message = "user not found."});
                    }
                    
                    fromUser.Balance -= request.Amount;

                    if (fromUser.Balance < 0)
                    {
                        return StatusCode(400, new { success = false, message = "Insufficient balance to complete the transaction."});
                    }

                    toUser.Balance += request.Amount;

                    transaction.Balance = fromUser.Balance;
                    Transaction transactionReceiver = new Transaction()
                    {
                        Amount = request.Amount,
                        Action = Enum.Parse<TransactionAction>(request.Action),
                        UserID = toUser.Id,
                        FromUserID = request?.FromUserID?? string.Empty,
                        ToUserID = toUser.Id?? string.Empty,
                        Balance = toUser.Balance
                    };

                    await _transactionRepo.AddTransactionAsync(transaction);
                    await _transactionRepo.AddTransactionAsync(transactionReceiver);
                    var fromUserUpdate = _acountRepo.UpdateUserAsync(fromUser);
                    var toUserUpdate = _acountRepo.UpdateUserAsync(toUser);

                    if (fromUserUpdate == null || toUserUpdate == null)
                    {
                        return StatusCode(400, new { success = false, message = "user not found."});
                    }
                }
                else if (transaction.Action == TransactionAction.Deposit)
                {
                    
                    user.Balance += request.Amount;
                    transaction.Balance = user.Balance;
                    await _transactionRepo.AddTransactionAsync(transaction);

                    var userUpdate = _acountRepo.UpdateUserAsync(user);

                    if (userUpdate == null)
                    {
                        return StatusCode(400, new { success = false, message = "User update failed."});
                    }
                }
                else if (transaction.Action == TransactionAction.Withdraw)
                {
                    if (user == null || user.Balance < request.Amount)
                    {
                        return BadRequest("Insufficient balance or user not found.");
                    }
                    user.Balance -= request.Amount;
                    if (user.Balance < 0)
                    {
                        return StatusCode(400, new { success = false, message = "Insufficient balance to complete the transaction."});
                    }
                    transaction.Balance = user.Balance;
            
                    await _transactionRepo.AddTransactionAsync(transaction);
                    var userUpdate = _acountRepo.UpdateUserAsync(user);

                    if (userUpdate == null)
                    {
                        return StatusCode(400, new { success = false, message = "User update failed."});
                    }
                }


                return Ok(new { success = true , newBalance = user.Balance});

            }
            catch(Exception e)
            {
                return StatusCode(500, new { success = false, message = e.Message});
            }
            
        }


        [HttpGet("GetTransactionsHistory")]
        public async Task<IActionResult> GetTransactionsHistory([FromQuery]TransactionDto request)
        {
            if (!HttpContext.Items.ContainsKey("UserID"))return StatusCode(400);
            var UserID = HttpContext.Items["UserID"]?.ToString();

            if (string.IsNullOrEmpty(UserID)) return StatusCode(400);

            var user = await _acountRepo.GetUserByGuidAsync(UserID);
            if(user == null)
            {
                return StatusCode(400);
            }
            request.UserID = user.Id;
            ViewBag.MenuType = request.menuType;

            GroupModel model = await _transactionRepo.GetTransactionsAsync(request);

            var pageTotal = (int)Math.Ceiling((double)model.totalItemCount / request.PageSize);
            ViewBag.PageNumberTotal = pageTotal;
            ViewBag.PageNumber = request.PageNumber; 
            ViewBag.FormName = "pagination";

            List<TransactionDto> result = model.transactionList.Select(x => x.ToTransactionDto()).ToList();

            return View("~/Views/Home/_TransactionHistoryListPartial.cshtml", result); 
        }
    }
}