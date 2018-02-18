using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DinkumCoin.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;



namespace DinkumCoin.Api.Controllers
{
    [Route("api/[controller]")]
    public class WalletsController : Controller
    {
        private readonly ILogger<WalletsController> _logger;
        private readonly IDinkumRepository _dinkumRepo;
        private readonly IMiningService _miningService;

        public WalletsController(ILogger<WalletsController> logger, IDinkumRepository dinkumRepo,IMiningService miningService)
        {
            _logger = logger;
            _dinkumRepo = dinkumRepo;
            _miningService = miningService;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetAllWallets() {
            string logLine = "";

            try { 
                logLine += $"GetAllWallets: ";

                var stopwatch = new Stopwatch();
                stopwatch.Start();


                var wallets = await _dinkumRepo.GetAllWallets();

                logLine += $"succeeded fetching {wallets.Count} wallets after {stopwatch.ElapsedMilliseconds}ms";
                    var response = Ok(wallets);
                    return response;
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"{logLine}: caught exception {ex.Message}");
                return BadRequest();
            } finally
            {
                _logger.LogInformation($"{logLine}");
            }
        }

        [HttpGet("{walletId}")]
        public async Task<IActionResult> GetWallet(string walletId)
        { 
            string logLine = "";

            try {
                logLine += $"GetWallet: ";
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var wallet = await _dinkumRepo.GetWallet(new Guid(walletId));
                logLine += $"succeeded fetching wallet [{wallet.Id}] after {stopwatch.ElapsedMilliseconds}ms";

                var response = Ok(wallet);
                return response;
            } catch (Exception ex) {
                _logger.LogError(ex, $"{logLine}: caught exception {ex.Message}");

                return BadRequest(ex.Message);
            }
            finally {
                _logger.LogInformation($"{logLine}");
            }
        }

        [HttpPost("{walletId}/minecoin")]
        public async Task<IActionResult> MineCoin(string walletId)
        { 
            string logLine = "";

            try {
                logLine += $"GetWallet: ";

                var stopwatch = new Stopwatch();
                stopwatch.Start();

            var mineResult = _miningService.AttemptMineNewCoin();
                logLine += $"mine attempt for walet [{walletId}] returned after {stopwatch.ElapsedMilliseconds}ms";

                if (mineResult.CoinCreated)
                {
                    logLine += $"mine attempt was successful, new coin [{mineResult.NewCoin.Id}]";

                    var wallet = await _dinkumRepo.AddCoinToWallet(new Guid(walletId), mineResult.NewCoin);

                    var response = Accepted(wallet);
                    return response;
                }
                else
                {
                    logLine += $"mine attempt failed";

                    var response = Ok("Mining Attempt unsuccesful");
                    return response;
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, $"{logLine}: caught exception {ex.Message}");

                return BadRequest(ex.Message);
            } 
            finally {
                _logger.LogInformation($"{logLine}");
            }
        }
    }


}
