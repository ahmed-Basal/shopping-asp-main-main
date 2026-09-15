using AutoMapper;
using core.Dto;
using core.interfaces;
using core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace api.Controllers
{
    using inftastructer.Repository.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("ADD-Commnent")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto dto)
        {
            // Get userId from JWT claims
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var comment = await _commentService.AddCommentAsync(dto, userId);
            return Ok(comment);
        }

        // مثال لباقي الـ endpoints
        [HttpGet("get comment for product/{productId}")]
        [Authorize]
        public async Task<IActionResult> GetProductComments(int productId)
        {
            var comments = await _commentService.GetProductComments(productId);
            return Ok(comments);
        }

        [HttpPut("{commentId}")]
        [Authorize]
        public async Task<IActionResult> EditComment(int commentId, [FromBody] string newContent)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var updatedComment = await _commentService.EditComment(commentId, newContent, userId);
            if (updatedComment == null)
                return NotFound();

            return Ok(updatedComment);
        }

        [HttpDelete("delete comemnt for ")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var result = await _commentService.DeleteComment(commentId, userId);
            if (!result)
                return NotFound();

            return Ok(new { message = "Comment deleted successfully" });
        }
    }
}
