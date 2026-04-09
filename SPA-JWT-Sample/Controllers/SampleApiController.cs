using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SPA_JWT_Sample.Controllers
{
    /// <summary>
    /// 模擬一個受保護的 API 服務。
    /// 呼叫端必須在 HTTP header 帶入：
    ///   Authorization: Bearer {access_token}
    /// JWT 簽章會以 RS256 公鑰驗證，無需連回 Auth Server。
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SampleApiController : ControllerBase
    {
        /// <summary>
        /// 回傳受保護的訂單清單（模擬資源 API）
        /// </summary>
        [HttpGet("orders")]
        [Authorize(Roles = "admin")]
        public IActionResult GetOrders()
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? "unknown";

            var orders = Enumerable
                .Range(1, 3)
                .Select(i => new
                {
                    OrderId = $"ORD-{i:D4}",
                    UserId = userId,
                    Product = $"Product {i}",
                    Amount = i * 100,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                });

            return Ok(new { Data = orders, RequestedBy = userId });
        }

        /// <summary>
        /// 回傳目前 JWT 所攜帶的 Claims 資訊（方便前端確認 token 內容）
        /// </summary>
        [HttpGet("me")]
        public IActionResult Me()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new { Claims = claims });
        }
    }
}
